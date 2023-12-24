using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Edge
{
    public sealed partial class ShowMoreFlyoutMenu : Page
    {
        public ShowMoreFlyoutMenu()
        {
            this.InitializeComponent();
        }

        private void TryCreateNewTab(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.Window as MainWindow;

            mainWindow.AddNewTab(new WebViewPage());
        }

        private void TryCreateNewWindow(object sender, RoutedEventArgs e)
        {
            Utils.CreateNewWindow(new WebViewPage());
        }

        private void TryCreateInPrivateWindow(object sender, RoutedEventArgs e)
        {
            Utils.ShowContentDialog(
                "InPrivate 模式不受支持", "Microsoft Edge 未提供 InPrivate API。", "确定",
                this.Content.XamlRoot);
        }

        private void TryOpenSettingPage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.Window as MainWindow;

            mainWindow.AddNewTab(new SettingsPage(), header: "Settings");
        }
    }
}
