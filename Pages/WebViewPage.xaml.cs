using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;


namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public string WebUri
        {
            get => EdgeWebViewEngine.CoreWebView2.Source;
            set => EdgeWebViewEngine.Source = new Uri(value);
        }

        public WebViewPage()
        {
            this.InitializeComponent();
            SetWebNaviButtonStatus();

            homeButton.Visibility = App.settings["ToolBar"].GetProperty("HomeButton").GetBoolean() ? Visibility.Visible : Visibility.Collapsed;
            historyButton.Visibility = App.settings["ToolBar"].GetProperty("HistoryButton").GetBoolean() ? Visibility.Visible : Visibility.Collapsed;
            downloadButton.Visibility = App.settings["ToolBar"].GetProperty("DownloadButton").GetBoolean() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetWebNaviButtonStatus()
        {
            uriGoBackButton.IsEnabled = EdgeWebViewEngine.CanGoBack;
            if (App.settings["ToolBar"].GetProperty("ForwardButton").GetBoolean())
            {
                uriGoForwardButton.Visibility = EdgeWebViewEngine.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UriGoBackRequest(object sender, RoutedEventArgs e)
        {
            if (EdgeWebViewEngine.CanGoBack)
            {
                EdgeWebViewEngine.GoBack();
            }
            SetWebNaviButtonStatus();
        }

        private void UriGoForwardRequest(object sender, RoutedEventArgs e)
        {
            if (EdgeWebViewEngine.CanGoForward)
            {
                EdgeWebViewEngine.GoForward();
            }
            SetWebNaviButtonStatus();
        }

        public void Refresh()
        {
            EdgeWebViewEngine.Reload();
        }

        private void UriRefreshRequest(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void EdgeWebViewEngine_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            // 事件绑定
            sender.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            sender.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.StatusBarTextChanged += CoreWebView2_StatusBarTextChanged;
            sender.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;

            // 加载设置项
            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        private void CoreWebView2_ContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            args.Handled = true;
            var menuItems = args.MenuItems;

            Deferral deferral = args.GetDeferral();

            MenuFlyout flyout = new();
            flyout.Closed += (sender, e) => deferral.Complete();

            SetMenuItems(args, menuItems, flyout.Items);
            flyout.ShowAt(EdgeWebViewEngine, args.Location);
        }

        private void SetMenuItems(
            CoreWebView2ContextMenuRequestedEventArgs args,
            IList<CoreWebView2ContextMenuItem> webMenuItems,
            IList<MenuFlyoutItemBase> menuItems)
        {
            foreach (var menuItem in webMenuItems)
            {
                switch (menuItem.Kind)
                {
                    case CoreWebView2ContextMenuItemKind.Separator:
                        menuItems.Add(new MenuFlyoutSeparator());
                        break;
                    case CoreWebView2ContextMenuItemKind.Submenu:
                        MenuFlyoutSubItem subItem = new()
                        {
                            Text = menuItem.Label[..menuItem.Label.IndexOf('(')],
                            IsEnabled = menuItem.IsEnabled,
                        };

                        SetMenuItems(args, menuItem.Children, subItem.Items);
                        menuItems.Add(subItem);
                        break;
                    case CoreWebView2ContextMenuItemKind.CheckBox:
                        menuItems.Add(new ToggleMenuFlyoutItem()
                        {
                            Text = menuItem.Label[..menuItem.Label.IndexOf('(')],
                            IsEnabled = menuItem.IsEnabled,
                            KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription,
                            IsChecked = menuItem.IsChecked
                        });
                        break;
                    case CoreWebView2ContextMenuItemKind.Command:
                        MenuFlyoutItem newItem = new()
                        {
                            Text = menuItem.Label[..menuItem.Label.IndexOf('(')],
                            IsEnabled = menuItem.IsEnabled,
                            KeyboardAcceleratorTextOverride = menuItem.ShortcutKeyDescription,
                            Icon = new FontIcon() { Glyph = menuItem.Name.ToGlyph() }
                        };

                        newItem.Click += (sender, e) =>
                        {
                            args.SelectedCommandId = menuItem.CommandId;
                        };
                        menuItems.Add(newItem);
                        break;
                    default:
                        break;
                }
            }
        }

        private void CoreWebView2_StatusBarTextChanged(CoreWebView2 sender, object args)
        {
            uriPreview.Text = sender.StatusBarText;
        }

        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;

            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage() { WebUri = args.Uri });
        }

        private async void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            args.Handled = true;
            if (App.settings["AskDownloadBehavior"].GetBoolean())
            {
                IntPtr hwnd = this.GetWindowHandle();

                StorageFile file = await Utilities.Utilities.SaveFile(args.ResultFilePath, hwnd);

                args.ResultFilePath = file.Path;
            }
            downloadButton.DownloadList.Add(new DownloadObject(args.DownloadOperation));
            if (App.settings["ShowFlyoutWhenStartDownloading"].GetBoolean())
            {
                downloadButton.ShowFlyout();
            }
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            SetWebNaviButtonStatus();
            MainWindow mainWindow = App.GetWindowForElement(this);

            mainWindow.Title = sender.DocumentTitle;
            App.Histories.Add(new WebViewHistory()
            {
                DocumentTitle = sender.DocumentTitle,
                Source = sender.Source,
                FaviconUri = sender.FaviconUri,
                Time = DateTime.Now.ToString()
            });

            TabViewItem item = (mainWindow.Content as TabView).TabItems
                .First(x => ((x as TabViewItem).Content as Page) == this) as TabViewItem;
            if (item != null)
            {
                item.Header = sender.DocumentTitle;
                string iconUri = sender.FaviconUri;
                if (iconUri != string.Empty)
                {
                    item.IconSource = new ImageIconSource()
                    {
                        ImageSource = new BitmapImage()
                        {
                            UriSource = new Uri(iconUri)
                        }
                    };
                }
            }
        }

        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            Search.Text = args.Uri;
        }

        private void ShowHomePage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new HomePage());
        }

        public void ShowPrintUI()
        {
            EdgeWebViewEngine.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        public CoreWebView2 CoreWebView2 => EdgeWebViewEngine.CoreWebView2;

        public void ShowFlyout(string name)
        {
            if (name == "历史记录")
            {
                historyButton.ShowFlyout();
            }
            else if (name == "下载")
            {
                downloadButton.ShowFlyout();
            }
        }
    }
}
