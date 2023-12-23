using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;

namespace Edge
{
    public class HistoryType: ObservableObject
    {
        public string Title { get; set; }

        public string Uri { get; set; }
    }

    public sealed partial class History : Page
    {
        //由于 List<T> 没有实现 INotifyPropertyChanged 接口，
        //因此若使用 List<T> 作为 ItemSource，则当 ListView 新增、删除 Item 时，ListView UI 会不能即时更新
        public static ObservableCollection<HistoryType> HistoryList = [];

        public History()
        {
            this.InitializeComponent();
            listView.ItemsSource = HistoryList;
            listView.SelectedIndex = 0;
        }

        public static void SetHistory(string title, string uri)
        {
            HistoryList.Add(new HistoryType()
            {
                Title = title,
                Uri = uri,
            });
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object selectedItem in e.AddedItems)
                SetButtonEnabled(selectedItem, true);
            foreach (object selectedItem in e.RemovedItems)
                SetButtonEnabled(selectedItem, false);
        }

        private void SetButtonEnabled(object item, bool display)
        {
            ListViewItem container = listView.ContainerFromItem(item) as ListViewItem;
            if (container != null)
            {
                Button button = FindVisualChild<Button>(container);
                if (button != null)
                    button.IsEnabled = display;
            }
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T t)
                    return t;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void RemoveHistory(object sender, RoutedEventArgs e)
        {
            (listView.ItemsSource as ObservableCollection<HistoryType>).RemoveAt(listView.SelectedIndex);
        }

        private void ListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            string uri = HistoryList[listView.SelectedIndex].Uri;
            //ShowMoreFlyoutMenu.CreateNewWindow(new WebViewPage(uri));
        }
    }
}
