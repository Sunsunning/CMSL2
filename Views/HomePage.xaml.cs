using CMSL.utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Effects;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CMSL
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private SelectedServer Selected = new utils.SelectedServer();
        public static event EventHandler<string> NavigateToPageRequested;
            public HomePage()
        {
            this.InitializeComponent();
            getMessage();
            if (!Selected.IsSelectedServer())
            {
                OpenServer.Content = OpenServer.Content + ":尚未选择";
            }
        }

        private void OpenServer_Click(object sender, RoutedEventArgs e)
        {
            if (!Selected.IsSelectedServer())
            {
                System.Threading.Tasks.Task task = CDialog.showDialog("发生错误", "尚未选择服务器", "现在去选", "取消", this.XamlRoot);
            }

        }
        private async Task getMessage()
        {
            string url = "https://gitee.com/chuan_yu/cmsl/raw/master/db.json";
            using (var client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(url);
                    JObject _json = JObject.Parse(json);
                    string textValue = _json["announcement"]["text"].ToString();
                    msg.Text = textValue;
                }
                catch (HttpRequestException e)
                {
                    msg.Text = $"Error fetching JSON: {e.Message}";
                }
            }
        }

        private  void DownloadServerSide_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPageRequested(this, "Add");
        }

        private async void SelectedServer_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPageRequested(this, "Play");
            
        }
    }
}
