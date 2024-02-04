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
            startBox.SelectedIndex = Info.StartPageBehaviorList.IndexOf(Info.data.StartPageBehavior);

            uriCard.IsEnabled = startBox.SelectedIndex != 0;
            setHomeButton.IsOn = Info.data.ShowHomeButton;
            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name);
            searchEngineBox.SelectedItem = Info.SearchEngineList.Select(x => x.Name).First(name => name == Info.data.SearchEngine);
        }

        private void StartBahaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriCard.IsEnabled = startBox.SelectedIndex != 0;
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Info.data.SpecificUri = uriText.Text;
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Info.data.ShowHomeButton = setHomeButton.IsOn;
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            Info.data.SearchEngine = Info.SearchEngineList.Where(x => x.Name == (string)searchEngineBox.SelectedItem).First().Name;
        }
    }
}
