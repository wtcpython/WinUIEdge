using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Edge
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static bool isMute = false;
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            AddNewTab(new HomePage(), header: "Home");

            this.SystemBackdrop = new MicaBackdrop();
        }

        private TabViewItem CreateNewTab(object content, string header)
        {
            TabViewItem tabViewItem = new()
            {
                IconSource = new SymbolIconSource() { Symbol = Symbol.Document },
                Header = header,
                Content = content,
                ContextFlyout = TabFlyout
            };
            return tabViewItem;
        }

        public void AddNewTab(object content, int index = -1, string header = "New Tab")
        {
            TabViewItem newTab = CreateNewTab(content, header);
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

        public List<object> TabItems => tabView.TabItems.ToList();

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            AddNewTab(new HomePage(), header: "Home");
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            tabView.TabItems.Remove(args.Tab);
        }

        private void CreateNewTabOnRight(object sender, RoutedEventArgs e)
        {
            AddNewTab(new HomePage(), tabView.SelectedIndex + 1, "Home");
        }

        private void TabView_ContextRequested(UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
        {
            if (sender is TabViewItem item)
            {
                TabFlyout.ShowAt(item, new FlyoutShowOptions()
                {
                    ShowMode = FlyoutShowMode.Standard
                });
            }
        }

        private void RefreshTab(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
                page.Refresh();
        }

        private void CopyTab(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
                AddNewTab(new WebViewPage() { WebUri = page.WebUri }, tabView.SelectedIndex + 1);
        }

        private void MoveTabToNewWindow(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
            {
                // App.CreateNewWindow(new WebViewPage() { WebUri = page.WebUri });
                var window = App.CreateNewWindow();
                window.AddNewTab(new WebViewPage() { WebUri = page.WebUri });
                window.Activate();
                tabView.TabItems.Remove(tabView.SelectedItem);
            }
        }

        private void CloseTab(object sender, RoutedEventArgs e)
        {
            tabView.TabItems.Remove(tabView.SelectedItem);
            if (tabView.TabItems.Count == 0)
            {
                Close();
            }
        }

        private void CloseOtherTab(object sender, RoutedEventArgs e)
        {
            while (tabView.SelectedIndex > 0)
            {
                tabView.TabItems.RemoveAt(0);
            }

            while (tabView.TabItems.Count > tabView.SelectedIndex + 1)
            {
                tabView.TabItems.RemoveAt(tabView.SelectedIndex + 1);
            }
            tabView.UpdateLayout();
        }

        private void CloseRightTab(object sender, RoutedEventArgs e)
        {
            while (tabView.TabItems.Count > tabView.SelectedIndex + 1)
            {
                tabView.TabItems.RemoveAt(tabView.SelectedIndex + 1);
            }
            tabView.UpdateLayout();
        }

        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabView.SelectedItem != null)
            {
                if ((tabView.SelectedItem as TabViewItem).Content is WebViewPage page)
                {
                    page.SetHomeButton();
                }
            }
        }

        private void MuteTab(object sender, RoutedEventArgs e)
        {
            if (SelectedItem is WebViewPage page)
            {
                if (!isMute)
                {
                    isMute = true;
                    page.Mute("true");
                    MuteButton.Icon = new SymbolIcon() { Symbol = Symbol.Volume };
                    MuteButton.Text = "È¡Ïû±êÇ©Ò³¾²Òô";
                }
                else
                {
                    isMute = false;
                    page.Mute("false");
                    MuteButton.Icon = new SymbolIcon() { Symbol = Symbol.Mute };
                    MuteButton.Text = "Ê¹±êÇ©Ò³¾²Òô";
                }
            }
        }

        public object SelectedItem
        {
            get => (tabView.SelectedItem as TabViewItem).Content;
            set => tabView.SelectedItem = value;
        }
        
    }
}
