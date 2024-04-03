using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public class BrowserDataKind
    {
        public CoreWebView2BrowsingDataKinds Kind { get; set; }
        public string Name { get; set; }
        public bool IsChecked = false;
    }
    public sealed partial class PrivacyItem : Page
    {
        public List<BrowserDataKind> BrowserDataKindList =
        [
            new() { Kind = CoreWebView2BrowsingDataKinds.BrowsingHistory, Name = "浏览历史记录"},
            new() { Kind = CoreWebView2BrowsingDataKinds.CacheStorage, Name = "缓存资源" },
            new() { Kind = CoreWebView2BrowsingDataKinds.Cookies, Name = "Cookies和网站数据" },
            new() { Kind = CoreWebView2BrowsingDataKinds.DownloadHistory, Name = "下载历史记录" },
            new() { Kind = CoreWebView2BrowsingDataKinds.DiskCache, Name = "磁盘缓存" },
            new() { Kind = CoreWebView2BrowsingDataKinds.IndexedDb, Name = "IndexedDB 数据存储" },
            new() { Kind = CoreWebView2BrowsingDataKinds.LocalStorage, Name = "本地存储数据" },
            new() { Kind = CoreWebView2BrowsingDataKinds.PasswordAutosave, Name = "密码自动填充" },
            new() { Kind = CoreWebView2BrowsingDataKinds.WebSql, Name = "WebSQL 数据库" },
        ];

        public List<string> trackLevelList = [.. Enum.GetNames(typeof(CoreWebView2TrackingPreventionLevel))];

        public PrivacyItem()
        {
            this.InitializeComponent();
            trackBox.ItemsSource = trackLevelList;
            trackBox.SelectedIndex = trackLevelList.IndexOf(App.webView2.CoreWebView2.Profile.PreferredTrackingPreventionLevel.ToString());
            ClearBrowsingDataButton.ItemsSource = BrowserDataKindList;
        }

        private async void ClearBrowsingData(object sender, RoutedEventArgs e)
        {
            foreach (var item in ClearBrowsingDataButton.ItemsSource as List<BrowserDataKind>)
            {
                if (item.IsChecked)
                {
                    await App.webView2.CoreWebView2.Profile.ClearBrowsingDataAsync(item.Kind);
                }
            }
            ClearBrowsingDataButton.Description = "已清理选择的项目";
        }

        private void TrackLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            string level = (sender as ComboBox).SelectedItem as string;
            App.webView2.CoreWebView2.Profile.PreferredTrackingPreventionLevel = Enum.Parse<CoreWebView2TrackingPreventionLevel>(level);
        }
    }
}
