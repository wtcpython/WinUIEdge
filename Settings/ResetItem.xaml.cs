using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Windows.Storage;

namespace Edge
{
    public sealed partial class ResetItem : Page
    {
        public ResetItem()
        {
            this.InitializeComponent();
        }

        private void ResetUserSettings(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            string path = ApplicationData.Current.LocalFolder.Path + "/settings.json";
            File.Delete(path);
            Info.CheckUserSettingData();
            App.settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(path));
        }
    }
}
