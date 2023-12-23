using System.Collections.Generic;
using static System.Net.WebRequestMethods;

namespace Edge
{
    public class JsonData
    {
        public string Theme { get; set; }
        public string StartPageBehavior { get; set; }
        public string SpecificUri { get; set; }
        public bool ShowHomeButton { get; set; }

        public string SearchEngine { get; set; }
    }

    public class JsonDataList
    {
        public static List<string> ApplicationThemeList = ["Mica", "Mica Alt", "Acrylic", "None"];
        public static List<string> StartPageBehaviorList = ["打开新标签页", "打开指定的页面"];
        public static List<string> SearchEngineNameList = ["Bing（推荐）", "Google", "百度", "搜狗", "360", "Github", "Gitee", "Bilibili"];
        public static List<string> SearchEngineUriList = [
            "https://bing.com/?q=",
            "https://www.google.com/search?q=",
            "https://www.baidu.com/s?ie={inputEncoding}&wd=",
            "https://www.sogou.com/web?ie={inputEncoding}&query=",
            "https://www.so.com/s?ie={inputEncoding}&q=",
            "https://github.com/search?q=",
            "https://gitee.com/search?utf8=%E2%9C%93&q=",
            "https://search.bilibili.com/all?keyword="];
    }
}