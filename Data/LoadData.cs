using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
        public static JToken LanguageDict = JToken.Parse(ReadPackageFileText("/Data/LanguageType.json"));
        public static JToken ImageDict = JToken.Parse(ReadPackageFileText("/Data/ImageType.json"));
        public static List<WebsiteInfo> SearchEngineList = JsonSerializer.Deserialize<List<WebsiteInfo>>(ReadPackageFileText("/Data/SearchEngine.json"))!;
        public static List<WebsiteInfo> SuggestWebsiteList = JsonSerializer.Deserialize<List<WebsiteInfo>>(ReadPackageFileText("/Data/SuggestWebsite.json"))!;

        public static List<string> WindowEffectList = ["Mica", "Mica Alt", "Acrylic", "None"];

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
                var keys1 = JToken.Parse(File.ReadAllText(settingsFile)).Select(x => ((JProperty)x).Name);
                var keys2 = JToken.Parse(File.ReadAllText(settingsInfo.FullName)).Select(x => ((JProperty)x).Name);
                if (!keys1.SequenceEqual(keys2))
                {
                    settingsInfo.CopyTo(settingsFile, true);
                }
            }
            return settingsFile;
        }
    }
}