using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage.Pickers;

namespace Edge
{
    public sealed partial class DownloadItem : Page
    {
        public DownloadItem()
        {
            this.InitializeComponent();

            DownloadFolderCard.Description = App.WebView2.CoreWebView2.Profile.DefaultDownloadFolderPath;

            setDownloadBehavior.IsOn = App.settings.AskDownloadBehavior;
            setDownloadFlyout.IsOn = App.settings.ShowFlyoutWhenStartDownloading;
        }

        private void DownloadBehaviorChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings.AskDownloadBehavior = setDownloadBehavior.IsOn;
        }

        private void ShowFlyoutChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings.ShowFlyoutWhenStartDownloading = setDownloadFlyout.IsOn;
        }

        private async void ChangeDownloadFolder(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FolderPicker picker = new();

            IntPtr hwnd = this.GetWindowHandle();

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                DownloadFolderCard.Description = App.WebView2.CoreWebView2.Profile.DefaultDownloadFolderPath = folder.Name;
            }
        }
    }
}
