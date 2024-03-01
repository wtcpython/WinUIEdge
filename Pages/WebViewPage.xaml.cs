using Edge.Data;
using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;


namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public string WebUri
        {
            get => Search.Text;
            set => EdgeWebViewEngine.Source = new Uri(value);
        }

        public WebViewPage()
        {
            this.InitializeComponent();
            SetWebNaviButtonStatus();
            EdgeWebViewEngine.UpdateLayout();

            if (WebUri == string.Empty)
            {
                WebUri = "https://bing.com";
            }

            UABox.ItemsSource = Info.UserAgentDict.Select(x => ((JProperty)x).Name);

            homeButton.Visibility = App.settings["ToolBar"]["HomeButton"].ToObject<bool>() ? Visibility.Visible : Visibility.Collapsed;
            historyButton.Visibility = App.settings["ToolBar"]["HistoryButton"].ToObject<bool>() ? Visibility.Visible : Visibility.Collapsed;
            downloadButton.Visibility = App.settings["ToolBar"]["DownloadButton"].ToObject<bool>() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetWebNaviButtonStatus()
        {
            uriGoBackButton.IsEnabled = EdgeWebViewEngine.CanGoBack;
            if (App.settings["ToolBar"]["ForwardButton"].ToObject<bool>())
            {
                uriGoForwardButton.Visibility = EdgeWebViewEngine.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            }
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

            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage() { WebUri = args.Uri });
        }

        private async void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            args.Handled = true;
            if (App.settings["AskDownloadBehavior"].ToObject<bool>())
            {
                IntPtr hwnd = App.GetWindowHandle(this);

                StorageFile file = await Dialog.SaveFile(args.ResultFilePath, hwnd);

                args.ResultFilePath = file.Path;
            }
            Download.Add(args.DownloadOperation);
            if (App.settings["ShowFlyoutWhenStartDownloading"].ToObject<bool>()) Download.ShowFlyout();
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            SetWebNaviButtonStatus();
            MainWindow mainWindow = App.GetWindowForElement(this);

            mainWindow.Title = sender.DocumentTitle;
            History.Add(sender);

            List<object> tabs = [.. (mainWindow.Content as TabView).TabItems];

            var tabItem = tabs.Find(x => (x as TabViewItem).Content as Page == this) as TabViewItem;
            if (tabItem != null)
            {
                tabItem.Header = sender.DocumentTitle;
                string iconUri = sender.FaviconUri;
                if (iconUri != string.Empty)
                {
                    tabItem.IconSource = new ImageIconSource()
                    {
                        ImageSource = new BitmapImage()
                        {
                            UriSource = new Uri(iconUri)
                        }
                    };
                }
            }
        }

        private async void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            Uri uri = new(args.Uri);
            if (uri.Scheme == "http")
            {
                args.Cancel = true;
                await Dialog.ShowMsgDialog(
                    App.GetWindowForElement(this).Content.XamlRoot, "网站警告", $"网址：{uri} 使用了不安全的 {uri.Scheme} 协议。", "确定");
            }
            else
            {
                Search.Text = uri.ToString();
            }
        }

        private void OpenTaskManager(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            EdgeWebViewEngine.CoreWebView2.OpenTaskManagerWindow();
        }

        private void ShowHomePage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new HomePage());
        }

        private void UserAgentChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EdgeWebViewEngine.CoreWebView2 != null)
            {
                EdgeWebViewEngine.CoreWebView2.Settings.UserAgent = Info.UserAgentDict[(string)(sender as ComboBox).SelectedItem].ToString();
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

        public CoreWebView2 CoreWebView2 => EdgeWebViewEngine.CoreWebView2;
    }
}
