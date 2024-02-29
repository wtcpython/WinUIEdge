using Edge.Utilities;
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

            var s = tabView.TabItems.Where(x => ((TabViewItem)x).Content is SettingsPage);
            if (s.Any()) tabView.SelectedItem = s.First();
            else mainWindow.AddNewTab(new SettingsPage(), "…Ë÷√");
        }

        private void ShowHistoryFlyout(object sender, RoutedEventArgs e)
        {
            History.ShowFlyout();
        }

        private void ShowDownloadFlyout(object sender, RoutedEventArgs e)
        {
            Download.ShowFlyout();
        }

        private void ShowPrintUI(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            if (mainWindow.SelectedItem is WebViewPage page)
                page.ShowPrintUI();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            foreach (Window window in App.mainWindows)
            {
                window.Close();
            }
        }

        private async void ScreenClip(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-screenclip:"));
        }
    }
}
