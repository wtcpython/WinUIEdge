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

    public class TrackKind
    {
        public string Title { get; set; }
        public List<string> Description { get; set; }
    }

    public sealed partial class PrivacyItem : Page
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

        public List<TrackKind> TrackKindList =
        [
            new() { Title = "基本", Description = ["允许所有站点中的大多数跟踪器", "内容和广告可能会经过个性化处理", "站点将按预期工作", "阻止已知的有害跟踪器"] },
            new() { Title = "平衡", Description = ["阻止来自你尚未访问的站点的跟踪器", "内容和广告的个性化程度可能降低", "站点将按预期工作", "阻止已知的有害跟踪器"] },
            new() { Title = "严格", Description = ["阻止来自所有站点的大多数跟踪器", "内容和广告的个性化程度可能降至最低", "部分站点可能无法工作", "阻止已知的有害跟踪器"] },
        ];

        public List<CoreWebView2TrackingPreventionLevel> trackLevelList = [.. Enum.GetValues<CoreWebView2TrackingPreventionLevel>()];

        public PrivacyItem()
        {
            this.InitializeComponent();
            trackView.ItemsSource = TrackKindList;

            var level = App.webView2.CoreWebView2.Profile.PreferredTrackingPreventionLevel;
            bool isTrackOn = level != CoreWebView2TrackingPreventionLevel.None;
            if (isTrackOn)
            {
                trackView.SelectedIndex = trackLevelList.IndexOf(level) - 1;
            }
            else
            {
                trackView.SelectedIndex = 1;
            }
            trackSwitch.IsOn = isTrackOn;

            msSmartScreen.IsOn = App.webView2.CoreWebView2.Settings.IsReputationCheckingRequired;
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

        private void TrackLevelToggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn)
            {
                App.webView2.CoreWebView2.Profile.PreferredTrackingPreventionLevel = trackLevelList[trackView.SelectedIndex + 1];
            }
            else
            {
                App.webView2.CoreWebView2.Profile.PreferredTrackingPreventionLevel = CoreWebView2TrackingPreventionLevel.None;
            }
        }

        private void TrackChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trackSwitch.IsOn)
            {
                App.webView2.CoreWebView2.Profile.PreferredTrackingPreventionLevel = trackLevelList[trackView.SelectedIndex + 1];
            }
        }

        private void SmartScreenChanged(object sender, RoutedEventArgs e)
        {
            App.webView2.CoreWebView2.Settings.IsReputationCheckingRequired = (sender as ToggleSwitch).IsOn;
        }
    }
}
