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
            startBox.ItemsSource = Info.StartPageBehaviorList;
            startBox.SelectedIndex = Info.StartPageBehaviorList.IndexOf(App.settings["StartPageBehavior"].ToString());

            uriCard.IsEnabled = startBox.SelectedIndex != 0;
            setHomeButton.IsOn = App.settings["ShowHomeButton"].ToObject<bool>();
            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name);
            searchEngineBox.SelectedItem = Info.SearchEngineList.Select(x => x.Name).First(name => name == App.settings["SearchEngine"].ToString());
        }

        private void StartBahaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriCard.IsEnabled = startBox.SelectedIndex != 0;
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["SpecificUri"] = uriText.Text;
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ShowHomeButton"] = setHomeButton.IsOn;
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            App.settings["SearchEngine"] = Info.SearchEngineList.Where(x => x.Name == (string)searchEngineBox.SelectedItem).First().Name;
        }
    }
}
