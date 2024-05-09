using CMSL.utils;
using CMSL.Views;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.


namespace CMSL
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private CSettings settings;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "CMSL∆Ù∂Ø∆˜";

            ExtendsContentIntoTitleBar = true;
            settings = new CSettings();

            initSettings();
            nanView.SelectedItem = MainPages;
            TrySetMicaBackdrop(true);

            HomePage.NavigateToPageRequested += SetNavigationViewSelectedItem;
            AddServerPage.NavigateToPageRequested += SetNavigationViewSelectedItem;
        }

        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                Microsoft.UI.Xaml.Media.MicaBackdrop micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
                micaBackdrop.Kind = useMicaAlt ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
                this.SystemBackdrop = micaBackdrop;

                return true; // Succeeded.
            }

            return false; // Mica is not supported on this system.
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                sender.Header = "…Ë÷√";
                PagesContent.Navigate(typeof(SettingPage));
            }
            else
            {
                var selectedItem = args.SelectedItem as NavigationViewItem;
                Type pageType = null;
                sender.Header = selectedItem.Content;

                switch (selectedItem.Tag.ToString())
                {
                    case "Home":
                        pageType = typeof(HomePage);
                        break;
                    case "AddServer":
                        pageType = typeof(AddServerPage);
                        break;
                    case "Concerning":
                        pageType= typeof(ConcerningPage);
                        break;
                    case "Download":
                        pageType = typeof(DownloadManager);
                        break;
                    default:
                        pageType = typeof(NotFoundPage);
                        break;
                }

                PagesContent.Navigate(pageType);
            }
        }
        public void SetNavigationViewSelectedItem(object sender, string tag)
        {
            switch (tag)
            {
                case "Home":
                    nanView.SelectedItem = MainPages;
                    break;
                case "Download":
                    nanView.SelectedItem = DownloadPages;
                    break;
                case "Add":
                    nanView.SelectedItem = AddServePages;
                    break;
                case "Play":
                    nanView.SelectedItem = PlayPages;
                    break;
                default:
                    nanView.SelectedItem = MainPages;
                    PagesContent.Navigate(typeof(NotFoundPage));
                    break;
            }

        }

        private void initSettings()
        {

            var defaultSettings = settings.Original;

            foreach (var kvp in defaultSettings)
            {
                if (!settings.ContainsKey(kvp.Key))
                {
                    settings.SetValue(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
