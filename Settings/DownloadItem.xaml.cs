using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;

namespace Edge
{
    public sealed partial class DownloadItem : Page
    {
        public DownloadItem()
        {
            this.InitializeComponent();

            DownloadFolderCard.Description = App.CoreWebView2Profile.DefaultDownloadFolderPath;

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
            FolderPicker picker = new(this.GetWindowId())
            {
                SuggestedStartLocation = PickerLocationId.Downloads, FileTypeFilter = { { "*" } }
            };
            
            var result = await picker.PickSingleFolderAsync();
            if (result != null)
            {
                DownloadFolderCard.Description = App.CoreWebView2Profile.DefaultDownloadFolderPath = result.Path;
            }
        }
    }
}
