using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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

            mainWindow.AddNewTab(new HomePage());
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
            else mainWindow.AddNewTab(new SettingsPage(), "ÉèÖÃ");
        }

        private void ShowFlyout(object sender, RoutedEventArgs e)
        {
            WebViewPage page = App.GetWindowForElement(this).SelectedItem as WebViewPage;
            page.ShowFlyout((sender as MenuFlyoutItem).Text);
        }

        private void ShowPrintUI(object sender, RoutedEventArgs e)
        {
            WebViewPage page = App.GetWindowForElement(this).SelectedItem as WebViewPage;
            page.ShowPrintUI();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            App.mainWindows.ForEach(x => x.Close());
        }

        private async void ScreenClip(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-screenclip:"));
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
