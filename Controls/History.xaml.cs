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
        public static ObservableCollection<CoreWebView2> Histories = [];

        public static Button button;

        public History()
        {
            this.InitializeComponent();
            listView.ItemsSource = Histories;
            button = HistoryButton;
        }

        public static void Add(CoreWebView2 item)
        {
            if (!Histories.Where(x => x.Source == item.Source).Any()) { Histories.Add(item); }
        }

        private void CommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter != null)
            {
                foreach (CoreWebView2 history in Histories)
                {
                    if (history.Source == (args.Parameter as string))
                    {
                        Histories.Remove(history);
                        return;
                    }
                }
            }
        }

        private void ListView_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "ShowCancelButton", true);
        }

        private void ListView_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HideCancelButton", true);
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
        public static void ShowFlyout()
        {
            button.Flyout.ShowAt(button);
        }
    }
}
