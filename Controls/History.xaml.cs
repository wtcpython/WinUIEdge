using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace Edge
{
    public sealed partial class History : Page
    {
        public ObservableCollection<CoreWebView2> Histories = [];

        public History()
        {
            this.InitializeComponent();
            listView.ItemsSource = Histories;
        }

        private void RemoveHistoryItem(object sender, RoutedEventArgs e)
        {
            foreach (CoreWebView2 history in Histories)
            {
                if (history.Source == (sender as Button).CommandParameter as string)
                {
                    Histories.Remove(history);
                    return;
                }
            }
        }

        private async void DeleteHistoryRequest(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            if (mainWindow.SelectedItem is WebViewPage page)
            {
                await page.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.BrowsingHistory);
            }
            Histories.Clear();
        }

        private void SearchHistory(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || SearchHistoryBox.Text == string.Empty)
            {
                listView.ItemsSource = Histories.Where(x => x.DocumentTitle.Contains(SearchHistoryBox.Text));
            }
        }

        public void ShowFlyout()
        {
            HistoryButton.Flyout.ShowAt(HistoryButton);
        }
    }
}
