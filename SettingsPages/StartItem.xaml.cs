using Edge.Data;
using Edge.Utilities;
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
            startBox.ItemsSource = Info.StartPageBehaviorList;
            startBox.SelectedIndex = Info.StartPageBehaviorList.IndexOf(Info.data.StartPageBehavior);

            uriCard.IsEnabled = startBox.SelectedIndex != 0;
            setHomeButton.IsOn = Info.data.ShowHomeButton;
            searchEngineBox.ItemsSource = Info.SearchEngineDict;
            searchEngineBox.SelectedIndex = Info.SearchEngineDict.Keys.ToList().IndexOf(Info.data.SearchEngine);
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
            Info.data.SearchEngine = (searchEngineBox.SelectedItem as KeyValuePair<string, string>?).Value.Key;
        }
    }
}
