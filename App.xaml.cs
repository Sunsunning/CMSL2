using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CMSL
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            // 我才是最抽的人!
            CreateDirectory("D:\\CMSL");
            CreateDirectory("D:\\CMSL\\MyDownload");
            CreateDirectory("D:\\CMSL\\java");
            CreateDirectory("D:\\CMSL\\Spigot");
            CreateDirectory("D:\\CMSL\\Paper");
            CreateDirectory("D:\\CMSL\\Forge-test");
            CreateDirectory("D:\\CMSL\\Fabric-test");
        }

        private void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            var ScreenHeight = DisplayArea.Primary.WorkArea.Height;
            m_window.AppWindow.MoveAndResize(new RectInt32(500, (int)(ScreenHeight - 40 - 700), 1011, 533));
            m_window.Activate();

            
        }

        private Window m_window;
    }
}
