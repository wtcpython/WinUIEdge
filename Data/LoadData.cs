using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Edge.Data
{
    public class WebsiteInfo
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Uri { get; set; }
    }

    public static class Info
    {
        public static JObject LanguageDict = JObject.Parse(ReadPackageFileText("/Data/LanguageType.json"));
        public static JObject ImageDict = JObject.Parse(ReadPackageFileText("/Data/ImageType.json"));
        public static List<WebsiteInfo> SearchEngineList = JsonSerializer.Deserialize<List<WebsiteInfo>>(ReadPackageFileText("/Data/SearchEngine.json"))!;
        public static ObservableCollection<WebsiteInfo> SuggestWebsiteList = JsonSerializer.Deserialize<ObservableCollection<WebsiteInfo>>(ReadPackageFileText("/Data/SuggestWebsite.json"))!;
        public static Dictionary<string, List<string>> HighlightKeyWords = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(ReadPackageFileText("/Data/HighlightData.json"))!;

        public static string ReadPackageFileText(string path)
        {
            return File.ReadAllText(Package.Current.InstalledPath + path);
        }

        public static string CheckUserSettingData()
        {
            string localFolder = ApplicationData.Current.LocalFolder.Path;
            string settingsFile = localFolder + "/settings.json";
            FileInfo settingsInfo = new(Package.Current.InstalledPath + "/Data/DefaultSettings.json");
            if (!File.Exists(settingsFile))
            {
                settingsInfo.CopyTo(settingsFile);
            }
            else
            {
                var keys1 = JObject.Parse(File.ReadAllText(settingsFile)).Properties().Select(x => x.Name);
                var keys2 = JObject.Parse(File.ReadAllText(settingsInfo.FullName)).Properties().Select(x => x.Name);
                if (!keys1.SequenceEqual(keys2))
                {
                    settingsInfo.CopyTo(settingsFile, true);
                }
            }
            return settingsFile;
        }
    }
}