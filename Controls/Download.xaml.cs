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
    public class DownloadObject : ObservableObject
    {
        public CoreWebView2DownloadOperation Operation { get; set; }
        public string Title { get; set; }
        private long bytesReceived;
        public long BytesReceived
        {
            get => bytesReceived;
            set => SetProperty(ref bytesReceived, value);
        }
        public long TotalBytes { get; set; }
        private string information;

        public string Information
        {
            get => information;
            set => SetProperty(ref information, value);
        }

        public string Time { get; set; }
        public DateTime DateTime { get; set; }
        public DownloadObject(CoreWebView2DownloadOperation operation)
        {
            Operation = operation;
            Title = Path.GetFileName(Operation.ResultFilePath);
            TotalBytes = Operation.TotalBytesToReceive;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime = DateTime.Now;
            Operation.BytesReceivedChanged += Operation_BytesReceivedChanged;
        }

        private void Operation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            string speed = Converters.ToFileSizeString((long)((sender.BytesReceived - BytesReceived) / (DateTime.Now - DateTime).TotalSeconds)) + "/ s";
            string information = $"Speed: {speed} Time: {DateTime.Parse(sender.EstimatedEndTime) - DateTime.Now:hh\\:mm\\:ss}";
            BytesReceived = sender.BytesReceived;
            DateTime = DateTime.Now;
            Information = information;
        }
    }

    public sealed partial class Download : Page
    {
        public static ObservableCollection<DownloadObject> DownloadList = [];

        public static Button button;

        public Download()
        {
            this.InitializeComponent();
            listView.ItemsSource = DownloadList;
            button = DownloadButton;
        }

        public static void Add(CoreWebView2DownloadOperation operation)
        {
            DownloadList.Add(new DownloadObject(operation));
        }

        private void RemoveDownloadItem(object sender, RoutedEventArgs e)
        {
            foreach (DownloadObject download in DownloadList)
            {
                if (download.Time == (sender as Button).CommandParameter as string)
                {
                    DownloadList.Remove(download);
                    return;
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
