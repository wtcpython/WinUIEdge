using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;


namespace Edge
{
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SUBCLASSPROC mainWindowSubClassProc;
        private OverlappedPresenter overlappedPresenter;

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get => _isWindowMaximized;

            set
            {
                if (_isWindowMaximized != value)
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            AppWindow.SetIcon("./Assets/icon.ico");
            SetTitleBar(AppTitleBar);

            this.SetBackdrop();
            this.SetThemeColor();

            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;

            IsWindowMaximized = overlappedPresenter.State == OverlappedPresenterState.Maximized;

            HWND hWND = (HWND)this.GetWindowHandle();

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            PInvoke.SetWindowSubclass(hWND, mainWindowSubClassProc, 0, 0);
        }

        public void AddHomePage()
        {
            string uri = App.settings.SpecificUri;
            if (!string.IsNullOrEmpty(uri))
            {
                AddNewTab(new WebViewPage() { WebUri = new Uri(uri) });
            }
            else
            {
                AddNewTab(new HomePage());
            }
        }

        public void AddNewTab(object content, string header = "主页", int index = -1)
        {
            TabViewItem newTab = new()
            {
                IconSource = new FontIconSource() { Glyph = "\ue80f" },
                Header = header,
                Content = content
            };
            if (content is WebViewPage web)
            {
                newTab.ContextFlyout = TabFlyout;
                web.tabViewItem = newTab;
            }

            int insertIndex = index >= 0 ? index : tabView.TabItems.Count;
            tabView.TabItems.Insert(insertIndex, newTab);
            tabView.SelectedIndex = insertIndex;
        }

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            // 窗口位置发生变化
            if (args.DidPositionChange)
            {
                if (TitlebarMenuFlyout.IsOpen)
                {
                    TitlebarMenuFlyout.Hide();
                }

                if (overlappedPresenter is not null)
                {
                    IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
                }
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Restore();
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                PInvoke.SendMessage((HWND)this.GetWindowHandle(), PInvoke.WM_SYSCOMMAND, 0xF010, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                PInvoke.SendMessage((HWND)this.GetWindowHandle(), PInvoke.WM_SYSCOMMAND, 0xF000, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Minimize();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Maximize();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            this.Close();
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private LRESULT MainWindowSubClassProc(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            switch (Msg)
            {
                // 选择窗口右键菜单的条目时接收到的消息
                case PInvoke.WM_SYSCOMMAND:
                    {
                        nuint sysCommand = wParam.Value & 0xFFF0;

                        if (sysCommand == PInvoke.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new Point(0, 15),
                                ShowMode = FlyoutShowMode.Standard,
                            };
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return (LRESULT)0;
                        }
                        else if (sysCommand == PInvoke.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new Point(0, 45),
                                ShowMode = FlyoutShowMode.Standard,
                            };
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return (LRESULT)0;
                        }
                        break;
                    }
                case PInvoke.WM_NCLBUTTONDOWN:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }

                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case PInvoke.WM_NCRBUTTONUP:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            System.Drawing.Point point = new((short)(lParam.Value & 0xFFFF), (short)((lParam.Value >> 16) & 0xFFFF));
                            PInvoke.ScreenToClient(hWnd, ref point);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = new Point(point.X, point.Y)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return (LRESULT)0;
                    }
            }
            return PInvoke.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        private void CreateNewTabOnRight(object sender, RoutedEventArgs e)
        {
            AddHomePage();
        }

        private void RefreshTab(object sender, RoutedEventArgs e)
        {
            WebView2 webView2 = App.GetWebView2(tabView.SelectedItem as TabViewItem);
            webView2.Reload();
        }

        private void CopyTab(object sender, RoutedEventArgs e)
        {
            WebView2 webView2 = App.GetWebView2(tabView.SelectedItem as TabViewItem);
            AddNewTab(new WebViewPage() { WebUri = webView2.Source }, index: tabView.SelectedIndex + 1);
        }

        private void MoveTabToNewWindow(object sender, RoutedEventArgs e)
        {
            WebView2 webView2 = App.GetWebView2(tabView.SelectedItem as TabViewItem);
            var window = App.CreateNewWindow();
            window.AddNewTab(new WebViewPage() { WebUri = webView2.Source });
            window.Activate();
            tabView.TabItems.Remove(tabView.SelectedItem);
        }

        private void CloseTab(object sender, object e)
        {
            if (e is TabViewTabCloseRequestedEventArgs args)
            {
                tabView.TabItems.Remove(args.Tab);
            }
            // else e is RoutedEventArgs
            else
            {
                tabView.TabItems.RemoveAt(tabView.SelectedIndex);
            }
            if (tabView.TabItems.Count <= 0)
            {
                Close();
            }
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
            WebView2 webView2 = App.GetWebView2(tabView.SelectedItem as TabViewItem);
            if (!webView2.CoreWebView2.IsMuted)
            {
                webView2.CoreWebView2.IsMuted = true;
                MuteButton.Icon = new FontIcon() { Glyph = "\ue995" };
                MuteButton.Text = "取消标签页静音";
            }
            else
            {
                webView2.CoreWebView2.IsMuted = false;
                MuteButton.Icon = new FontIcon() { Glyph = "\ue74f" };
                MuteButton.Text = "使标签页静音";
            }
        }

        public object SelectedItem
        {
            get => (tabView.SelectedItem as TabViewItem).Content;
            set => tabView.SelectedItem = value;
        }

        private void OpenClosedTab(object sender, RoutedEventArgs e)
        {
            AddNewTab(new WebViewPage() { WebUri = new Uri(App.Histories[^1].Source) });
        }

        private void PinTab(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            var item = tabView.SelectedItem as TabViewItem;
            if (item.IsClosable)
            {
                menuItem.Text = "取消固定标签页";
                item.IsClosable = false;
            }
            else
            {
                menuItem.Text = "固定标签页";
                item.IsClosable = true;
            }
        }

        private void AddNewButtonClick(SplitButton sender, SplitButtonClickEventArgs args)
        {
            AddHomePage();
        }

        private void OpenHomePage(object sender, RoutedEventArgs e)
        {
            AddHomePage();
        }

        private void OpenBingPage(object sender, RoutedEventArgs e)
        {
            AddNewTab(new WebViewPage() { WebUri = new Uri("https://www.bing.com/") });
        }

        private async void LearnAboutWorkspaces(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://aka.ms/edgeworkspaces/learn-more-msa"));
        }

        private void CreateNewWorkspaces(object sender, RoutedEventArgs e)
        {

        }
    }
}
