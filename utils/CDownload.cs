using ABI.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CMSL.utils
{
    class CDownload
    {
        static async Task DownloadManager(string downloadPath, List<string> urls, string[] fileNames, int threadCount, int blockSize)
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

            List<Task> downloadTasks = new List<Task>();
            for (int i = 0; i < downloadItems.Count; i++)
            {
                var item = downloadItems[i];
                int webClientIndex = i % threadCount;
                downloadTasks.Add(ProcessDownloads(webClients[webClientIndex], downloadPath, item.Url, item.FileName, blockSize));
            }

            await Task.WhenAll(downloadTasks);
            Console.WriteLine("所有文件下载完成！");
        }
        private static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // 计算下载的百分比
            int percentage = e.ProgressPercentage;
            Console.WriteLine($"{e.UserState}: 下载进度: {percentage}%");
        }

        private static string GetFileNameFromUrl(string url)
        {
            var uri = new System.Uri(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            return fileName ?? "nonename.error";
        }

        private static async Task ProcessDownloads(WebClient client, string downloadPath, string url, string fileName, int blockSize)
        {
            string localFilePath = Path.Combine(downloadPath, fileName);
            try
            {
                await client.DownloadFileTaskAsync(url, localFilePath);
                Console.WriteLine($"文件 {fileName} 下载完成！");
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

        public static async Task Start(string downloadPath, List<string> urls, string[] fileNames, int threadCount = 4)
        {
            int blockSize = 1024 * 1024; // 指定块大小这里为1MB
            await DownloadManager(downloadPath, urls, fileNames, threadCount, blockSize);
        }
    }
}