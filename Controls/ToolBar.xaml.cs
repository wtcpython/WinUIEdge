using CommunityToolkit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;


namespace Edge
{
    public class WebViewHistory
    {
        public string DocumentTitle { get; set; }
        public string Source { get; set; }
        public Uri FaviconUri { get; set; }
        public string Time { get; set; }
    }

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

    public sealed partial class ToolBar : UserControl
    {
        public ToolBar()
        {
            this.InitializeComponent();
            historyList.ItemsSource = App.Histories;
            downloadList.ItemsSource = App.DownloadList;

            HistoryButton.Visibility = App.settings.ToolBar!["HistoryButton"] ? Visibility.Visible : Visibility.Collapsed;
            DownloadButton.Visibility = App.settings.ToolBar!["DownloadButton"] ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SearchHistory(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            historyList.ItemsSource = App.Histories.Where(x => x.DocumentTitle.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void OpenHistory(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);

            mainWindow.AddNewTab(new WebViewPage() { WebUri = new Uri((e.ClickedItem as WebViewHistory).Source) });
        }

        private void RemoveDownloadItem(object sender, RoutedEventArgs e)
        {
            DownloadObject deleteObject = (sender as Button).DataContext as DownloadObject;
            deleteObject.Operation.Cancel();
            App.DownloadList.Remove(deleteObject);
        }

        private void SearchDownload(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            historyList.ItemsSource = App.DownloadList.Where(x => x.Title.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void ShowFlyout(string name)
        {
            switch (name)
            {
                case "下载":
                    DownloadButton.ShowFlyout();
                    break;
                case "历史记录":
                    HistoryButton.ShowFlyout();
                    break;
                case "收藏夹":
                    FavoriteButton.ShowFlyout();
                    break;
            }
        }
    }
}
