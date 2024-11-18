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
        public Dictionary<string, bool> ToolBar = JsonSerializer.Deserialize<Dictionary<string, bool>>(App.settings["ToolBar"].ToJsonString());
        public StartItem()
        {
            this.InitializeComponent();
            radios.SelectedIndex = App.settings["StartBehavior"].GetValue<int>();

            uriBox.Text = App.settings["SpecificUri"].ToString();
            uriBox.IsEnabled = radios.SelectedIndex == 2;

            setHomeButton.IsOn = ToolBar["HomeButton"];
            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name);
            searchEngineBox.SelectedItem = Info.SearchEngineList.Select(x => x.Name).First(name => name == App.settings["SearchEngine"].ToString());

            showSuggestUri.IsOn = App.settings["ShowSuggestUri"].GetValue<bool>();

            showBackground.IsOn = backgroundCard.IsEnabled = App.settings["ShowBackground"].GetValue<bool>();
        }

        private void SetStartUri(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            App.settings["SpecificUri"] = uriBox.Text;
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ToolBar["HomeButton"] = setHomeButton.IsOn;
            App.settings["ToolBar"] = JsonSerializer.SerializeToNode(ToolBar);
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            App.settings["SearchEngine"] = Info.SearchEngineList.Where(x => x.Name == (string)searchEngineBox.SelectedItem).First().Name;
        }

        private void BehaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriBox.IsEnabled = radios.SelectedIndex == 2;
            App.settings["StartBehavior"] = radios.SelectedIndex;
            if (radios.SelectedIndex != 2)
            {
                uriBox.Text = string.Empty;
                App.settings["SpecificUri"] = uriBox.Text;
            }
        }

        private void SuggestUriVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings["ShowSuggestUri"] = showSuggestUri.IsOn;
        }

        private void ShowBackgroundChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            backgroundCard.IsEnabled = showBackground.IsOn;
            App.settings["ShowBackground"] = showBackground.IsOn;
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
