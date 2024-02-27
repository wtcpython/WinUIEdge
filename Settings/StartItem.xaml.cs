using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace Edge
{
    public sealed partial class StartItem : Page
    {
        public StartItem()
        {
            this.InitializeComponent();
            radios.SelectedIndex = App.settings["StartBehavior"].ToObject<int>();

            uriButton.IsEnabled = radios.SelectedIndex == 2;

            setHomeButton.IsOn = App.settings["ShowHomeButton"].ToObject<bool>();
            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name);
            searchEngineBox.SelectedItem = Info.SearchEngineList.Select(x => x.Name).First(name => name == App.settings["SearchEngine"].ToString());

            showSuggestUri.IsOn = App.settings["ShowSuggestUri"].ToObject<bool>();
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["SpecificUri"] = uriBox.Text;
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ShowHomeButton"] = setHomeButton.IsOn;
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            App.settings["SearchEngine"] = Info.SearchEngineList.Where(x => x.Name == (string)searchEngineBox.SelectedItem).First().Name;
        }

        private void BehaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriButton.IsEnabled = radios.SelectedIndex == 2;
        }

        private void SuggestUriVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ShowSuggestUri"] = showSuggestUri.IsOn;
        }
    }
}
