using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;

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

        public DateTime DateTime { get; set; }
    }

    public sealed partial class Download : Page
    {
        public static ObservableCollection<DownloadType> DownloadList = [];

        public static Button button;

        public Download()
        {
            this.InitializeComponent();
            listView.ItemsSource = DownloadList;
            button = DownloadButton;
        }

        public static void Add(string title, long totalBytes)
        {
            DownloadList.Add(new DownloadType()
            {
                Title = title,
                TotalBytes = totalBytes,
                ReceivedBytes = 0,
                Information = "",
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                DateTime = DateTime.Now
            }); ;
        }

        public static void SetDownloadInfo(CoreWebView2DownloadOperation operation)
        {
            DownloadType item = DownloadList.Single(x => x.Title == Path.GetFileName(operation.ResultFilePath));
            long preReceivedBytes = item.ReceivedBytes;
            DateTime preDateTime = item.DateTime;
            string speed = Converters.ToFileSizeString((long)((operation.BytesReceived - preReceivedBytes) / (DateTime.Now - preDateTime).TotalSeconds)) + "/ s";
            string information = $"Speed: {speed} Time: {DateTime.Parse(operation.EstimatedEndTime) - DateTime.Now:hh\\:mm\\:ss}";
            item.ReceivedBytes = operation.BytesReceived;
            item.DateTime = DateTime.Now;
            item.Information = information;
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
            System.Diagnostics.Process.Start("explorer", UserDataPaths.GetDefault().Downloads);
        }

        private async void DeleteDownloadRequest(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            if (mainWindow.SelectedItem is WebViewPage page)
            {
                await page.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DownloadHistory);
            }
            DownloadList.Clear();
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
