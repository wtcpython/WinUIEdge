using Edge.Data;
using Edge.Utilities;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;


namespace Edge
{
    public sealed partial class MainWindow : Window
    {
        public OverlappedPresenter overlappedPresenter;
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;

            AddNewTab(new HomePage());

            this.SetBackdrop();
            this.SetThemeColor();

            IntPtr hwnd = this.GetWindowHandle();
            int nStyle = PInvoke.GetWindowLong((HWND)hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            nStyle &= ~(int)0x00080000L;
            PInvoke.SetWindowLong((HWND)hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, nStyle);
        }

        public void AddNewTab(object content, string header = "主页", int index = -1)
        {
            TabViewItem newTab = new()
            {
                IconSource = new FontIconSource() { Glyph = "\ue80f" },
                Header = header,
                Content = content
            };
            if (content is WebViewPage) newTab.ContextFlyout = TabFlyout;
            if (index >= 0)
            {
                tabView.TabItems.Insert(index, newTab);
                tabView.SelectedIndex = index;
            }
            else
            {
                tabView.TabItems.Add(newTab);
                tabView.SelectedIndex = tabView.TabItems.Count - 1;
            }
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            AddNewTab(new HomePage());
        }

        private void CreateNewTabOnRight(object sender, RoutedEventArgs e)
        {
            AddNewTab(new HomePage(), index: tabView.SelectedIndex + 1);
        }

        private void RefreshTab(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
                page.Refresh();
        }

        private void CopyTab(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
                AddNewTab(new WebViewPage() { WebUri = page.WebUri }, index: tabView.SelectedIndex + 1);
        }

        private void MoveTabToNewWindow(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
            {
                var window = App.CreateNewWindow();
                window.AddNewTab(new WebViewPage() { WebUri = page.WebUri });
                window.Activate();
                tabView.TabItems.Remove(tabView.SelectedItem);
            }
        }

        private void CloseTab(object sender, object e)
        {
            if (e is TabViewTabCloseRequestedEventArgs args) tabView.TabItems.Remove(args.Tab);
            // else e is RoutedEventArgs
            else
            {
                TabViewItem item = tabView.SelectedItem as TabViewItem;
                tabView.TabItems.Remove(item);
                if (item.Content is WebViewPage)
                {
                    App.ClosedTabs.Add(item);
                }
            }
            if (!tabView.TabItems.Any()) Close();
        }

        private void CloseOtherTab(object sender, RoutedEventArgs e)
        {
            var selectedItem = tabView.SelectedItem;
            tabView.TabItems.Clear();
            tabView.TabItems.Add(selectedItem);
        }

        private void CloseRightTab(object sender, RoutedEventArgs e)
        {
            while (tabView.TabItems.Count > tabView.SelectedIndex + 1)
            {
                tabView.TabItems.RemoveAt(tabView.SelectedIndex + 1);
            }
            tabView.UpdateLayout();
        }

        private void MuteTab(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
            {
                if (!page.CoreWebView2.IsMuted)
                {
                    page.CoreWebView2.IsMuted = true;
                    MuteButton.Icon = new FontIcon() { Glyph = "\ue995" };
                    MuteButton.Text = "取消标签页静音";
                }
                else
                {
                    page.CoreWebView2.IsMuted = false;
                    MuteButton.Icon = new FontIcon() { Glyph = "\ue74f" };
                    MuteButton.Text = "使标签页静音";
                }
            }
        }

        public object SelectedItem
        {
            get => (tabView.SelectedItem as TabViewItem).Content;
            set => tabView.SelectedItem = value;
        }

        private void ShowMenuFlyout()
        {
            HWND hwnd = (HWND)this.GetWindowHandle();
            if (PInvoke.IsZoomed(hwnd))
            {
                MaximizeItem.IsEnabled = false;
                RestoreItem.IsEnabled = true;
            }
            else
            {
                MaximizeItem.IsEnabled = true;
                if (!PInvoke.IsIconic(hwnd))
                {
                    RestoreItem.IsEnabled = false;
                }
                else RestoreItem.IsEnabled = true;
            }
            AppMenuFlyout.ShowAt(AppTitleBarHeader, new FlyoutShowOptions()
            {
                Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft
            });
        }

        private void AppTitleBarHeader_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            ShowMenuFlyout();
        }

        private void ShowMenuFlyoutInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ShowMenuFlyout();
        }

        private void AppMinimize(object sender, RoutedEventArgs e)
        {
            overlappedPresenter.Minimize();
        }

        private void AppMaximize(object sender, RoutedEventArgs e)
        {
            overlappedPresenter.Maximize();
        }

        private void AppRestore(object sender, RoutedEventArgs e)
        {
            overlappedPresenter.Restore();
        }

        private void AppClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenClosedTabs(object sender, RoutedEventArgs e)
        {
            foreach (var item in App.ClosedTabs)
            {
                tabView.TabItems.Add(item);
            }
            App.ClosedTabs.Clear();
        }

        private void TabView_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "在新选项卡打开文件";
            }
        }

        private async void TabView_Drop(object sender, DragEventArgs e)
        {
            var items = await e.DataView.GetStorageItemsAsync();
            foreach (var item in items)
            {
                FileInfo fileInfo = new(item.Path);
                if (Info.LanguageDict.ContainsKey(fileInfo.Extension))
                {
                    AddNewTab(new TextFilePage(fileInfo.FullName), fileInfo.Name);
                }

                else if (Info.ImageDict.ContainsKey(fileInfo.Extension))
                {
                    AddNewTab(new ImageViewer(fileInfo.FullName), fileInfo.Name);
                }
            }
        }
    }
}
