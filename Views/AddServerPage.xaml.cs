

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
using System.Linq;
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
        private CSettings CSettings = new CSettings();
        private string path = @"D:\CMSL\servers.json";
        private CDownload Download = new CDownload();
        private InstallJava install;

        public AddServerPage()
        {
            this.InitializeComponent();
            go.NavigateUri = new System.Uri(CSettings.GetValue<string>("DownloadSourceAPI"));
            install = new InstallJava();
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
                await CDialog.showDialog("����", $"�޷����ذ汾��Ϣ: {ex.Message}", "ȷ��", null, this.XamlRoot);
            }
        }
        private async void LoadLocalVersion(Server.Side side)
        {
            if (!Directory.Exists(path))
            {
                await CDialog.showDialog("����", "�б��ļ�ȱʧ", "ȷ��", null, this.XamlRoot);
                return;
            }
            await CDialog.showDialog("����", "�б��ļ�����", "ȷ��", null, this.XamlRoot);
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
            // ȷ���������붼����
            if (string.IsNullOrEmpty(ServerName.Text) || ServerSide.SelectedItem == null || version.SelectedItem == null || double.IsNaN(Max.Value) || double.IsNaN(Max.Value))
            {
                await CDialog.showDialog("����", "����д���б����ֶ�", "ȷ��", null, this.XamlRoot);
                ProgressBar.Value = 100;
                return;
            }
            if (Min.Value > Max.Value)          
            {
                await CDialog.showDialog("����", "�Ҿ�������ѧ����\n��Сֵ�������ֵ!", "ȷ��", null, this.XamlRoot);
                ProgressBar.Value = 100;
                return;
            }

            string ServerPath = @"D:\CMSL\" + ServerSide.SelectedItem + @"\" + ServerName.Text;
            if (Directory.Exists(ServerPath))
            {
                await CDialog.showDialog("����", "�������������Ѵ������㻻һ��!", "ȷ��", null, this.XamlRoot);
                ProgressBar.Value = 100;
                return;
            }
            Directory.CreateDirectory(ServerPath);
            ProgressBar.ShowPaused = false;
            ProgressBar.ShowError = false;
            ProgressBar.Value = 50;

            FileStream file = new FileStream(ServerPath + @"\start.bat", FileMode.Create, FileAccess.Write);
            FileStream config = new FileStream(ServerPath + @"\config.sci",FileMode.Create,FileAccess.Write);
            FileStream eula = new FileStream(ServerPath + @"\eula.txt",FileMode.Create, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(config)) {
                writer.Write("side=" + ServerSide.SelectedValue + "\n"+ "version=" + version.SelectedValue + "\nXms=" + Min.Value + "\nXmx=" + Max.Value);
                writer.Close();
                config.Close();
            }
            using (StreamWriter writer = new StreamWriter(eula))
            {
                writer.Write("eula=true");
                writer.Close();
                eula.Close();
            }

            using (StreamWriter writer = new StreamWriter(file))
            {
                writer.WriteLine("@ECHO OFF");
                JavaRequired.JavaVersion javaVersion = await JavaRequired.Start(version.SelectedValue as string, (Server.Side)Enum.Parse(typeof(Server.Side), ServerSide.SelectedItem.ToString(), ignoreCase: true));
                if (ServerSide.SelectedItem.Equals("Spigot"))
                {
                    writer.WriteLine(@"D:\CMSL\java\" + javaVersion + @"\bin\java.exe" + " -Xms" + Min.Value + "G" + " -Xmx" + Max.Value + "G" + " -jar spigot-" + version.SelectedItem + ".jar" + " nogui");
                    await CDialog.showDialog("����", "�����ļ��Ѵ�������������������������������ļ�:\n" + ServerSide.SelectedValue + "-" + version.SelectedItem + ".jar\n" + javaVersion,"�õ�",null,this.XamlRoot);
                    DownloadBukkit("spigot", version.SelectedItem as string,@"D:\CMSL\Spigot\" + ServerName.Text);
                }
                else
                {
                    string json = await FetchJsonFromUrlAsync("https://gitee.com/chuan_yu/cmsl/raw/master/servers.json");
                    JObject _json = JObject.Parse(json);
                    string build = _json["release"]["paper"]["build"][version.SelectedItem as string].ToString();
                    writer.WriteLine(@"D:\CMSL\java\" + javaVersion + @"\bin\java.exe" + " -Xms" + Min.Value + "G" + " -Xmx" + Max.Value + "G" + " -jar paper-" + version.SelectedItem + "-" + build + ".jar" + " nogui");
                    await CDialog.showDialog("����", "�����ļ��Ѵ�������������������������������ļ�:\n" + ServerSide.SelectedValue + "-" + version.SelectedItem + "-" + build + ".jar\n" + javaVersion, "�õ�", null, this.XamlRoot);
                    DownloadBukkit("paper", version.SelectedItem as string, @"D:\CMSL\Paper\" + ServerName.Text, build);
                }
                writer.WriteLine("pause");
                writer.Write(":: By CMSL");
                writer.Close();
                file.Close();
                ProgressBar.Value = 100;
                if (!await ProcessJava(javaVersion))
                {
                    await CDialog.showDialog("InstallJava", "���ڽ���Java����,�˲�������Ӱ��������ļ��Ĵ������ǻ���ݵ��޷�����������!", "�Ƶ�", null, this.XamlRoot);
                    install.Install(javaVersion, @"D:\CMSL\java\" + javaVersion);
                    return;
                }
            }
        }
        // https://download.getbukkit.org/spigot/spigot-1.20.4.jar
        // https://api.papermc.io/v2/projects/paper/versions/1.20.4/builds/496/downloads/paper-1.20.4-496.jar
        private async void DownloadBukkit(string side,string version,string path,string build = "22")
        {
            NavigateToPageRequested?.Invoke(this, "Download");
            List<string> urls = new List<string>();
            string[] FileName = new string[20];
            string[] description = new string[20];
            if (side.Equals("spigot"))
            {
                urls.Add("https://download.getbukkit.org/spigot/spigot-" + version + ".jar");
                FileName[0] = "spigot-" + version + ".jar";
                description[0] = "Spigot�������ڿ���������";
            }
            else
            {
                urls.Add("https://api.papermc.io/v2/projects/paper/versions/" + version + "/builds/" + build + "/downloads/paper-" + version + "-" + build + ".jar");
                FileName[0] = "paper-" + version  + "-" + build + ".jar";
                description[0] = "Paper�������ڿ���������";
            }
            await Download.Start(path, urls, FileName, description);
        }

        private async Task<bool> ProcessJava(JavaRequired.JavaVersion javaVersion)
        {
            if (install.Running)
            {
                await CDialog.showDialog("��ѽ!������!", "��ǰ��һ��Java��װ������ȴ�����װ���", "�Ƶ�", null, this.XamlRoot);
                return false;
            }

            string JavaPath = @"D:\CMSL\java\" + javaVersion;
                if (!Directory.Exists(JavaPath))
                {
                    Directory.CreateDirectory(JavaPath);
                    return false;
                }
            return true;
        }

        private void cancle_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPageRequested?.Invoke(this, "Home");
        }

        private async void ServerSide_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var side = (Server.Side)Enum.Parse(typeof(Server.Side), ServerSide.SelectedItem.ToString(), ignoreCase: true);
            if (CSettings.GetValue<bool>("DownloadList"))
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
