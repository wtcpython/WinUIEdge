using Edge.Data;
using Edge.Utilities;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Edge
{
    public sealed partial class StartItem : Page
    {
        public Dictionary<string, bool> ToolBar = JsonSerializer.Deserialize<Dictionary<string, bool>>(App.settings["ToolBar"].ToString());
        public StartItem()
        {
            this.InitializeComponent();
            radios.SelectedIndex = App.settings["StartBehavior"].GetInt32();

            uriButton.IsEnabled = radios.SelectedIndex == 2;

            setHomeButton.IsOn = ToolBar["HomeButton"];
            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name);
            searchEngineBox.SelectedItem = Info.SearchEngineList.Select(x => x.Name).First(name => name == App.settings["SearchEngine"].ToString());

            showSuggestUri.IsOn = App.settings["ShowSuggestUri"].GetBoolean();

            showBackground.IsOn = backgroundCard.IsEnabled = App.settings["ShowBackground"].GetBoolean();
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["SpecificUri"] = App.ToJsonElement(uriBox.Text);
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ToolBar["HomeButton"] = setHomeButton.IsOn;
            App.settings["ToolBar"] = App.ToJsonElement(ToolBar);
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            App.settings["SearchEngine"] = App.ToJsonElement(Info.SearchEngineList.Where(x => x.Name == (string)searchEngineBox.SelectedItem).First().Name);
        }

        private void BehaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriButton.IsEnabled = radios.SelectedIndex == 2;
        }

        private void SuggestUriVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ShowSuggestUri"] = App.ToJsonElement(showSuggestUri.IsOn);
        }

        private void ShowBackgroundChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            backgroundCard.IsEnabled = showBackground.IsOn;
            App.settings["ShowBackground"] = App.ToJsonElement(showBackground.IsOn);
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
            App.settings["BackgroundImage"] = App.ToJsonElement(storageFile.Path);
        }
    }
}
