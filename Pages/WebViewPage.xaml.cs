using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace Edge
{
    public sealed partial class WebViewPage : Page
    {
        public TabViewItem tabViewItem { get; set; }
        private bool InFavoriteList = false;

        public WebViewPage(Uri WebUri)
        {
            InitializeComponent();
            InitializeToolbarVisibility();
            WebViewEngine.Source = WebUri;
        }

        private void InitializeToolbarVisibility()
        {
            homeButton.Visibility = App.settings.ToolBar!["HomeButton"] ? Visibility.Visible : Visibility.Collapsed;
        }

        private void WebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.ContextMenuRequested += (s, args) => CoreWebView2_ContextMenuRequested(sender, s, args);
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            sender.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
            sender.CoreWebView2.NavigationStarting += (s, e) => Search.Text = e.Uri;
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.ScriptDialogOpening += CoreWebView2_ScriptDialogOpening;
            sender.CoreWebView2.StatusBarTextChanged += (s, e) => uriPreview.Text = s.StatusBarText;
            sender.CoreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;

            sender.CoreWebView2.Settings.IsStatusBarEnabled = false;
            sender.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
        }

        private void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {
            App.Histories.Add(new WebViewHistory()
            {
                DocumentTitle = sender.DocumentTitle,
                Source = sender.Source,
                FaviconUri = new Uri(sender.FaviconUri),
                Time = DateTime.Now.ToString()
            });

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
                    Icon = WebViewEngine.CoreWebView2.FaviconUri,
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
                RightWebView.Source = new("https://www.bing.com/");
                RightWebView.Visibility = Visibility.Visible;
            }
            else
            {
                rightColumn.Width = new GridLength(0);
                RightWebView.Visibility = Visibility.Collapsed;
            }
        }
    }
}
