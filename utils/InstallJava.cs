using CommunityToolkit.WinUI.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMSL.utils
{
    class InstallJava
    {
        public bool Running = false;
        private Dictionary<JavaRequired.JavaVersion, object> DownloadJavaUrl = new Dictionary<JavaRequired.JavaVersion, object>() {
            {JavaRequired.JavaVersion.Java17 , "https://download.oracle.com/java/17/archive/jdk-17.0.10_windows-x64_bin.exe"},
            {JavaRequired.JavaVersion.Java16,"https://files02.tchspt.com/down/jdk-16.0.2_windows-x64_bin.exe" },
            {JavaRequired.JavaVersion.Java8,"https://d6.injdk.cn/openjdk/openjdk/8/openjdk-8u41-b04-windows-i586-14_jan_2020.zip" }
        };
        private CDownload CDOwnload;
        private CSettings _CSettings;
        private string _name;
        private string _path;
        public InstallJava()
        {
            DownloadTaskList.OnProcessDownload += ProcessInstall;
            CDOwnload = new CDownload();
            _CSettings = new CSettings();
        }
        public async void Install(JavaRequired.JavaVersion java,string path)
        {
            if (DownloadJavaUrl.TryGetValue(java, out var url))
            {
                List<string> urls = new List<string>() {
                    url.ToString()
                };
                string[] name = {
                    java + ".exe"
                };
                string[] des = { 
                    "服务器运行必备组件"
                };
                _name = java + ".exe";
                _path = path;
                await CDOwnload.Start(@"D:\CMSL\MyDownload",urls, name, des, 10);
            }
            Running = true;
        }
        public async void ProcessInstall(object sender,string FileName)
        {
            if (FileName == _name)
            {
                new ToastContentBuilder().AddText(FileName + "下载完毕正在进行安装……").Show();
                Setup(FileName);
                bool isInstallerRunning = await DetectJavaInstallerAsync("Java(TM) Platform SE binary", 5000);
                if (isInstallerRunning)
                {
                    Console.WriteLine("检测到Java安装程序正在运行");
                }
                else
                {
                    new ToastContentBuilder().AddText("安装完成!").Show();
                    Running = false;
                }
            }
        }
        private void Setup(string file)
        {
            
            Command(@"/c D:\CMSL\MyDownload\" + file + " /s " + "INSTALLDIR=\"" + _path + "\"");
            // 检测后台进程
        }
        private void Command(string command)
        {
            Process.Start("cmd.exe", command);
        }
        public async Task<bool> DetectJavaInstallerAsync(string installerName, int timeoutMilliseconds)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                Task<bool> detectionTask = Task.Run(() => IsJavaInstallerRunning(installerName), cancellationTokenSource.Token);
                cancellationTokenSource.CancelAfter(timeoutMilliseconds);

                try
                {
                    await detectionTask;

                    return detectionTask.Result;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Java安装程序检测超时");
                    new ToastContentBuilder().AddText( "Java安装程序检测超时").Show();
                    Running = false;
                    return false;
                }
            }
        }
        private bool IsJavaInstallerRunning(string installerName)
        {
            Process[] processes = Process.GetProcesses();
            return processes.Any(p => p.ProcessName.Contains(installerName, StringComparison.OrdinalIgnoreCase));
        }


       

    }
}
