using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace Edge.Utilities
{
    public static class Dialog
    {
        public static async void ShowMsgDialog(string title, string content, string closeButtonText)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText,
                XamlRoot = App.Window.Content.XamlRoot,
                RequestedTheme = Enum.Parse<ElementTheme>(Info.data.Appearance)
            };
            await dialog.ShowAsync();
        }

        public static async Task<bool> ShowMsgDialog(string title, string content, string closeButtonText, string primaryButtonText)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText,
                PrimaryButtonText = primaryButtonText,
                XamlRoot = App.Window.Content.XamlRoot,
                RequestedTheme = Enum.Parse<ElementTheme>(Info.data.Appearance)
            };
            ContentDialogResult result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
    }
}