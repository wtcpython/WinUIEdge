using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Edge
{
    public enum Effect
    {
        Mica,
        MicaAlt,
        Acrylic,
        None
    }

    public class Settings
    {
        public string Appearance { get; set; }
        public bool AskDownloadBehavior { get; set; }
        public Effect BackgroundEffect { get; set; }
        public string BackgroundImage { get; set; }
        public string SearchEngine { get; set; }
        public bool ShowBackground { get; set; }
        public bool ShowFlyoutWhenStartDownloading { get; set; }
        public bool ShowSuggestUri { get; set; }
        public string SpecificUri { get; set; }
        public int StartBehavior { get; set; }
        public Dictionary<string, bool> ToolBar { get; set; }
        public ObservableCollection<WebsiteInfo> Favorites { get; set; }
    }

    public class WebsiteInfo
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Uri Uri { get; set; }
    }

    [JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
    [JsonSerializable(typeof(Settings))]
    [JsonSerializable(typeof(ObservableCollection<WebsiteInfo>))]
    [JsonSerializable(typeof(List<WebsiteInfo>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    internal partial class JsonContext : JsonSerializerContext { }

    public static class Info
    {
        public static Dictionary<string, string> LanguageDict = JsonSerializer.Deserialize(File.ReadAllText("./Data/LanguageType.json"), JsonContext.Default.DictionaryStringString);
        public static Dictionary<string, string> ImageDict = JsonSerializer.Deserialize(File.ReadAllText("./Data/ImageType.json"), JsonContext.Default.DictionaryStringString);
        public static List<WebsiteInfo> SearchEngineList = JsonSerializer.Deserialize(File.ReadAllText("./Data/SearchEngine.json"), JsonContext.Default.ListWebsiteInfo);
        public static ObservableCollection<WebsiteInfo> SuggestWebsiteList = JsonSerializer.Deserialize(File.ReadAllText("./Data/SuggestWebsite.json"), JsonContext.Default.ObservableCollectionWebsiteInfo);

        public static Settings LoadSettings(bool overwrite = false)
        {
            string settingsFile = "./settings.json";
            string defaultSettingsPath = "./Data/DefaultSettings.json";

            if (!File.Exists(settingsFile) || overwrite)
            {
                File.Copy(defaultSettingsPath, settingsFile, overwrite);
            }
            Settings settings = JsonSerializer.Deserialize(File.ReadAllText(settingsFile), JsonContext.Default.Settings);
            return settings;
        }
    }
}
