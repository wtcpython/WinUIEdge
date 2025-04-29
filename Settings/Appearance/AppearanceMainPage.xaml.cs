using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System;

namespace Edge
{
    public sealed partial class AppearanceMainPage : Page
    {
        private static bool inLoading = false;

        public List<string> themeList = [.. Enum.GetNames<ElementTheme>()];
        public List<string> effects = [.. Enum.GetNames<Effect>()];

        public AppearanceMainPage()
        {
            this.InitializeComponent();
            effectBox.ItemsSource = effects;

            inLoading = true;
            appearanceView.SelectedIndex = themeList.IndexOf(App.settings.Appearance);
            effectBox.SelectedIndex = effects.IndexOf(App.settings.BackgroundEffect.ToString());
            inLoading = false;
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                int index = appearanceView.SelectedIndex;
                App.settings.Appearance = themeList[index];

                foreach (Window window in App.mainWindows)
                {
                    window.SetThemeColor();
                }
            }
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                App.settings.BackgroundEffect = Enum.Parse<Effect>((sender as ComboBox).SelectedItem.ToString());

                foreach (Window window in App.mainWindows)
                {
                    window.SetBackdrop();
                }
            }
        }

        private void JumpToToolBarItem(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ToolBarItem));
            SettingsPage.BreadcrumbBarItems.Add(new()
            {
                Header = "工具栏",
                Type = typeof(ToolBarItem)
            });
        }
    }
}
