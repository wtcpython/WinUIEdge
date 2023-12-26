using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public sealed partial class StartItem : Page
    {
        public StartItem()
        {
            this.InitializeComponent();
            startBox.ItemsSource = JsonDataList.StartPageBehaviorList;
            startBox.SelectedIndex = JsonDataList.StartPageBehaviorList.IndexOf(Utils.data.StartPageBehavior);

            uriCard.IsEnabled = startBox.SelectedIndex != 0;
            setHomeButton.IsOn = Utils.data.ShowHomeButton;
            searchEngineBox.ItemsSource = JsonDataList.SearchEngineDictionary;
            searchEngineBox.SelectedIndex = JsonDataList.SearchEngineDictionary.Keys.ToList().IndexOf(Utils.data.SearchEngine);
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
            Utils.data.ShowHomeButton = setHomeButton.IsOn;
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            Utils.data.SearchEngine = (searchEngineBox.SelectedItem as KeyValuePair<string, string>?).Value.Key;
        }
    }
}
