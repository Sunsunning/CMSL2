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
    public sealed partial class NotFoundPage : Page
    {
        private Random random = new Random();
        public NotFoundPage()
        {
            this.InitializeComponent();
            QQGroup.NavigateUri = new Uri("https://github.com/Sunsunning/cmsl/issues");
            int id = random.Next(1,6);
            string name;
            switch (id)
            {
                case 1:
                    name = "С��";
                    break;
                case 2:
                    name = "С��";
                    break;
                case 3:
                    name = "С��";
                    break;
                case 4:
                    name = "м����(ǿ�ǰ�)";
                    break;
                case 5:
                    name = "С6";
                    break;
                case 6:
                    name = "����nine";
                    break;
                default:
                    name = "С��";
                    break;
            }
            eat.Text = "�϶���" + name + "�Ե���(С��)";
        }
    }
}
