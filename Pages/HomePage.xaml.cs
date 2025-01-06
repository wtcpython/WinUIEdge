using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Edge
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += InstallWebView2;

            if (App.settings["ShowSuggestUri"].GetValue<bool>())
            {
                View.ItemsSource = Info.SuggestWebsiteList;
            }
            else
            {
                View.Visibility = Visibility.Collapsed;
            }

            if (App.settings["ShowBackground"].GetValue<bool>())
            {
                homeGrid.Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage()
                    {
                        UriSource = new Uri(App.settings["BackgroundImage"].ToString())
                    },
                    Stretch = Stretch.UniformToFill
                };
            }
        }

        public async void InstallWebView2(object sender, RoutedEventArgs e)
        {
            bool isWebViewInstalled = CheckWebView2();
            if (!isWebViewInstalled)
            {
                await warningDialog.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://developer.microsoft.com/zh-cn/microsoft-edge/webview2"));
                App.GetWindowForElement(this).Close();
            }
        }

        public bool CheckWebView2()
        {
            string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            if (!string.IsNullOrEmpty(version))
            {
                return true;
            }
            else
            {
                string webView2Path = Path.Combine(SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView");

                if (Directory.Exists(webView2Path))
                {
                    Environment.SetEnvironmentVariable("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", webView2Path);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        private void OpenSuggestWebsite(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = (e.ClickedItem as WebsiteInfo).Uri
            });
        }

        private void OpenWebSite(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            string Uri = ((sender as MenuFlyoutItem).DataContext as WebsiteInfo).Uri;
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = Uri
            });
        }

        private void HideItem(object sender, RoutedEventArgs e)
        {
            Info.SuggestWebsiteList.Remove(Info.SuggestWebsiteList.First(x => x.Name == ((sender as MenuFlyoutItem).DataContext as WebsiteInfo).Name));
        }
    }
}