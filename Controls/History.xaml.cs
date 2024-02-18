using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace Edge
{
    public class HistoryType: ObservableObject
    {
        public string Title { get; set; }

        public string Uri { get; set; }

        public string Time { get; set; }
    }

    public sealed partial class History : Page
    {
        //由于 List<T> 没有实现 INotifyPropertyChanged 接口，
        //因此若使用 List<T> 作为 ItemSource，则当 ListView 新增、删除 Item 时，ListView UI 会不能即时更新
        public static ObservableCollection<HistoryType> HistoryList = [];

        public static Button button;

        public History()
        {
            this.InitializeComponent();
            listView.ItemsSource = HistoryList;
            button = HistoryButton;
        }

        public static void SetHistory(string title, string uri)
        {
            HistoryList.Add(new HistoryType()
            {
                Title = title,
                Uri = uri,
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            }); ;
        }

        private void CommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter != null)
            {
                foreach (HistoryType history in HistoryList)
                {
                    if (history.Time == (args.Parameter as string))
                    {
                        HistoryList.Remove(history);
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
            HistoryList.Clear();
        }

        private void SearchHistory(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || SearchHistoryBox.Text == string.Empty)
            {
                listView.ItemsSource = HistoryList.Where(x => x.Title.Contains(SearchHistoryBox.Text));
            }
        }
        public static void ShowFlyout()
        {
            button.Flyout.ShowAt(button);
        }
    }
}
