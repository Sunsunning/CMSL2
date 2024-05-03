using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSL.utils
{
    class CDialog
    {

        public static async Task showDialog(string title, string text, string FirstButtonText, string SecondButtonText, XamlRoot xaml)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.TextWrapping = TextWrapping.Wrap;
            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = textBlock
            };

            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = FirstButtonText,
                SecondaryButtonText = SecondButtonText,
                DefaultButton = ContentDialogButton.Primary,
                Content = scrollViewer,
                XamlRoot = xaml
            };

            await dialog.ShowAsync();
        }
        public static async void showDialog(string title, string text, string FirstButtonText, string SecondButtonText, string CloseButtonText, XamlRoot xaml)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.TextWrapping = TextWrapping.Wrap;
            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = textBlock
            };

            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                PrimaryButtonText = FirstButtonText,
                SecondaryButtonText = SecondButtonText,
                CloseButtonText = CloseButtonText,
                DefaultButton = ContentDialogButton.Primary,
                Content = scrollViewer,
                XamlRoot = xaml
            };

            await dialog.ShowAsync();
        }

    }
}
