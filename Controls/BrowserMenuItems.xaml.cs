using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            MainWindow mainWindow = App.Window as MainWindow;

            mainWindow.AddNewTab(new WebViewPage());
        }

        private void TryCreateNewWindow(object sender, RoutedEventArgs e)
        {
            App.CreateNewWindow(new WebViewPage());
        }

        private void TryCreateInPrivateWindow(object sender, RoutedEventArgs e)
        {
            Dialog.ShowMsgDialog(
                "InPrivate 模式不受支持", "Microsoft Edge 未提供 InPrivate API。", "确定");
        }

        private void TryOpenSettingPage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.Window as MainWindow;

            var s = mainWindow.TabItems.Where(x => ((TabViewItem)x).Content is SettingsPage);
            if (s.Any()) mainWindow.SelectedItem = s.First();
            else mainWindow.AddNewTab(new SettingsPage(), header: "设置");
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
            if ((App.Window as MainWindow).SelectedItem is WebViewPage page)
                page.ShowPrintUI();
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            App.Window.Close();
        }
    }
}
