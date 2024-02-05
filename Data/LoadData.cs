using Edge.Utilities;
using System.Collections.Generic;

namespace Edge.Data
{
    public class JsonData
    {
        public string Appearance { get; set; }
        public string WindowEffect { get; set; }
        public string StartPageBehavior { get; set; }
        public string SpecificUri { get; set; }
        public bool ShowHomeButton { get; set; }
        public string SearchEngine { get; set; }
        public bool AskDownloadBehavior { get; set; }
        public bool ShowFlyoutWhenStartDownloading { get; set; }
        public string DefaultDownloadFolder { get; set; }
    }

    public class WebsiteInfo
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Uri { get; set; }
    }

    public static class Info
    {
        public static JsonData data = LoadSettingsData("/Data/DefaultSettings.json");
        public static Dictionary<string, string> LanguageDict = LoadStringJsonData("/Data/LanguageType.json");
        public static Dictionary<string, string> WebFileDict = LoadStringJsonData("/Data/WebFileType.json");
        public static Dictionary<string, string> ImageDict = LoadStringJsonData("/Data/ImageType.json");
        public static Dictionary<string, string> UserAgentDict = LoadStringJsonData("/Data/UserAgent.json");
        public static List<WebsiteInfo> SearchEngineList = LoadWebsiteInfoData("/Data/SearchEngine.json");
        public static List<WebsiteInfo> SuggestWebsiteList = LoadWebsiteInfoData("/Data/SuggestWebsite.json");

        public static List<string> AppearanceList = ["System", "Light", "Dark"];
        public static List<string> WindowEffectList = ["Mica", "Mica Alt", "Acrylic", "None"];
        public static List<string> StartPageBehaviorList = ["打开主页", "打开指定的页面"];
        public static List<string> ProtocolList = ["https", "edge", "file"];

        private static Dictionary<string, string> LoadStringJsonData(string filePath)
        {
            return Files.LoadJsonFile<Dictionary<string, string>>(filePath);
        }

        private static JsonData LoadSettingsData(string filePath)
        {
            return Files.LoadJsonFile<JsonData>(filePath);
        }

        private static List<WebsiteInfo> LoadWebsiteInfoData(string filePath)
        {
            return Files.LoadJsonFile<List<WebsiteInfo>>(filePath);
        }
    }
}