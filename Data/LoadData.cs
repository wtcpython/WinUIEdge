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
        public static Dictionary<string, string> LanguageDict = LoadStringJsonData("/Data/LanguageType.json");
        public static Dictionary<string, string> ImageDict = LoadStringJsonData("/Data/ImageType.json");
        public static Dictionary<string, string> UserAgentDict = LoadStringJsonData("/Data/UserAgent.json");
        public static List<WebsiteInfo> SearchEngineList = LoadWebsiteInfoData("/Data/SearchEngine.json");
        public static List<WebsiteInfo> SuggestWebsiteList = LoadWebsiteInfoData("/Data/SuggestWebsite.json");

        public static List<string> WindowEffectList = ["Mica", "Mica Alt", "Acrylic", "None"];

        private static Dictionary<string, string> LoadStringJsonData(string filePath)
        {
            return LoadJsonFile<Dictionary<string, string>>(filePath);
        }

        private static List<WebsiteInfo> LoadWebsiteInfoData(string filePath)
        {
            return LoadJsonFile<List<WebsiteInfo>>(filePath);
        }

        public static T LoadJsonFile<T>(string filePath)
        {
            string content = File.ReadAllText(Package.Current.InstalledPath + filePath);
            T data = JsonSerializer.Deserialize<T>(content)!;
            return data;
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
                if(keys1 != keys2)
                {
                    settingsInfo.CopyTo(settingsFile, true);
                }
            }
            return settingsFile;
        }
    }
}