using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;


namespace Edge
{
    public sealed partial class FavoriteList : UserControl
    {
        public ItemsPanelTemplate HorizontalTemplate => horizontalTemplate;
        public ItemsPanelTemplate VerticalTemplate => verticalTemplate;

        public FavoriteList()
        {
            this.InitializeComponent();
            SetItemsPanel(HorizontalTemplate);
            listView.ItemsSource = Info.Favorites;
        }

        public void SetItemsPanel(ItemsPanelTemplate itemsPanelTemplate)
        {
            listView.ItemsPanel = itemsPanelTemplate;
            if (itemsPanelTemplate == HorizontalTemplate)
            {
                scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            else
            {
                scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        public ItemsPanelTemplate ItemsPanel
        {
            get => listView.ItemsPanel;
            set => listView.ItemsPanel = value;
        }

        public ObservableCollection<WebsiteInfo> ItemsSource
        {
            get => (ObservableCollection<WebsiteInfo>)listView.ItemsSource;
            set => listView.ItemsSource = value;
        }

        private void OpenFavoriteWebsite(object sender, ItemClickEventArgs e)
        {
            OpenWebSite(((WebsiteInfo)e.ClickedItem).Uri);
        }

        private void OpenFavoriteWebsiteInNewTab(object sender, RoutedEventArgs e)
        {
            Uri uri = ((WebsiteInfo)((MenuFlyoutItem)sender).DataContext).Uri;
            OpenWebSite(uri);
        }

        private void OpenWebSite(Uri uri)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage(uri));
        }

        private void OpenFavoriteWebsiteInNewWindow(object sender, RoutedEventArgs e)
        {
            Uri uri = ((WebsiteInfo)((MenuFlyoutItem)sender).DataContext).Uri;
            var window = App.CreateNewWindow();
            window.AddNewTab(new WebViewPage(uri));
            window.Activate();
        }

        private void CopyFavoriteWebsite(object sender, RoutedEventArgs e)
        {
            Uri uri = ((WebsiteInfo)((MenuFlyoutItem)sender).DataContext).Uri;
            DataPackage package = new();
            package.SetText(uri.ToString());
            Clipboard.SetContent(package);
        }

        private void DeleteFavoriteWebsite(object sender, RoutedEventArgs e)
        {
            WebsiteInfo info = (WebsiteInfo)((MenuFlyoutItem)sender).DataContext;
            Info.Favorites.Remove(info);
        }
    }
}
