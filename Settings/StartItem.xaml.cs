using Edge.Data;
using Edge.Utilities;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Edge
{
    public sealed partial class StartItem : Page
    {
        public StartItem()
        {
            this.InitializeComponent();
            radios.SelectedIndex = App.settings["StartBehavior"].ToObject<int>();

            uriButton.IsEnabled = radios.SelectedIndex == 2;

            setHomeButton.IsOn = App.settings["ToolBar"]["HomeButton"].ToObject<bool>();
            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name);
            searchEngineBox.SelectedItem = Info.SearchEngineList.Select(x => x.Name).First(name => name == App.settings["SearchEngine"].ToString());

            showSuggestUri.IsOn = App.settings["ShowSuggestUri"].ToObject<bool>();

            showBackground.IsOn = backgroundCard.IsEnabled = App.settings["ShowBackground"].ToObject<bool>();
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["SpecificUri"] = uriBox.Text;
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ToolBar"]["HomeButton"] = setHomeButton.IsOn;
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

        private void ShowBackgroundChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ShowBackground"] = backgroundCard.IsEnabled = showBackground.IsOn;
        }

        private async void SetBackgroundImage(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter =
                {
                    {".jpg" },
                    {".jpeg"},
                    {".png" }
                }
            };

            InitializeWithWindow.Initialize(picker, this.GetWindowHandle()); 

            StorageFile storageFile = await picker.PickSingleFileAsync();
            App.settings["BackgroundImage"] = storageFile.Path;
        }
    }
}
