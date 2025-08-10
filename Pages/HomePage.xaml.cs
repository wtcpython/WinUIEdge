using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;

namespace Edge
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            this.Loaded += InstallWebView2;

            View.ItemsSource = Info.Favorites;

            if (App.settings.ShowBackground)
            {
                if (!string.IsNullOrEmpty(App.settings.BackgroundImage))
                {
                    Background = new ImageBrush()
                    {
                        ImageSource = new BitmapImage()
                        {
                            UriSource = new Uri(App.settings.BackgroundImage)
                        },
                        Stretch = Stretch.UniformToFill
                    };
                }
                else
                {
                    LoadBingImage();
                }
            }

            favoriteList.Visibility = App.settings.MenuStatus != "Never" ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void LoadBingImage()
        {
            string url = await Utilities.GetBingImageUrlAsync();
            Background = new ImageBrush()
            {
                ImageSource = new BitmapImage()
                {
                    UriSource = new Uri(url)
                },
                Stretch = Stretch.UniformToFill
            };
        }

        private async void InstallWebView2(object sender, RoutedEventArgs e)
        {
            string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            string bootstrapUri = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";
            if (!string.IsNullOrEmpty(version)) return;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"install --id=Microsoft.EdgeWebView2Runtime -e --silent",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception)
            {
                await warningDialog.ShowAsync();
                await Windows.System.Launcher.LaunchUriAsync(new Uri(bootstrapUri));
                App.GetWindowForElement(this).Close();
            }
        }

        private void OpenFavoriteWebsite(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage((e.ClickedItem as WebsiteInfo).Uri));
        }

        private void WebSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string text = args.QueryText;
            sender.Text = string.Empty;
            MainWindow mainWindow = App.GetWindowForElement(this);
            Utilities.Search(text, mainWindow);
        }
    }
}
