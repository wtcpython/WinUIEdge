using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                Info.CheckUserSettingData();
                App.settings = JsonNode.Parse(File.ReadAllText(path));
            }
        }
    }
}
