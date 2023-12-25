using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public static string chromiumVersion;

        public static List<string> ProtocolList = ["https", "edge", "file"];

        public string GetUri => uriAddressBox.Text;

        public WebViewPage()
        {
            this.InitializeComponent();
            InitWebView("");
        }

        public WebViewPage(string uri)
        {
            InitializeComponent();
            InitWebView(uri);
        }

        private void InitWebView(string uri)
        {
            SetHomeButton();
            SetWebNaviButtonStatus();
            EdgeWebViewEngine.UpdateLayout();

            if (!string.IsNullOrWhiteSpace(uri))
            {
                EdgeWebViewEngine.Source = new Uri(uri);
            }
            else
            {
                EdgeWebViewEngine.Source = new Uri("https://bing.com");
            }

            UABox.ItemsSource = Utils.UserAgentDictionary.Keys;
        }

        private void SetWebNaviButtonStatus()
        {
            uriGoBackButton.IsEnabled = EdgeWebViewEngine.CanGoBack;
            uriGoForwardButton.Visibility = EdgeWebViewEngine.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UriGoBackRequest(object sender, RoutedEventArgs e)
        {
            if (EdgeWebViewEngine.CanGoBack)
            {
                EdgeWebViewEngine.GoBack();
            }
            SetWebNaviButtonStatus();
        }

        private void UriGoForwardRequest(object sender, RoutedEventArgs e)
        {
            if (EdgeWebViewEngine.CanGoForward)
            {
                EdgeWebViewEngine.GoForward();
            }
            SetWebNaviButtonStatus();
        }

        public void Refresh()
        {
            EdgeWebViewEngine.Reload();
        }

        private void UriRefreshRequest(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public void SetHomeButton()
        {
            showHomePageButton.Visibility = Utils.data.ShowHomeButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EdgeWebViewEngine_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            // 事件绑定
            sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            sender.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.StatusBarTextChanged += CoreWebView2_StatusBarTextChanged;

            // 加载设置项
            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;

            chromiumVersion = sender.CoreWebView2.Environment.BrowserVersionString;
        }

        private void CoreWebView2_StatusBarTextChanged(CoreWebView2 sender, object args)
        {
            string text = sender.StatusBarText;
            if (text != null)
            {
                uriPreview.Text = text;
            }
            else
            {
                uriPreview.Text = string.Empty;
            }
        }

        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;

            MainWindow mainWindow = App.Window as MainWindow;
            mainWindow.AddNewTab(new WebViewPage(args.Uri));
        }

        private void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            args.Handled = true;
            args.DownloadOperation.BytesReceivedChanged += DownloadOperation_BytesReceivedChanged;
            Download.SetDownloadItem(args.ResultFilePath, args.DownloadOperation.TotalBytesToReceive);
        }

        private void DownloadOperation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            string infomation = $"Progress: {Utils.ConvertBytesToString(sender.BytesReceived)} / {Utils.ConvertBytesToString(sender.TotalBytesToReceive)}, Time: {sender.EstimatedEndTime}";
            Download.DownloadList.Single(x => x.Title.Equals(sender.ResultFilePath)).Information = infomation;
            Download.DownloadList.Single(x => x.Title.Equals(sender.ResultFilePath)).ReceivedBytes = sender.BytesReceived;
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            SetWebNaviButtonStatus();
            App.Window.Title = sender.DocumentTitle;
            History.SetHistory(sender.DocumentTitle, uriAddressBox.Text);

            MainWindow mainWindow = App.Window as MainWindow;
            for (int i = 0; i < mainWindow.GetItems().Count; i++)
            {
                if (((mainWindow.GetItems()[i] as TabViewItem).Content as Frame).Content as Page == this)
                {
                    (mainWindow.GetItems()[i] as TabViewItem).Header = EdgeWebViewEngine.CoreWebView2.DocumentTitle;
                    break;
                }
            }
        }

        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            Uri uri = new(args.Uri);
            if (!ProtocolList.Contains(uri.Scheme))
            {
                Utils.ShowContentDialog("网站警告", $"网址：{uri} 使用了不安全的 {uri.Scheme} 协议。", "确定", App.Window.Content.XamlRoot);
                args.Cancel = true;
            }
            else
            {
                uriAddressBox.Text = uri.ToString();
            }
        }

        private void Key_Down(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (Uri.TryCreate(uriAddressBox.Text, UriKind.Absolute, out Uri uriResult))
                {
                    if (uriResult.Scheme == "file")
                    {
                        if (File.Exists(uriResult.OriginalString))
                        {
                            string ext = Path.GetExtension(uriAddressBox.Text);
                            string value;
                            if (Utils.LanguageTypeDict.TryGetValue(ext, out value))
                            {
                                (App.Window as MainWindow).AddNewTab(new TextFilePage(uriAddressBox.Text, value));
                                return;
                            }

                            else if (Utils.WebFileTypeDict.TryGetValue(ext, out value))
                            {
                                (App.Window as MainWindow).AddNewTab(new TextFilePage(uriAddressBox.Text, value, true));
                                return;
                            }

                            else if (Utils.ImageTypeDict.TryGetValue(ext, out value))
                            {
                                (App.Window as MainWindow).AddNewTab(new ImageViewer(uriAddressBox.Text, value));
                                return;
                            }

                            else
                            {
                                EdgeWebViewEngine.CoreWebView2.Navigate(uriAddressBox.Text);
                                return;
                            }
                        }
                    }
                    else if (ProtocolList.Contains(uriResult.Scheme))
                    {
                        EdgeWebViewEngine.CoreWebView2.Navigate(uriAddressBox.Text);
                        return;
                    }
                }
                EdgeWebViewEngine.CoreWebView2.Navigate(JsonDataList.SearchEngineDictionary[Utils.data.SearchEngine] + uriAddressBox.Text);
            }
        }

        private void OpenDownloadFolderRequest(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", GetMoreSpecialFolder.GetSpecialFolder(GetMoreSpecialFolder.SpecialFolder.Downloads));
        }

        public bool CheckInput(string input)
        {
            //App.Window.Title = input;
            if (Uri.TryCreate(input, UriKind.Absolute, out Uri uriResult))
            {
                if (uriResult.Scheme == "file")
                {
                    if (File.Exists(uriResult.OriginalString))
                    {
                        string ext = Path.GetExtension(input);
                        
                        return true;
                    }
                }
                else if (ProtocolList.Contains(uriResult.Scheme))
                {
                    return true;
                }
            }
            return false;
        }

        private void OpenTaskManager(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            EdgeWebViewEngine.CoreWebView2.OpenTaskManagerWindow();
        }

        private void ShowHomePage(object sender, RoutedEventArgs e)
        {

        }

        private void UserAgentChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EdgeWebViewEngine.CoreWebView2 != null)
            {
                EdgeWebViewEngine.CoreWebView2.Settings.UserAgent = Utils.UserAgentDictionary[(string)(sender as ComboBox).SelectedItem];
                Refresh();
            }
        }

        private void OpenDevToolRequest(object sender, RoutedEventArgs e)
        {
            EdgeWebViewEngine.CoreWebView2.OpenDevToolsWindow();
        }
    }
}
