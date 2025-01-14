using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage;

namespace Edge
{
    public sealed partial class ResetItem : Page
    {
        public ResetItem()
        {
            this.InitializeComponent();
        }

        private async void ResetUserSettings(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialogResult res = await resetDialog.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                string path = ApplicationData.Current.LocalFolder.Path + "/settings.json";
                File.Delete(path);
                App.settings = Info.LoadSettings(true);
            }
        }
    }
}
