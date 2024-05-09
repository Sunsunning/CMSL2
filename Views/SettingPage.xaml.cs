using CMSL.utils;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CMSL
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public static event EventHandler<string> BackGroundChange;
        private CSettings CSettings;
        private int updateClickCount;
        private int updateClickMax = 10;
        public SettingPage()
        {
            this.InitializeComponent();
            CSettings = new CSettings();
            Initialize();
        }

        private void Initialize()
        {
            if (CSettings.ContainsKey("notices"))
            {
                string notices = CSettings.GetValue<string>("notices");
                Selectednotices.SelectedItem = notices;
            }
            if (CSettings.ContainsKey("SaveDirectory"))
            {
                SaveDirectory.Text = CSettings.GetValue<String>("SaveDirectory");
            }
            if (CSettings.ContainsKey("DownloadSourceAPI"))
            {
                DownloadSource.Text = CSettings.GetValue<String>("DownloadSourceAPI");
            }
            if (CSettings.ContainsKey("DownloadTaskCount"))
            {
                TaskCount.Value = CSettings.GetValue<int>("DownloadTaskCount");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string item = combo.SelectedItem as string;
            if (item != null)
            {
                CSettings.SetValue("notices", item);
            }
        }

        private void SaveDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            CSettings.SetValue("SaveDirectory", SaveDirectory.Text);
        }

        private void DownloadSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            CSettings.SetValue("DownloadSourceAPI", DownloadSource.Text);
        }

        private void TaskCount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            CSettings.SetValue("DownloadTaskCount", TaskCount.Value);
        }
        private void setup_Click(object sender, RoutedEventArgs e)
        {
            var defaultSettings = CSettings.Original;

            foreach (var kvp in defaultSettings)
            {
                CSettings.SetValue(kvp.Key, kvp.Value);
            }
            Initialize();
            updateClickCount++;
            if (updateClickCount >= updateClickMax)
            {
                Random  random = new Random();
                updateClickMax = updateClickMax * 2 + random.Next(1,20);
                CDialog.showDialog("点太多啦!", "已经点击此按钮" + updateClickCount + "次了!你好无聊哦!", "我确实很无聊!", "我才不无聊!!",this.XamlRoot);
            }
        }
    }
}
