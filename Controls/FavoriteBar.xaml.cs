using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace Edge
{
    public sealed partial class FavoriteBar : UserControl
    {
        public FavoriteBar()
        {
            this.InitializeComponent();
            FavoritesView.ItemsSource = App.settings.Favorites;
        }

        private void OpenFavoriteWebsite(object sender, ItemClickEventArgs e)
        {
            OpenWebSite((e.ClickedItem as WebsiteInfo).Uri);
        }

        private void OpenFavoriteWebsiteInNewTab(object sender, RoutedEventArgs e)
        {
            Uri uri = ((sender as MenuFlyoutItem).DataContext as WebsiteInfo).Uri;
            OpenWebSite(uri);
        }

        private void OpenWebSite(Uri uri)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = uri
            });
        }

        private void OpenFavoriteWebsiteInNewWindow(object sender, RoutedEventArgs e)
        {
            Uri uri = ((sender as MenuFlyoutItem).DataContext as WebsiteInfo).Uri;
            var window = App.CreateNewWindow();
            window.AddNewTab(new WebViewPage() { WebUri = uri });
            window.Activate();
        }

        private void CopyFavoriteWebsite(object sender, RoutedEventArgs e)
        {
            Uri uri = ((sender as MenuFlyoutItem).DataContext as WebsiteInfo).Uri;
            DataPackage package = new();
            package.SetText(uri.ToString());
            Clipboard.SetContent(package);
        }

        private void DeleteFavoriteWebsite(object sender, RoutedEventArgs e)
        {
            WebsiteInfo info = (sender as MenuFlyoutItem).DataContext as WebsiteInfo;
            App.settings.Favorites.Remove(info);
        }
    }
}
