using ABI.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using static CMSL.utils.DownloadTaskList;

namespace CMSL.utils
{
    class CDownload
    {
        private  DownloadTaskList taskList; 
        private  Dictionary<string, Task> downloadTasks = new Dictionary<string, Task>();
        private Dictionary<WebClient, string> webClientToFileMapping = new Dictionary<WebClient, string>();
        public static event System.EventHandler<object> DownloadCompleted;

        private async Task DownloadManager(string downloadPath, List<string> urls, string[] fileNames, int threadCount, int blockSize)
        {
            // 将URL和对应的文件名封装到DownloadItem中
            var downloadItems = new List<DownloadItem>();
            for (int i = 0; i < urls.Count; i++)
            {
                downloadItems.Add(new DownloadItem
                {
                    Url = urls[i],
                    FileName = string.IsNullOrEmpty(fileNames[i]) ? GetFileNameFromUrl(urls[i]) : fileNames[i]
                });
            }

            // 根据线程数量创建WebClient列表
            var webClients = new WebClient[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                webClients[i] = new WebClient();
                webClients[i].DownloadProgressChanged += WebClient_DownloadProgressChanged;
            }

            // 存储每个文件名对应的下载任务
            foreach (var item in downloadItems)
            {
                int webClientIndex = downloadItems.IndexOf(item) % threadCount;
                WebClient client = webClients[webClientIndex];
                webClientToFileMapping[client] = item.FileName;
                Task downloadTask = ProcessDownloads(webClients[webClientIndex], downloadPath, item.Url, item.FileName, blockSize);
                downloadTasks[item.FileName] = downloadTask; // 将任务与文件名关联
            }

            try
            {
                await Task.WhenAll(downloadTasks.Values);
                Console.WriteLine("所有文件下载完成！");
            }
            catch (AggregateException ae) when (ae.InnerExceptions.Count > 0)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                    {
                        Console.WriteLine($"文件下载 {e.Message} 已被取消！");
                    }
                    else
                    {
                        Console.WriteLine($"文件下载 {e.Message} 时出错！");
                    }
                }
            }
        }

        // 辅助方法，停止特定文件名的下载任务
        public  void StopDownloadForFileName(string fileName)
        {
            if (downloadTasks.ContainsKey(fileName) && !downloadTasks[fileName].IsCompleted)
            {
                downloadTasks[fileName].Dispose(); // 取消任务
                downloadTasks.Remove(fileName); // 从字典中移除
                Console.WriteLine($"下载任务 {fileName} 已被取消。");
            }
            else
            {
                Console.WriteLine($"文件 {fileName} 不在下载中或不存在。");
            }
        }

        public void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // 计算下载的百分比
            int percentage = e.ProgressPercentage;
            if (webClientToFileMapping.TryGetValue(sender as WebClient,out string fileName))
            {
                var data = new { FileName = fileName, ProgressPercentage = percentage };
                DownloadCompleted(this,data);
            }

        }

        private string GetFileNameFromUrl(string url)
        {
            var uri = new System.Uri(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            return fileName ?? "nonename.error";
        }

        private async Task ProcessDownloads(WebClient client, string downloadPath, string url, string fileName, int blockSize)
        {
            string localFilePath = Path.Combine(downloadPath, fileName);
            try
            {
                await client.DownloadFileTaskAsync(url, localFilePath);
                taskList.RemoveItem(fileName);
                Console.WriteLine($"文件 {fileName} 下载完成！");
                taskList.RemoveItem(fileName);
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"下载文件 {fileName} 时出错: {e.Message}");
            }
        }

        public class DownloadItem
        {
            public string Url { get; set; }
            public string FileName { get; set; }
        }

        public async Task Start(string downloadPath, List<string> urls, string[] fileNames, string[] description, int threadCount = 4)
        {
            taskList = new DownloadTaskList();
            for (int i = 0; i < urls.Count; i++)
            {
                string url = urls[i];
                string file = fileNames[i];
                string des = description[i];
                taskList.AddItem(new DownloadTaskListItem { url = url, name = file, description = des });
            }
            int blockSize = 1024 * 1024; // 指定块大小这里为1MB
            await DownloadManager(downloadPath, urls, fileNames, threadCount, blockSize);
        }
    }
}