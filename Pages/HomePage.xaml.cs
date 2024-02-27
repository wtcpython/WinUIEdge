using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            if (App.settings["ShowSuggestUri"].ToObject<bool>())
            {
                View.ItemsSource = Info.SuggestWebsiteList;
            }
            else
            {
                View.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenSuggestWebsite(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = (e.ClickedItem as WebsiteInfo).Uri
            });
        }
    }
}