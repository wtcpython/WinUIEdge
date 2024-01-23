using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Edge
{
    public class DownloadType: ObservableObject
    {
        public string Title { get; set; }

        public long TotalBytes { get; set; }

        private long receivedBytes;

        public long ReceivedBytes
        {
            get => receivedBytes;
            set => SetProperty(ref receivedBytes, value);
        }

        private string information;

        public string Information
        {
            get => information;
            set => SetProperty(ref information, value);
        }

        public string Time { get; set; }
    }

    public sealed partial class Download : Page
    {
        //由于 List<T> 没有实现 INotifyPropertyChanged 接口，
        //因此若使用 List<T> 作为 ItemSource，则当 ListView 新增、删除 Item 时，ListView UI 会不能即时更新
        public static ObservableCollection<DownloadType> DownloadList = [];

        public static Button button;

        public Download()
        {
            this.InitializeComponent();
            listView.ItemsSource = DownloadList;
            button = DownloadButton;
        }

        public static void SetDownloadItem(string title, long totalBytes)
        {
            DownloadList.Add(new DownloadType()
            {
                Title = title,
                TotalBytes = totalBytes,
                ReceivedBytes = 0,
                Information = "",
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            }); ;
        }

        private void CommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter != null)
            {
                foreach (DownloadType download in DownloadList)
                {
                    if (download.Time == (args.Parameter as string))
                    {
                        DownloadList.Remove(download);
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

        private void OpenDownloadFolderRequest(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", GetMoreSpecialFolder.GetSpecialFolder(GetMoreSpecialFolder.SpecialFolder.Downloads));
        }

        private void SearchDownload(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || SearchDownloadBox.Text == string.Empty)
            {
                listView.ItemsSource = DownloadList.Where(x => x.Title.Contains(SearchDownloadBox.Text));
            }
        }

        public static void ShowFlyout()
        {
            button.Flyout.ShowAt(button);
        }
    }
}
