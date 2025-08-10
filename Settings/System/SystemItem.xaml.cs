using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Edge
{
    public sealed partial class SystemItem : Page
    {
        public SystemItem()
        {
            this.InitializeComponent();
            setToggleEnableGpu.IsOn = !App.settings.DisableGpu;
            restartInfoBar.IsOpen = App.NeedRestartEnvironment;
        }
        
        private void ToggleEnableGpu(object sender, RoutedEventArgs e)
        {
            if (App.settings.DisableGpu == setToggleEnableGpu.IsOn)
            {
                App.NeedRestartEnvironment = true;
                App.settings.DisableGpu = !setToggleEnableGpu.IsOn;
                restartInfoBar.IsOpen = true;
            }
        }

        private async void OpenSettingsProxy(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network-proxy"));
        }
        
        private void CloseAllWebviews(object sender, RoutedEventArgs e)
        {
            if (App.NeedRestartEnvironment)
            {
                List<TabViewItem> tabs = [];
                foreach (MainWindow window in App.mainWindows)
                {
                    foreach (object tabItem in window.TabView.TabItems)
                    {
                        if (tabItem is TabViewItem { Content: WebViewPage webViewPage } tabViewItem)
                        {
                            webViewPage.Close();
                            tabs.Add(tabViewItem);
                        }
                    }
                    foreach (TabViewItem tabViewItem in tabs)
                    {
                        window.TabView.TabItems.Remove(tabViewItem);
                    }
                }
                App.WebView2.Close();
            }
            restartInfoBar.IsOpen = false;
        }
    }
}
