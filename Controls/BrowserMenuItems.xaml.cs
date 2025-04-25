using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Linq;

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
            MainWindow window = App.CreateNewWindow();
            window.AddHomePage();
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
            WebView2 webView2 = App.GetWebView2(this);
            webView2.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            App.mainWindows.ToList().ForEach(x => x.Close());
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
