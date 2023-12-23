using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class StartItem : Page
    {
        public StartItem()
        {
            this.InitializeComponent();
            startBox.ItemsSource = JsonDataList.StartPageBehaviorList;
            startBox.SelectedIndex = JsonDataList.StartPageBehaviorList.IndexOf(Utils.data.StartPageBehavior);
            if (startBox.SelectedIndex == -1 ) { startBox.SelectedIndex = 0; };
            uriCard.IsEnabled = startBox.SelectedIndex != 0;
            setHomeButton.IsOn = Utils.data.ShowHomeButton;
            searchEngineBox.ItemsSource = JsonDataList.SearchEngineNameList;
            searchEngineBox.SelectedIndex = JsonDataList.StartPageBehaviorList.IndexOf(Utils.data.SearchEngine);
            if (searchEngineBox.SelectedIndex == -1) { searchEngineBox.SelectedIndex = 0; };
        }

        private void StartBahaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriCard.IsEnabled = startBox.SelectedIndex != 0;
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Utils.data.SpecificUri = uriText.Text;
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            foreach (TabViewItem tabViewItem in ((App.Window as MainWindow).GetItems()))
            {
                if ((tabViewItem.Content as Frame).Content is WebViewPage page)
                {
                    Utils.data.ShowHomeButton = setHomeButton.IsOn;
                    page.SetHomeButton();
                }
            }
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
