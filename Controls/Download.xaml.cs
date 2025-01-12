using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Edge
{
    public partial class DownloadObject : ObservableObject
    {
        public CoreWebView2DownloadOperation Operation { get; set; }
        public string Title { get; set; }

        [ObservableProperty]
        private double bytesReceived;

        public double TotalBytes { get; set; }

        [ObservableProperty]
        private string information;

        public DateTime DateTime { get; set; }
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
        public ObservableCollection<DownloadObject> DownloadList = [];

        public Download()
        {
            this.InitializeComponent();
            listView.ItemsSource = DownloadList;
        }

        private void RemoveDownloadItem(object sender, RoutedEventArgs e)
        {
            DownloadObject deleteObject = (sender as Button).DataContext as DownloadObject;
            deleteObject.Operation.Cancel();
            DownloadList.Remove(deleteObject);
        }

        private void OpenDownloadFolderRequest(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", UserDataPaths.GetDefault().Downloads);
        }

        private async void DeleteDownloadRequest(object sender, RoutedEventArgs e)
        {
            CoreWebView2 coreWebView2 = App.GetCoreWebView2(this);
            await coreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DownloadHistory);
            DownloadList.Clear();
        }

        private void SearchDownload(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            listView.ItemsSource = DownloadList.Where(x => x.Title.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void ShowFlyout()
        {
            DownloadButton.Flyout.ShowAt(DownloadButton);
        }
    }
}
