using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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

            AddNewTab(new HomePage());

            this.SystemBackdrop = Theme.SetBackDrop();
        }

        public void AddNewTab(object content, string header = "主页", int index = -1)
        {
            TabViewItem newTab = new()
            {
                IconSource = new FontIconSource() { Glyph = "\ue80f" },
                Header = header,
                Content = content,
                ContextFlyout = TabFlyout
            };
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
            else tabView.TabItems.Remove(tabView.SelectedItem);
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
        
    }
}
