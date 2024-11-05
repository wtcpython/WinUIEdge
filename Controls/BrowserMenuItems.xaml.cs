using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Edge
{
    public sealed partial class BrowserMenuItems : Page
    {
        public BrowserMenuItems()
        {
            this.InitializeComponent();
        }

        private void TryCreateNewTab(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);

            mainWindow.AddHomePage();
        }

        private void TryCreateNewWindow(object sender, RoutedEventArgs e)
        {
            var window = App.CreateNewWindow();
            window.Activate();
        }

        private void TryOpenSettingPage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            TabView tabView = mainWindow.Content as TabView;

            object item = tabView.TabItems.FirstOrDefault(x => ((TabViewItem)x).Content is SettingsPage);
            if (item != null) tabView.SelectedItem = item;
            else mainWindow.AddNewTab(new SettingsPage(), "设置");
        }

        private void ShowFlyout(object sender, RoutedEventArgs e)
        {
            WebViewPage page = App.GetWindowForElement(this).SelectedItem as WebViewPage;
            page.ShowFlyout((sender as MenuFlyoutItem).Text);
        }

        private void ShowPrintUI(object sender, RoutedEventArgs e)
        {
            CoreWebView2 coreWebView2 = App.GetCoreWebView2(this);
            coreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            App.mainWindows.ForEach(x => x.Close());
        }

        private async void ScreenClip(object sender, RoutedEventArgs e)
        {
            CoreWebView2 coreWebView2 = App.GetCoreWebView2(this);
            using InMemoryRandomAccessStream stream = new();
            await coreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, stream);

            BitmapImage bitmapImage = new();
            stream.Seek(0);
            await bitmapImage.SetSourceAsync(stream);

            await ShowScreenshotDialog(bitmapImage);
        }

        private async Task ShowScreenshotDialog(BitmapImage screenshot)
        {
            ContentDialog dialog = new()
            {
                Title = "网页截图",
                CloseButtonText = "关闭",
                Content = new Image
                {
                    Source = screenshot,
                    Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform
                },
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            var items = (sender as MenuFlyout).Items.ToList()[2..^3];
            MainWindow mainWindow = App.GetWindowForElement(this);
            if (mainWindow.SelectedItem is WebViewPage page)
                items.ForEach(x => x.Visibility = Visibility.Visible);
            else
                items.ForEach(x => x.Visibility = Visibility.Collapsed);
        }
    }
}
