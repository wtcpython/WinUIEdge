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
            MainWindow mainWindow = App.GetWindowForElement(this) as MainWindow;
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = (e.ClickedItem as WebsiteInfo).Uri
            });
        }
    }
}