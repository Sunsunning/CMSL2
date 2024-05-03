

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using ABI.System;
using CMSL.utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace CMSL.Views
{
    public sealed partial class AddServerPage : Page
    {
        public static event System.EventHandler<string> NavigateToPageRequested;

        private List<string> spigot_version = new List<string>();
        private List<string> paper_version = new List<string>();
        private CSettings _settings = new CSettings();
        private string path = @"D:\CMSL\servers.json";
        private CSettings CSettings = new CSettings();
        private string build;

        public AddServerPage()
        {
            this.InitializeComponent();
            go.NavigateUri = new System.Uri(CSettings.GetValue<string>("DownloadSourceAPI"));
        }

        private async Task LoadVersionsAsync(Server.Side side)
        {
            try
            {
                string json = await FetchJsonFromUrlAsync("https://gitee.com/chuan_yu/cmsl/raw/master/servers.json");
                UpdateVersionsList(side, json);
            }
            catch (System.Exception ex)
            {
                CDialog.showDialog("错误", $"无法加载版本信息: {ex.Message}", "确定", null, this.XamlRoot);
            }
        }
        private async void LoadLocalVersion(Server.Side side)
        {
            if (!Directory.Exists(path))
            {
                CDialog.showDialog("错误", "列表文件缺失", "确定", null, this.XamlRoot);
                return;
            }
            CDialog.showDialog("错误", "列表文件存在", "确定", null, this.XamlRoot);
            UpdateVersionsList(side, File.ReadAllText(@"D:\CMSL\servers.json"));
        }

        private async Task<string> FetchJsonFromUrlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        private void UpdateVersionsList(Server.Side side, string json)
        {
            JObject jsonObject = JObject.Parse(json);
            JArray versionsArray;
            if (side == Server.Side.Spigot)
            {
                versionsArray = (JArray)jsonObject["release"]["spigot"];
                spigot_version = versionsArray.ToObject<List<string>>();
            }
            else
            {
                versionsArray = (JArray)jsonObject["release"]["paper"]["version"];
                paper_version = versionsArray.ToObject<List<string>>();
            }

            SetVersionItemsSource(side, side == Server.Side.Spigot ? spigot_version : paper_version);
        }

        private void SetVersionItemsSource(Server.Side side, List<string> versionsList)
        {
            switch (side)
            {
                case Server.Side.Spigot:
                    version.ItemsSource = spigot_version;
                    break;
                case Server.Side.Paper:
                    version.ItemsSource = paper_version;
                    break;
            }
        }

        private async void create_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.ShowError = true;
            // 确保所有输入都已填
            if (string.IsNullOrEmpty(ServerName.Text) || ServerSide.SelectedItem == null || version.SelectedItem == null || double.IsNaN(Max.Value) || double.IsNaN(Max.Value))
            {
                CDialog.showDialog("错误", "请填写所有必填字段", "确定", null, this.XamlRoot);
                ProgressBar.Value = 100;
                return;
            }
            if (Min.Value > Max.Value) 
            {
                CDialog.showDialog("错误", "我觉得你数学不好\n最小值大于最大值!", "确定", null, this.XamlRoot);
                ProgressBar.Value = 100;
                return;
            }

            string ServerPath = @"D:\CMSL\" + ServerSide.SelectedItem + @"\" + ServerName.Text;
            if (Directory.Exists(ServerPath))
            {
                CDialog.showDialog("错误", "此命名服务器已存在请你换一个!", "确定", null, this.XamlRoot);
                ProgressBar.Value = 100;
                return;
            }
            Directory.CreateDirectory(ServerPath);
            ProgressBar.ShowPaused = false;
            ProgressBar.ShowError = false;
            ProgressBar.Value = 50;

            FileStream file = new FileStream(ServerPath + @"\start.bat", FileMode.Create, FileAccess.Write);
            FileStream config = new FileStream(ServerPath + @"\config.ini",FileMode.Create,FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(config)) {
                writer.Write("side=" + ServerSide.SelectedValue + "\n"+ "version=" + version.SelectedValue + "\nXms=" + Min.Value + "\nXmx=" + Max.Value);
                writer.Close();
                config.Close();
            }


            using (StreamWriter writer = new StreamWriter(file))
            {
                writer.WriteLine("@ECHO OFF");
                if (ServerSide.SelectedItem.Equals("Spigot"))
                {
                    writer.WriteLine("java -Xms" + Min.Value + "G" + " -Xmx" + Max.Value + "G" + " -jar spigot-" + version.SelectedItem + ".jar" + " nogui");
                }
                else
                {
                    string json = await FetchJsonFromUrlAsync("https://gitee.com/chuan_yu/cmsl/raw/master/servers.json");
                    JObject _json = JObject.Parse(json);
                    build = _json["release"]["paper"]["build"][version.SelectedItem as string].ToString();
                    writer.WriteLine("java -Xms" + Min.Value + "G" + " -Xmx" + Max.Value + "G" + " -jar paper-" + version.SelectedItem + "-" + build + ".jar" + " nogui");

                }
                writer.WriteLine("pause");
                writer.Write(":: By CMSL");
                writer.Close();
                file.Close();
                ProgressBar.Value = 100;

            }
            // 这里添加创建服务器的逻辑
        }

        private void cancle_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPageRequested?.Invoke(this, "Home");
        }

        private async void ServerSide_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var side = (Server.Side)Enum.Parse(typeof(Server.Side), ServerSide.SelectedItem.ToString(), ignoreCase: true);
            if (_settings.GetValue<bool>("DownloadList"))
            {
                await LoadVersionsAsync(side);
            }
            else
            {
                await LoadVersionsAsync(side);
            }

        }
    }
}
