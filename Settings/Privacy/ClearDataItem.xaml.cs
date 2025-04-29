using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;

namespace Edge
{
    public class BrowserDataKind
    {
        public CoreWebView2BrowsingDataKinds Kind { get; set; }
        public string Name { get; set; }
        public bool IsChecked = false;
    }

    public sealed partial class ClearDataItem : Page
    {
        public List<BrowserDataKind> BrowserDataKindList =
        [
            new() { Kind = CoreWebView2BrowsingDataKinds.BrowsingHistory, Name = "浏览历史记录" },
            new() { Kind = CoreWebView2BrowsingDataKinds.DownloadHistory, Name = "下载历史记录" },
            new() { Kind = CoreWebView2BrowsingDataKinds.Cookies, Name = "Cookies 和其他站点数据" },
            new() { Kind = CoreWebView2BrowsingDataKinds.CacheStorage, Name = "缓存的图像和文件" },
            new() { Kind = CoreWebView2BrowsingDataKinds.PasswordAutosave, Name = "密码" },
            new() { Kind = CoreWebView2BrowsingDataKinds.GeneralAutofill, Name = "自动填充表单数据(包括表单和卡)" },
            new() { Kind = CoreWebView2BrowsingDataKinds.AllSite, Name = "站点权限" },
        ];

        public ClearDataItem()
        {
            this.InitializeComponent();
        }

        private async void ClearBrowsingData(object sender, RoutedEventArgs e)
        {
            foreach (var item in ClearBrowsingDataButton.ItemsSource as List<BrowserDataKind>)
            {
                if (item.IsChecked)
                {
                    await App.CoreWebView2Profile.ClearBrowsingDataAsync(item.Kind);
                }
            }
            ClearBrowsingDataButton.Description = "已清理选择的项目";
        }
    }
}
