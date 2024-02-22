using Edge.Data;
using Edge.Utilities;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace Edge
{
    public sealed partial class AppearanceItem : Page
    {
        public static bool inLoading = false;
        public AppearanceItem()
        {
            this.InitializeComponent();
            this.Loaded += AppearanceItemLoaded;
        }

        private void AppearanceItemLoaded(object sender, RoutedEventArgs e)
        {
            inLoading = true;
            var themeList = Enum.GetNames(typeof(ElementTheme));
            appearanceBox.ItemsSource = themeList;
            appearanceBox.SelectedIndex = Array.IndexOf(themeList, App.settings["Appearance"].ToString());
            effectBox.ItemsSource = Info.WindowEffectList;
            effectBox.SelectedIndex = Info.WindowEffectList.IndexOf(App.settings["WindowEffect"].ToString());
            inLoading = false;
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                string appearance = appearanceBox.SelectedItem.ToString();
                App.settings["Appearance"] = appearance;

                foreach (Window window in App.mainWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = Enum.Parse<ElementTheme>(appearance);
                        window.AppWindow.TitleBar.ButtonForegroundColor =
                            rootElement.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
                        Theme.UpdateTitleBarContextMenu(rootElement.RequestedTheme);
                    }
                }
            }
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                App.settings["WindowEffect"] = effectBox.SelectedItem.ToString();

                foreach (Window window in App.mainWindows)
                {
                    window.SystemBackdrop = Theme.SetBackDrop();
                }
            }
        }
    }
}
