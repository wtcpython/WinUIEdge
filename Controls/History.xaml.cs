using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.Linq;


namespace Edge
{
    public class WebViewHistory
    {
        public string DocumentTitle { get; set; }
        public string Source { get; set; }
        public string FaviconUri { get; set; }
        public string Time { get; set; }
    }

    public sealed partial class History : Page
    {
        public History()
        {
            this.InitializeComponent();
            listView.ItemsSource = App.Histories;
        }

        private async void DeleteHistoryRequest(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            if (mainWindow.SelectedItem is WebViewPage page)
            {
                await page.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.BrowsingHistory);
            }
            App.Histories.Clear();
        }

        private void SearchHistory(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || SearchHistoryBox.Text == string.Empty)
            {
                listView.ItemsSource = App.Histories.Where(x => x.DocumentTitle.Contains(SearchHistoryBox.Text));
            }
        }

        public void ShowFlyout()
        {
            HistoryButton.Flyout.ShowAt(HistoryButton);
        }

        private void OpenHistory(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);

            mainWindow.AddNewTab(new WebViewPage() { WebUri = (e.ClickedItem as WebViewHistory).Source });
        }
    }
}
