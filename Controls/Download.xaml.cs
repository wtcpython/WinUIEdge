using CommunityToolkit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Edge
{
    public partial class DownloadObject : INotifyPropertyChanged
    {
        public CoreWebView2DownloadOperation Operation { get; set; }
        public string Title { get; set; }

        private double bytesReceived;

        public double BytesReceived
        {
            get => bytesReceived;
            set
            {
                bytesReceived = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BytesReceived)));
            }
        }

        public double TotalBytes { get; set; }

        private string information;

        public string Information
        {
            get => information;
            set
            {
                information = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Information)));
            }
        }

        public DateTime DateTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadObject(CoreWebView2DownloadOperation operation)
        {
            Operation = operation;
            Title = Path.GetFileName(Operation.ResultFilePath);
            TotalBytes = Operation.TotalBytesToReceive;
            DateTime = DateTime.Now;
            Operation.BytesReceivedChanged += Operation_BytesReceivedChanged;
        }

        private void Operation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            string receivedDelta = Converters.ToFileSizeString((long)((sender.BytesReceived - BytesReceived) / (DateTime.Now - DateTime).TotalSeconds));
            string received = Converters.ToFileSizeString(sender.BytesReceived);
            string total = Converters.ToFileSizeString(sender.TotalBytesToReceive);
            string speed = receivedDelta + "/s";
            string information = $"{speed} - {received}/{total}，剩余时间：{DateTime.Parse(sender.EstimatedEndTime) - DateTime.Now:hh\\:mm\\:ss}";
            BytesReceived = sender.BytesReceived;
            DateTime = DateTime.Now;
            Information = information;
        }
    }

    public sealed partial class Download : Page
    {
        public Download()
        {
            this.InitializeComponent();
            listView.ItemsSource = App.DownloadList;
        }

        private void RemoveDownloadItem(object sender, RoutedEventArgs e)
        {
            DownloadObject deleteObject = (sender as Button).DataContext as DownloadObject;
            deleteObject.Operation.Cancel();
            App.DownloadList.Remove(deleteObject);
        }

        private void OpenDownloadFolderRequest(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", UserDataPaths.GetDefault().Downloads);
        }

        private async void DeleteDownloadRequest(object sender, RoutedEventArgs e)
        {
            WebView2 webView2 = App.GetWebView2(this);
            await webView2.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DownloadHistory);
            App.DownloadList.Clear();
        }

        private void SearchDownload(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            listView.ItemsSource = App.DownloadList.Where(x => x.Title.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void ShowFlyout()
        {
            DownloadButton.Flyout.ShowAt(DownloadButton);
        }
    }
}
