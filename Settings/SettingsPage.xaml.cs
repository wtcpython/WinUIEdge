using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace Edge
{
    public sealed partial class SettingsPage : Page
    {
        public static WebView2 webView2 = new();
        public SettingsPage()
        {
            this.InitializeComponent();
            navigation.SelectedItem = navigation.MenuItems.OfType<NavigationViewItem>().First();
            EnsureWebView2Async();
        }

        public async void EnsureWebView2Async()
        {
            await webView2.EnsureCoreWebView2Async();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            string tag = (string)(args.SelectedItem as NavigationViewItem).Tag;
            ContentFrame.Navigate(Type.GetType("Edge." + tag));
            navigation.Header = ((NavigationViewItem)navigation.SelectedItem)?.Content?.ToString();
        }
    }
}
