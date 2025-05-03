using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Foundation;

namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public TabViewItem tabViewItem { get; set; }
        public ProgressRing HeaderProgressRing { get; set; }
        public TextBlock HeaderTextBlock { get; set; }
        private bool InFavoriteList = false;
        private bool NavigationCompleted  = false;
        private ulong NowNavigationId = 0;
        private static readonly FontIconSource DefaultIconSource = new () { Glyph = "\ue774" };
        private static string InjectExtensionsStoreScript = null;

        private readonly ConcurrentDictionary<string, ImageIconSource> CachedIconSource = new ();

        public WebViewPage(Uri WebUri)
        {
            InitializeComponent();
            InitializeToolbarVisibility();
            Loaded += (sender, args) => InitializeWebView2Async(WebViewEngine, WebUri);

            favoriteList.Visibility = App.settings.MenuStatus == "Always" ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Close()
        {
            WebView2.Close();
        }

        private void InitializeToolbarVisibility()
        {
            homeButton.Visibility = App.settings.ToolBar!["HomeButton"] ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void InitializeWebView2Async(WebView2 webview2, Uri WebUri)
        {
            InjectExtensionsStoreScript ??= await File.ReadAllTextAsync("./Data/InjectExtensionsStoreScript.js", Encoding.UTF8);
            if (webview2.CoreWebView2 == null)
            {
                await webview2.EnsureCoreWebView2Async(App.CoreWebView2Environment);
                CoreWebView2 coreWebView2 = webview2.CoreWebView2;
                coreWebView2!.ContextMenuRequested += (s, args) => CoreWebView2_ContextMenuRequested(webview2, s, args);
                coreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
                coreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
                coreWebView2.SourceChanged += CoreWebView2_SourceChanged;
                coreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
                coreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                coreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
                coreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
                coreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                coreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening;
                coreWebView2.StatusBarTextChanged += (s, e) => uriPreview.Text = s.StatusBarText;
                coreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;
                coreWebView2.Settings.IsStatusBarEnabled = false;
                coreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            }
            if (WebUri != null && WebUri != webview2.Source)
            {
                webview2.Source = WebUri;
            }
            webview2.Visibility = Visibility.Visible;
        }

        private void CoreWebView2_NavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            NavigationCompleted = false;
            NowNavigationId = args.NavigationId;
            Search.Text = args.Uri;
            HeaderProgressRing.Visibility = Visibility.Visible;
            tabViewItem.IconSource = null;
        }

        private void CoreWebView2_SourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
        {
            if (Info.Favorites.Where(x => x.Uri.Equals(sender.Source)).Any())
            {
                InFavoriteList = true;
            }
            else
            {
                InFavoriteList = false;
            }
            SetFavoriteIcon();
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            if (App.settings.InjectExtensionsStore && InjectExtensionsStoreScript != null && sender.Source.StartsWith("https://microsoftedge.microsoft.com/addons/"))
            {
                sender.ExecuteScriptAsync(InjectExtensionsStoreScript);
            }
        }

        private void CoreWebView2_NavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            NavigationCompleted = true;
            App.Histories.Add(new WebViewHistory()
            {
                DocumentTitle = sender.DocumentTitle,
                Source = sender.Source,
                FaviconUri = sender.FaviconUri.Length > 0 ? new Uri(sender.FaviconUri) : null,
                Time = DateTime.Now.ToString(),
                NavigationId = args.NavigationId
            });
            if (sender.FaviconUri.Length > 0) {
                if (!CachedIconSource.TryGetValue(sender.FaviconUri, out ImageIconSource iconSource))
                {
                    iconSource = new ImageIconSource()
                    {
                        ImageSource = new BitmapImage(new Uri(sender.FaviconUri))
                    };
                    CachedIconSource.TryAdd(sender.FaviconUri, iconSource);
                }
                tabViewItem.IconSource = iconSource;
            }
            else
            {
                tabViewItem.IconSource = DefaultIconSource;
            }
            HeaderProgressRing.Visibility = Visibility.Collapsed;
        }

        private void CoreWebView2_FaviconChanged(CoreWebView2 sender, object args)
        {
            if (!NavigationCompleted || sender.FaviconUri.Length == 0)
            {
                return;
            }
            Uri faviconUri = new Uri(sender.FaviconUri);
            if (!CachedIconSource.TryGetValue(sender.FaviconUri, out ImageIconSource iconSource))
            {
                iconSource = new ImageIconSource()
                {
                    ImageSource = new BitmapImage(faviconUri)
                };
                CachedIconSource.TryAdd(sender.FaviconUri, iconSource);
            }
            tabViewItem.IconSource = iconSource;
            WebViewHistory history = App.Histories.FirstOrDefault(x => x.NavigationId == NowNavigationId, null);
            if (history != null)
            {
                history.FaviconUri = faviconUri;
            }
            WebsiteInfo favorite = Info.Favorites.FirstOrDefault(x => !x.CustomIcon && x.Uri.Equals(faviconUri), null);
            if (favorite != null)
            {
                favorite.Icon = faviconUri;
            }
        }

        private void CoreWebView2_DocumentTitleChanged(CoreWebView2 sender, object args)
        {
            string title = sender.DocumentTitle;
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.Title = title;
            HeaderTextBlock.Text = title;
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

        private void CoreWebView2_ContextMenuRequested(UIElement element, CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
        {
            args.Handled = true;
            var deferral = args.GetDeferral();
            var menuFlyout = new MenuFlyout();

            CreateMenuItems(args, args.MenuItems, menuFlyout.Items);

            menuFlyout.Closed += (s, e) => deferral.Complete();
            menuFlyout.ShowAt(element, args.Location);
        }

        private void CreateMenuItems(CoreWebView2ContextMenuRequestedEventArgs args, IList<CoreWebView2ContextMenuItem> webMenuItems, IList<MenuFlyoutItemBase> menuItems)
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

            CreateMenuItems(args, menuItem.Children, subItem.Items);
            return subItem;
        }

        private static RadioMenuFlyoutItem CreateRadioMenuItem(CoreWebView2ContextMenuItem menuItem)
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

        private static ToggleMenuFlyoutItem CreateToggleMenuItem(CoreWebView2ContextMenuItem menuItem)
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

        private static MenuFlyoutItem CreateCommandMenuItem(CoreWebView2ContextMenuRequestedEventArgs args, CoreWebView2ContextMenuItem menuItem)
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

            System.Threading.SynchronizationContext.Current.Post((_) =>
            {
                using (deferral)
                {
                    args.Handled = true;
                    string file = Utilities.Win32SaveFile(args.ResultFilePath, this.GetWindowHandle());
                    if (file != null)
                    {
                        args.ResultFilePath = file;
                        App.DownloadList.Add(new DownloadObject(args.DownloadOperation));
                        if (App.settings.ShowFlyoutWhenStartDownloading)
                        {
                            ShowFlyout("下载");
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
            mainWindow.AddNewTab(new WebViewPage(new Uri(args.Uri)));
        }

        private void UriGoBackRequest(object sender, RoutedEventArgs e)
        {
            if (WebViewEngine.CanGoBack) WebViewEngine.GoBack();
        }

        private void UriGoForwardRequest(object sender, RoutedEventArgs e)
        {
            if (WebViewEngine.CanGoForward) WebViewEngine.GoForward();
        }

        private void UriRefreshRequest(object sender, RoutedEventArgs e) => WebViewEngine.Reload();

        public void ShowHomePage(object sender, RoutedEventArgs e)
        {
            var mainWindow = App.GetWindowForElement(this);
            mainWindow.AddHomePage();
        }

        public void ShowFlyout(string name)
        {
            toolBar.ShowFlyout(name);
        }

        public WebView2 WebView2 => WebViewEngine;

        private void FavoriteStateChanged(object sender, RoutedEventArgs e)
        {
            WebsiteInfo info = Info.Favorites.FirstOrDefault(x => x.Uri.Equals(WebViewEngine.Source));
            if (info != null)
            {
                Info.Favorites.Remove(info);
                InFavoriteList = false;
            }
            else
            {
                WebsiteInfo newInfo = new()
                {
                    Name = WebViewEngine.CoreWebView2.DocumentTitle,
                    Icon = WebViewEngine.CoreWebView2.FaviconUri.Length > 0 ? new Uri(WebViewEngine.CoreWebView2.FaviconUri) : null,
                    Uri = WebViewEngine.Source
                };
                Info.Favorites.Add(newInfo);
                InFavoriteList = true;
            }
            SetFavoriteIcon();
        }

        private void SetFavoriteIcon()
        {
            if (InFavoriteList)
            {
                favoriteButton.IconGlyph = "\ue735";
            }
            else
            {
                favoriteButton.IconGlyph = "\ue734";
            }
        }

        public void CreateSplitWindow()
        {
            leftColumn.Width = new GridLength(1, GridUnitType.Star);
            if (RightWebView.Visibility == Visibility.Collapsed)
            {
                rightColumn.Width = new GridLength(1, GridUnitType.Star);
                InitializeWebView2Async(RightWebView, WebViewEngine.Source);
            }
            else
            {
                rightColumn.Width = new GridLength(0);
                RightWebView.Visibility = Visibility.Collapsed;
            }
        }

        private void WebSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string text = args.QueryText;
            sender.Text = string.Empty;
            MainWindow mainWindow = App.GetWindowForElement(this);
            Utilities.Search(text, mainWindow);
        }
    }
}
