using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public sealed partial class StartItem : Page
    {
        public StartItem()
        {
            this.InitializeComponent();
            radios.SelectedIndex = App.settings.StartBehavior;

            uriBox.Text = App.settings.SpecificUri;
            uriBox.IsEnabled = radios.SelectedIndex == 2;
            uriButton.IsEnabled = radios.SelectedIndex == 2;

            setHomeButton.IsOn = App.settings.ToolBar.GetValueOrDefault("HomeButton", false);

            searchEngineBox.ItemsSource = Info.SearchEngineList.Select(x => x.Name).ToList();
            searchEngineBox.SelectedItem =
                Info.SearchEngineList.First(x => x.Name.Equals(App.settings.SearchEngine)).Name;

            showBackground.IsOn = backgroundCard.IsEnabled = App.settings.ShowBackground;
        }

        private void CheckUri(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            UriType uriType = uriBox.Text.DetectUri();
            if (uriType == UriType.WithoutProtocol)
            {
                uriBox.Text = "https://" + uriBox.Text;
                uriType = UriType.WithProtocol;
            }

            if (uriType != UriType.WithProtocol)
            {
                var builder = new AppNotificationBuilder()
                    .AddText("输入的 Uri 非法，请修改内容")
                    .AddArgument("Notification", "ChangeStartUri");
                var notificationManager = AppNotificationManager.Default;
                notificationManager.Show(builder.BuildNotification());
            }
            else
            {
                App.settings.SpecificUri = uriBox.Text;
            }
        }

        private void HomeButtonVisualChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            App.settings.ToolBar["HomeButton"] = setHomeButton.IsOn;
        }

        private void SearchEngineChanged(object sender, SelectionChangedEventArgs e)
        {
            App.settings.SearchEngine =
                Info.SearchEngineList.First(x => x.Name == (string)searchEngineBox.SelectedItem).Name;
        }

        private void BehaviorChanged(object sender, SelectionChangedEventArgs e)
        {
            uriBox.IsEnabled = radios.SelectedIndex == 2;
            uriButton.IsEnabled = radios.SelectedIndex == 2;
            App.settings.StartBehavior = radios.SelectedIndex;
            if (radios.SelectedIndex != 2)
            {
                uriBox.Text = string.Empty;
                App.settings.SpecificUri = uriBox.Text;
            }
        }

        private void ShowBackgroundChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            backgroundCard.IsEnabled = showBackground.IsOn;
            App.settings.ShowBackground = showBackground.IsOn;
        }

        private async void SetBackgroundImage(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker picker = new(this.GetWindowId())
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { { ".jpg" }, { ".jpeg" }, { ".png" } }
            };

            var result = await picker.PickSingleFileAsync();
            App.settings.BackgroundImage = result.Path;
        }
    }
}
