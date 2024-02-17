using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Edge
{
    public sealed partial class DownloadItem : Page
    {
        public DownloadItem()
        {
            this.InitializeComponent();

            DownloadFolderCard.Description = SettingsPage.webView2.CoreWebView2.Profile.DefaultDownloadFolderPath;

            setDownloadBehavior.IsOn = Info.data.AskDownloadBehavior;
            setDownloadFlyout.IsOn = Info.data.ShowFlyoutWhenStartDownloading;
        }

        private void DownloadBehaviorChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Info.data.AskDownloadBehavior = setDownloadBehavior.IsOn;
        }

        private void ShowFlyoutChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Info.data.ShowFlyoutWhenStartDownloading = setDownloadFlyout.IsOn;
        }

        private async void ChangeDownloadFolder(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FolderPicker picker = new();

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.GetWindowForElement(this));

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.Downloads;

            var folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                DownloadFolderCard.Description = SettingsPage.webView2.CoreWebView2.Profile.DefaultDownloadFolderPath = folder.Name;
            }
        }
    }
}
