using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace Edge
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            View.ItemsSource = Info.SuggestWebsiteList;
        }

        private void OpenSuggestWebsite(object sender, ItemClickEventArgs e)
        {
            (App.Window as MainWindow).AddNewTab(new WebViewPage()
            {
                WebUri = (e.ClickedItem as WebsiteInfo).Uri
            });
        }

        private void SearchKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                (App.Window as MainWindow).AddNewTab(new WebViewPage()
                {
                    WebUri = Info.SearchEngineList.Where(x => x.Name == Info.data.SearchEngine).Select(x => x.Uri).First() + SearchBox.Text
                });
            }
        }
    }
}