using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace Edge
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += InstallWebView2;

            if (App.settings.ShowSuggestUri)
            {
                View.ItemsSource = Info.SuggestWebsiteList;
            }
            else
            {
                View.Visibility = Visibility.Collapsed;
            }

            if (App.settings.ShowBackground)
            {
                homeGrid.Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage()
                    {
                        UriSource = new Uri(App.settings.BackgroundImage)
                    },
                    Stretch = Stretch.UniformToFill
                };
            }
        }

        public async void InstallWebView2(object sender, RoutedEventArgs e)
        {
            string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            string bootstrapUri = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
            string bootstrapPath = Path.Combine(AppContext.BaseDirectory, "MicrosoftEdgeWebview2Setup.exe");
            try
            {
                if (string.IsNullOrEmpty(version))
                {
                    using HttpClient client = new();
                    var response = await client.GetAsync(bootstrapUri);
                    response.EnsureSuccessStatusCode();

                    using var stream = await response.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(bootstrapPath, FileMode.Create, FileAccess.Write);
                    await stream.CopyToAsync(fileStream);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = bootstrapPath,
                        Arguments = "/silent /install",
                        UseShellExecute = true
                    });
                }
            }
            catch (HttpRequestException)
            {
                await warningDialog.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri(bootstrapUri));
                App.GetWindowForElement(this).Close();
            }
        }

        private void OpenSuggestWebsite(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = (e.ClickedItem as WebsiteInfo).Uri,
            });
        }

        private void OpenWebSite(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            Uri uri = ((sender as MenuFlyoutItem).DataContext as WebsiteInfo).Uri;
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = uri
            });
        }

        private void HideItem(object sender, RoutedEventArgs e)
        {
            Info.SuggestWebsiteList.Remove((sender as MenuFlyoutItem).DataContext as WebsiteInfo);
        }
    }
}