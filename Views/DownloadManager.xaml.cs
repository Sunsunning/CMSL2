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

namespace CMSL.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadManager : Page
    {
        private DownloadTaskList taskList ;
        private Dictionary<string, ProgressBar> progressBars = new Dictionary<string, ProgressBar>();
        private Dictionary<string,Button> buttonList = new Dictionary<string, Button>();
        private TextBlock text;
        private CDownload CDownload;
        public DownloadManager()
        {
            this.InitializeComponent();
            taskList = new DownloadTaskList();
            DownloadTaskList.OnDownloadTaskList += OnStartDownload;
            DownloadTaskList.OnProcessDownload += OnProcessDownload;
            CDownload.DownloadCompleted += OnProgressBarCompleted;
            CDownload = new CDownload();

            if (buttonList.Count == 0)
            {
                DownloadTaskListIsNull();
            }
        }

       public void OnProgressBarCompleted(object sender, object e)
        {
            dynamic data = e;
            string fileName = data.FileName;
            int progress = data.ProgressPercentage;
            UpdateProgressBar(fileName, progress);
        }

        public void OnStartDownload(object sender, DownloadTaskList.DownloadTaskListItem item)
        {

            MainStackPanel.Children.Remove(text);
            Button button = InitializeButton(item.name, "下载ID:" + item.id + " 具体描述:" + item.description);
            buttonList[item.name] = button;
            MainStackPanel.Children.Add(button);
        }

        public void OnProcessDownload(object sender, string fileName)
        {
            if (buttonList.TryGetValue(fileName, out Button button))
            {
                Remove(button);
            }
            buttonList.Remove(fileName);
        }

        private void DownloadTaskListIsNull()
        {
             text = new TextBlock() {
                Text = "当前没有下载任务喔",
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 150, 0, 0)
            };
            MainStackPanel.Children.Add(text);
            
        }
        private Button InitializeButton(string title, string description)
        {
            Button button = new Button
            {
                Height = 90,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(60, 0, 20, 60)
            };

            Grid grid = new Grid
            {
                RowDefinitions =
        {
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto }
        },
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = GridLength.Auto},
                    new ColumnDefinition { Width = GridLength.Auto},
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            // 添加 Title TextBlock
            TextBlock titleTextBlock = new TextBlock
            {
                Margin = new Thickness(10, 0, 780, 40),
                Text = title,
                FontSize = 15,
                FontWeight = FontWeights.Bold
            };
            grid.Children.Add(titleTextBlock);

            // 添加 Description TextBlock
            TextBlock descriptionTextBlock = new TextBlock
            {
                Margin = new Thickness(10, 20, 780, 40),
                Text = description,
                FontSize = 13
            };
            grid.Children.Add(descriptionTextBlock);

            // 添加 ProgressBar 控件并存储引用
            ProgressBar progressBar = new ProgressBar
            {
                Margin = new Thickness(10, 30, 0, 0),
                Value = 0
            };
            grid.Children.Add(progressBar);
            // 将 ProgressBar 添加到字典中，键为标题 title
            progressBars[title] = progressBar;

            // 将 Grid 设置为按钮的内容
            button.Content = grid;
            button.Click += Remove_Click;

            return button;
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            foreach (KeyValuePair<string, Button> kvp in buttonList)
            {
                if (kvp.Value == sender as Button)
                {
                    string correspondingKey = kvp.Key;
                    CDownload.StopDownloadForFileName(correspondingKey);
                    break;
                }
            }
            Remove( sender as Button);
        }
        private void Remove(Button button)
        {
            MainStackPanel.Children.Remove(button);
        }

        public void UpdateProgressBar(string uid, double newValue)
        {
            if (progressBars.TryGetValue(uid, out ProgressBar progressBar))
            {
                progressBar.Value = newValue;
            }
        }
    }
}
