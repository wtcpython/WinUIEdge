using System.Collections.Generic;

namespace Edge
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

    public class JsonDataList
    {
        public static List<string> AppearanceList = ["System", "Light", "Dark"];
        public static List<string> WindowEffectList = ["Mica", "Mica Alt", "Acrylic", "None"];
        public static List<string> StartPageBehaviorList = ["打开新标签页", "打开指定的页面"];

        public static Dictionary<string, string> SearchEngineDictionary = new()
        {
            { "Bing", "https://bing.com/?q=" },
            { "Google", "https://www.google.com/search?q=" },
            { "Baidu", "https://www.baidu.com/s?ie={inputEncoding}&wd=" },
            { "Sougou", "https://www.sogou.com/web?ie={inputEncoding}&query=" },
            { "360", "https://www.so.com/s?ie={inputEncoding}&q=" },
            { "Github", "https://github.com/search?q=" },
            { "Gitee", "https://gitee.com/search?utf8=%E2%9C%93&q=" },
            { "Bilibili", "https://search.bilibili.com/all?keyword=" }
        };
    }
}