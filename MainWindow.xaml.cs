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
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            AddNewTab(new WebViewPage()
            {
                WebUri = "https://bing.com"
            });

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
            }
            else
            {
                tabView.TabItems.Add(newTab);
            }
        }

        public List<object> TabItems => tabView.TabItems.ToList();

        public WebViewPage SelectedItem => (tabView.SelectedItem as TabViewItem).Content as WebViewPage;

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            AddNewTab(new WebViewPage());
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            tabView.TabItems.Remove(args.Tab);
        }

        private void CreateNewTabOnRight(object sender, RoutedEventArgs e)
        {
            AddNewTab(new WebViewPage(), tabView.SelectedIndex + 1);
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
            SelectedItem.Refresh();
        }

        private void CopyTab(object sender, RoutedEventArgs e)
        {
            AddNewTab(new WebViewPage() { WebUri = SelectedItem.WebUri }, tabView.SelectedIndex + 1);
        }

        private void MoveTabToNewWindow(object sender, RoutedEventArgs e)
        {
            Utils.CreateNewWindow(new WebViewPage() { WebUri = SelectedItem.WebUri });
            tabView.TabItems.Remove(tabView.SelectedItem);
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
    }
}
