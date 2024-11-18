using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public WebViewPage()
        {
            InitializeComponent();
            InitializeToolbarVisibility();
            SetNavigationButtonStatus();
            EdgeWebViewEngine.CoreWebView2Initialized += WebView2Initialized;
        }

        public string WebUri
        {
            get => EdgeWebViewEngine.Source.ToString();
            set => EdgeWebViewEngine.Source = new Uri(value);
        }

        private void InitializeToolbarVisibility()
        {
            homeButton.Visibility = App.settings["ToolBar"]!["HomeButton"].GetValue<bool>() ? Visibility.Visible : Visibility.Collapsed;
            historyButton.Visibility = App.settings["ToolBar"]!["HistoryButton"].GetValue<bool>() ? Visibility.Visible : Visibility.Collapsed;
            downloadButton.Visibility = App.settings["ToolBar"]!["DownloadButton"].GetValue<bool>() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetNavigationButtonStatus()
        {
            uriGoBackButton.IsEnabled = EdgeWebViewEngine.CanGoBack;
            uriGoForwardButton.Visibility = App.settings["ToolBar"]!["ForwardButton"].GetValue<bool>() && EdgeWebViewEngine.CanGoForward
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void WebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.NavigationStarting += (s, e) => Search.Text = e.Uri;
            sender.CoreWebView2.DOMContentLoaded += (s, e) => UpdatePageTitleAndHistory();
            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.StatusBarTextChanged += (s, e) => uriPreview.Text = s.StatusBarText;
            sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;

            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        private void UpdatePageTitleAndHistory()
        {
            SetNavigationButtonStatus();
            var mainWindow = App.GetWindowForElement(this);
            mainWindow.Title = EdgeWebViewEngine.CoreWebView2.DocumentTitle;

            App.Histories.Add(new WebViewHistory()
            {
                DocumentTitle = mainWindow.Title,
                Source = WebUri,
                FaviconUri = EdgeWebViewEngine.CoreWebView2.FaviconUri,
                Time = DateTime.Now.ToString()
            });

            UpdateTabIconAndTitle(mainWindow);
        }

        private void UpdateTabIconAndTitle(MainWindow mainWindow)
        {
            var tabView = mainWindow.Content as TabView;
            var tabItem = tabView?.TabItems
                .FirstOrDefault(x => ((x as TabViewItem)?.Content as Page) == this) as TabViewItem;

            if (tabItem != null)
            {
                tabItem.Header = EdgeWebViewEngine.CoreWebView2.DocumentTitle;
                if (!string.IsNullOrEmpty(EdgeWebViewEngine.CoreWebView2.FaviconUri))
                {
                    tabItem.IconSource = new ImageIconSource()
                    {
                        ImageSource = new BitmapImage(new Uri(EdgeWebViewEngine.CoreWebView2.FaviconUri))
                    };
                }
            }
        }

        private void CoreWebView2_ContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            args.Handled = true;
            var deferral = args.GetDeferral();
            var menuFlyout = new MenuFlyout();

            PopulateContextMenuItems(args, args.MenuItems, menuFlyout.Items);

            menuFlyout.Closed += (s, e) => deferral.Complete();
            menuFlyout.ShowAt(EdgeWebViewEngine, args.Location);
        }

        private void PopulateContextMenuItems(CoreWebView2ContextMenuRequestedEventArgs args, IList<CoreWebView2ContextMenuItem> webMenuItems, IList<MenuFlyoutItemBase> menuItems)
        {
            foreach (var menuItem in webMenuItems)
            {
                menuItems.Add(menuItem.Kind switch
                {
                    CoreWebView2ContextMenuItemKind.Separator => new MenuFlyoutSeparator(),
                    CoreWebView2ContextMenuItemKind.Submenu => CreateSubMenuItem(args, menuItem),
                    CoreWebView2ContextMenuItemKind.CheckBox => CreateToggleMenuItem(menuItem),
                    CoreWebView2ContextMenuItemKind.Command => CreateCommandMenuItem(args, menuItem),
                    _ => null
                });
            }
        }

        private MenuFlyoutSubItem CreateSubMenuItem(CoreWebView2ContextMenuRequestedEventArgs args, CoreWebView2ContextMenuItem menuItem)
        {
            var subItem = new MenuFlyoutSubItem
            {
                Text = menuItem.Label.Split('(')[0],
                IsEnabled = menuItem.IsEnabled
            };

            PopulateContextMenuItems(args, menuItem.Children, subItem.Items);
            return subItem;
        }

        private ToggleMenuFlyoutItem CreateToggleMenuItem(CoreWebView2ContextMenuItem menuItem)
        {
            return new ToggleMenuFlyoutItem
            {
                Text = menuItem.Label.Split('(')[0],
                IsEnabled = menuItem.IsEnabled,
                IsChecked = menuItem.IsChecked,
                KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription
            };
        }

        private MenuFlyoutItem CreateCommandMenuItem(CoreWebView2ContextMenuRequestedEventArgs args, CoreWebView2ContextMenuItem menuItem)
        {
            var newItem = new MenuFlyoutItem
            {
                Text = menuItem.Label.Split('(')[0],
                IsEnabled = menuItem.IsEnabled,
                KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription,
                Icon = new FontIcon { Glyph = menuItem.Name.ToGlyph() }
            };

            newItem.Click += (s, e) => args.SelectedCommandId = menuItem.CommandId;
            return newItem;
        }

        private async void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            if (!App.settings["AskDownloadBehavior"].GetValue<bool>()) return;

            args.Handled = true;
            var hwnd = this.GetWindowHandle();
            var file = await Utilities.Utilities.SaveFile(args.ResultFilePath, hwnd);

            args.ResultFilePath = file.Path;
            downloadButton.DownloadList.Add(new DownloadObject(args.DownloadOperation));

            if (App.settings["ShowFlyoutWhenStartDownloading"].GetValue<bool>())
            {
                downloadButton.ShowFlyout();
            }
        }

        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            var mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage { WebUri = args.Uri });
        }

        private void UriGoBackRequest(object sender, RoutedEventArgs e)
        {
            if (EdgeWebViewEngine.CanGoBack) EdgeWebViewEngine.GoBack();
            SetNavigationButtonStatus();
        }

        private void UriGoForwardRequest(object sender, RoutedEventArgs e)
        {
            if (EdgeWebViewEngine.CanGoForward) EdgeWebViewEngine.GoForward();
            SetNavigationButtonStatus();
        }

        private void UriRefreshRequest(object sender, RoutedEventArgs e) => EdgeWebViewEngine.Reload();

        public void ShowHomePage(object sender, RoutedEventArgs e)
        {
            var mainWindow = App.GetWindowForElement(this);
            mainWindow.AddHomePage();
        }

        public void ShowFlyout(string name)
        {
            if (name == "历史记录") historyButton.ShowFlyout();
            else if (name == "下载") downloadButton.ShowFlyout();
        }

        public CoreWebView2 CoreWebView2 => EdgeWebViewEngine.CoreWebView2;
    }
}
