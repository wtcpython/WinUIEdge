using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public sealed partial class HistoryItem : Page
    {
        public static List<CoreWebView2BrowsingDataKinds> values = Enum.GetValues<CoreWebView2BrowsingDataKinds>().ToList();
        public HistoryItem()
        {
            this.InitializeComponent();
            ClearBrowsingDataButton.Description = string.Empty;
            view.ItemsSource = Enum.GetNames(typeof(CoreWebView2BrowsingDataKinds)).Select(x => new CheckBox()
            {
                Content = x,
            });
        }

        private async void ClearBrowsingData(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < view.Items.Count; i++)
            {
                if ((bool)(view.Items[i] as CheckBox).IsChecked)
                {
                    await SettingsPage.webView2.CoreWebView2.Profile.ClearBrowsingDataAsync(values[i]);
                }
            }
            ClearBrowsingDataButton.Description = "已清理选择的项目";
        }
    }
}
