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

        public string WebUri
        {
            get => SearchBox.Text;
            set => EdgeWebViewEngine.Source = new Uri(value);
        }

        public WebViewPage()
        {
            this.InitializeComponent();
            SetWebNaviButtonStatus();
            EdgeWebViewEngine.UpdateLayout();
            TempSearchEngineBox.ItemsSource = JsonDataList.SearchEngineDictionary.Keys;
            TempSearchEngineBox.SelectedItem = Utils.data.SearchEngine;

            if (WebUri == string.Empty)
            {
                WebUri = "https://bing.com";
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
            sender.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;

            // 加载设置项
            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;

            chromiumVersion = sender.CoreWebView2.Environment.BrowserVersionString;
        }

        private void CoreWebView2_PermissionRequested(CoreWebView2 sender, CoreWebView2PermissionRequestedEventArgs args)
        {
            
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
            mainWindow.AddNewTab(new WebViewPage() { WebUri = args.Uri });
        }

        private void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            args.DownloadOperation.BytesReceivedChanged += DownloadOperation_BytesReceivedChanged;
            Download.SetDownloadItem(args.ResultFilePath, args.DownloadOperation.TotalBytesToReceive);
            if (Utils.data.ShowFlyoutWhenStartDownloading) Download.ShowFlyout();
            args.Handled = true;
        }

        private void DownloadOperation_BytesReceivedChanged(CoreWebView2DownloadOperation sender, object args)
        {
            string infomation = $"Time: {DateTime.Now - DateTime.Parse(sender.EstimatedEndTime)}";
            Download.DownloadList.Single(x => x.Title.Equals(sender.ResultFilePath)).Information = infomation;
            Download.DownloadList.Single(x => x.Title.Equals(sender.ResultFilePath)).ReceivedBytes = sender.BytesReceived;
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            SetWebNaviButtonStatus();
            App.Window.Title = sender.DocumentTitle;
            History.SetHistory(sender.DocumentTitle, SearchBox.Text);

            MainWindow mainWindow = App.Window as MainWindow;

            var tabItem = mainWindow.TabItems.Find(x => (x as TabViewItem).Content as Page == this) as TabViewItem;
            if (tabItem != null)
            {
                tabItem.Header = sender.DocumentTitle;
                var iconUri = sender.FaviconUri;
                if (iconUri != string.Empty)
                {
                    tabItem.IconSource = new BitmapIconSource()
                    {
                        UriSource = new Uri(sender.FaviconUri)
                    };
                }
            }
        }

        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            Uri uri = new(args.Uri);
            if (!ProtocolList.Contains(uri.Scheme))
            {
                Utils.ShowContentDialog("网站警告", $"网址：{uri} 使用了不安全的 {uri.Scheme} 协议。", "确定");
                args.Cancel = true;
            }
            else
            {
                SearchBox.Text = uri.ToString();
            }
        }

        private void SearchKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            string text = SearchBox.Text;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (Uri.TryCreate(text, UriKind.Absolute, out Uri uriResult))
                {
                    if (uriResult.Scheme == "file")
                    {
                        if (File.Exists(uriResult.OriginalString))
                        {
                            string ext = Path.GetExtension(text);
                            string value;
                            if (Utils.LanguageTypeDict.TryGetValue(ext, out value))
                            {
                                (App.Window as MainWindow).AddNewTab(new TextFilePage(text, value));
                                return;
                            }

                            else if (Utils.WebFileTypeDict.TryGetValue(ext, out value))
                            {
                                (App.Window as MainWindow).AddNewTab(new TextFilePage(text, value, true));
                                return;
                            }

                            else if (Utils.ImageTypeDict.TryGetValue(ext, out _))
                            {
                                (App.Window as MainWindow).AddNewTab(new ImageViewer(text));
                                return;
                            }

                            else
                            {
                                EdgeWebViewEngine.CoreWebView2.Navigate(text);
                                return;
                            }
                        }
                    }
                    else if (ProtocolList.Contains(uriResult.Scheme))
                    {
                        EdgeWebViewEngine.CoreWebView2.Navigate(text);
                        return;
                    }
                }
                EdgeWebViewEngine.CoreWebView2.Navigate(JsonDataList.SearchEngineDictionary[(string)TempSearchEngineBox.SelectedItem] + text);
            }

            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                if (text.EndsWith(' ') && JsonDataList.SearchEngineDictionary.ContainsKey(text.TrimEnd()))
                {
                    TempSearchEngineBox.SelectedItem = text.TrimEnd();
                    SearchBox.Text = "";
                }
            }
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

        public void ShowPrintUI()
        {
            EdgeWebViewEngine.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        public async void Mute(string jsbool)
        {
            string mutefunctionString = $@"
                var videos = document.querySelectorAll('video'),
                audios = document.querySelectorAll('audio');
                [].forEach.call(videos, function(video) {{ video.muted = {jsbool}; }});
                [].forEach.call(audios, function(audio) {{ audio.muted = {jsbool}; }}); ";
            await EdgeWebViewEngine.ExecuteScriptAsync(mutefunctionString);
        }
    }
}
