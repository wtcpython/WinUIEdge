using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
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

        private void ResetUserSettings(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            File.Delete(ApplicationData.Current.LocalFolder.Path + "/settings.json");
            string path = Info.CheckUserSettingData();
            App.settings = JToken.Parse(File.ReadAllText(path));
        }
    }
}
