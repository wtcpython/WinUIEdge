using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Linq;

namespace Edge
{
    public sealed partial class HistoryItem : Page
    {
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
            foreach (var item in view.Items)
            {
                if ((bool)(item as CheckBox).IsChecked)
                {
                    await SettingsPage.webView2.CoreWebView2.Profile.ClearBrowsingDataAsync(Enum.Parse<CoreWebView2BrowsingDataKinds>((string)(item as CheckBox).Content));
                }
            }
            ClearBrowsingDataButton.Description = "已清理选择的项目";
        }
    }
}
