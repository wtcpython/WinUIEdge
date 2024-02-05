using Edge.Data;
using Microsoft.UI.Xaml.Controls;

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
    }
}