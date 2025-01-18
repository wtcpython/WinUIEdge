using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

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
            CoreWebView2 coreWebView2 = App.GetCoreWebView2(this);
            coreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            App.mainWindows.ForEach(x => x.Close());
        }

        private void ScreenClip(object sender, RoutedEventArgs e)
        {
            WebView2 webView2 = (App.GetWindowForElement(this).SelectedItem as WebViewPage).webView2;
            webView2.Focus(FocusState.Programmatic);

            List<INPUT> inputs =
            [
                new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = { ki = { wVk = VIRTUAL_KEY.VK_CONTROL } }
                },
                new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = { ki = { wVk = VIRTUAL_KEY.VK_SHIFT } }
                },
                new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = { ki = { wVk = VIRTUAL_KEY.VK_S } }
                },
                new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = { ki = { wVk = VIRTUAL_KEY.VK_S, dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP } }
                },
                new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = { ki = { wVk = VIRTUAL_KEY.VK_SHIFT, dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP } }
                },
                new()
                {
                    type = INPUT_TYPE.INPUT_KEYBOARD,
                    Anonymous = { ki = { wVk = VIRTUAL_KEY.VK_CONTROL, dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP } }
                }
            ];
            PInvoke.SendInput(inputs.ToArray(), Marshal.SizeOf<INPUT>());
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
