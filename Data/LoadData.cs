using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

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
        public static JsonDocument LanguageDict = JsonDocument.Parse(File.ReadAllText("./Data/LanguageType.json"));
        public static JsonDocument ImageDict = JsonDocument.Parse(File.ReadAllText("./Data/ImageType.json"));
        public static List<WebsiteInfo> SearchEngineList = JsonDocument.Parse(File.ReadAllText("./Data/SearchEngine.json"))!.RootElement.EnumerateArray().Select(x => new WebsiteInfo()
        {
            Name = x.GetProperty("Name").GetString(),
            Icon = x.GetProperty("Icon").GetString(),
            Uri = x.GetProperty("Uri").GetString()
        }).ToList();
        public static ObservableCollection<WebsiteInfo> SuggestWebsiteList = new(JsonDocument.Parse(File.ReadAllText("./Data/SuggestWebsite.json"))!.RootElement.EnumerateArray().Select(x => new WebsiteInfo()
        {
            Name = x.GetProperty("Name").GetString(),
            Icon = x.GetProperty("Icon").GetString(),
            Uri = x.GetProperty("Uri").GetString()
        }));

        public static string CheckUserSettingData()
        {
            string settingsFile = "./settings.json";
            string defaultSettingsPath = "./Data/DefaultSettings.json";

            if (!File.Exists(settingsFile))
            {
                File.Copy(defaultSettingsPath, settingsFile);
            }
            else
            {
                var settingsFileKeys = GetJsonKeys(settingsFile);
                var defaultSettingsFileKeys = GetJsonKeys(defaultSettingsPath);

                if (!settingsFileKeys.SequenceEqual(defaultSettingsFileKeys))
                {
                    File.Copy(defaultSettingsPath, settingsFile, true);
                }
            }

            return settingsFile;
        }

        private static IEnumerable<string> GetJsonKeys(string filePath)
        {
            using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);
            using var doc = JsonDocument.Parse(stream);
            return doc.RootElement.EnumerateObject().Select(prop => prop.Name).ToList();
        }
    }
}