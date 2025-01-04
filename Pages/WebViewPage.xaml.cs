using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public TabViewItem tabViewItem { get; set; }
        public WebViewPage()
        {
            InitializeComponent();
            InitializeToolbarVisibility();
            SetNavigationButtonStatus();
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
            sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
            sender.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            sender.CoreWebView2.NavigationStarting += (s, e) => Search.Text = e.Uri;
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening;
            sender.CoreWebView2.StatusBarTextChanged += (s, e) => uriPreview.Text = s.StatusBarText;
            sender.CoreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;

            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
        }

        private void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            App.Histories.Add(new WebViewHistory()
            {
                DocumentTitle = sender.DocumentTitle,
                Source = sender.Source,
                FaviconUri = sender.FaviconUri,
                Time = DateTime.Now.ToString()
            });
            SetNavigationButtonStatus();
        }

        private void CoreWebView2_FaviconChanged(CoreWebView2 sender, object args)
        {
            tabViewItem.IconSource = new ImageIconSource()
            {
                ImageSource = new BitmapImage(new Uri(sender.FaviconUri))
            };
        }

        private void CoreWebView2_DocumentTitleChanged(CoreWebView2 sender, object args)
        {
            string title = sender.DocumentTitle;
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.Title = title;
            tabViewItem.Header = title;
        }

        private void CoreWebView2_WindowCloseRequested(CoreWebView2 sender, object args)
        {
            TabView tabView = App.GetWindowForElement(this).Content as TabView;
            tabView.TabItems.Remove(tabViewItem);
        }

        private async void CoreWebView2_ScriptDialogOpening(CoreWebView2 sender, CoreWebView2ScriptDialogOpeningEventArgs args)
        {
            string title = $"{new Uri(args.Uri).Host} 显示";
            if (args.Kind == CoreWebView2ScriptDialogKind.Alert)
            {
                ContentDialog dialog = new()
                {
                    Title = title,
                    Content = args.Message,
                    XamlRoot = this.XamlRoot,
                    CloseButtonText = "确定",
                    DefaultButton = ContentDialogButton.Close
                };
                await dialog.ShowAsync();
            }
            else if (args.Kind == CoreWebView2ScriptDialogKind.Confirm)
            {
                ContentDialog dialog = new()
                {
                    Title = title,
                    Content = args.Message,
                    XamlRoot = this.XamlRoot,
                    PrimaryButtonText = "确定",
                    SecondaryButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary
                };
                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    args.Accept();
                }
            }
            else if (args.Kind == CoreWebView2ScriptDialogKind.Prompt)
            {
                TextBox box = new()
                {
                    Text = args.DefaultText,
                };
                ContentDialog dialog = new()
                {
                    Title = title,
                    Content = new StackPanel()
                    {
                        Children =
                        {
                            new TextBlock() { Text = args.Message },
                            box
                        }
                    },
                    XamlRoot = this.XamlRoot,
                    PrimaryButtonText = "确定",
                    SecondaryButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary
                };
                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    args.ResultText = box.Text;
                    args.Accept();
                }
            }
            else if (args.Kind == CoreWebView2ScriptDialogKind.Beforeunload)
            {
                ContentDialog dialog = new()
                {
                    Title = "是否离开网站？",
                    Content = "你所做的更改可能未保存。",
                    XamlRoot = this.XamlRoot,
                    PrimaryButtonText = "确定",
                    SecondaryButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary
                };
                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    args.Accept();
                }
            }
            else
            {
                throw new NotImplementedException();
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
                    CoreWebView2ContextMenuItemKind.Command => CreateCommandMenuItem(args, menuItem),
                    CoreWebView2ContextMenuItemKind.CheckBox => CreateToggleMenuItem(menuItem),
                    CoreWebView2ContextMenuItemKind.Radio => CreateRadioMenuItem(menuItem),
                    CoreWebView2ContextMenuItemKind.Separator => new MenuFlyoutSeparator(),
                    CoreWebView2ContextMenuItemKind.Submenu => CreateSubMenuItem(args, menuItem),
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

        private RadioMenuFlyoutItem CreateRadioMenuItem(CoreWebView2ContextMenuItem menuItem)
        {
            var item = new RadioMenuFlyoutItem()
            {
                Text = menuItem.Label.Split('(')[0],
                IsEnabled = menuItem.IsEnabled,
                IsChecked = menuItem.IsChecked,
                KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription
            };
            menuItem.CustomItemSelected += (s, e) => item.IsChecked = menuItem.IsChecked;
            return item;
        }

        private ToggleMenuFlyoutItem CreateToggleMenuItem(CoreWebView2ContextMenuItem menuItem)
        {
            var item = new ToggleMenuFlyoutItem
            {
                Text = menuItem.Label.Split('(')[0],
                IsEnabled = menuItem.IsEnabled,
                IsChecked = menuItem.IsChecked,
                KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription
            };
            menuItem.CustomItemSelected += (s, e) => item.IsChecked = menuItem.IsChecked;
            return item;
        }

        private MenuFlyoutItem CreateCommandMenuItem(CoreWebView2ContextMenuRequestedEventArgs args, CoreWebView2ContextMenuItem menuItem)
        {
            var item = new MenuFlyoutItem
            {
                Text = menuItem.Label.Split('(')[0],
                IsEnabled = menuItem.IsEnabled,
                KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription,
                Icon = new FontIcon { Glyph = menuItem.Name.ToGlyph() },
            };

            item.Click += (s, e) => args.SelectedCommandId = menuItem.CommandId;
            return item;
        }

        private void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            Deferral deferral = args.GetDeferral();

            System.Threading.SynchronizationContext.Current.Post(async (_) =>
            {
                using (deferral)
                {
                    args.Handled = true;
                    var hwnd = this.GetWindowHandle();
                    var file = await Utilities.Utilities.SaveFile(args.ResultFilePath, hwnd);
                    if (file != null)
                    {
                        args.ResultFilePath = file.Path;
                        downloadButton.DownloadList.Add(new DownloadObject(args.DownloadOperation));
                        if (App.settings["ShowFlyoutWhenStartDownloading"].GetValue<bool>())
                        {
                            downloadButton.ShowFlyout();
                        }
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }
            }, null);
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
        public WebView2 webView2 => EdgeWebViewEngine;
    }
}
