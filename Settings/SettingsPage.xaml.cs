using Microsoft.UI.Xaml.Controls;
using System;

namespace Edge
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            navigation.SelectedItem = navigation.MenuItems[0];
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            string tag = (string)(args.SelectedItem as NavigationViewItem).Tag;
            ContentFrame.Navigate(Type.GetType("Edge." + tag));
            navigation.Header = ((NavigationViewItem)navigation.SelectedItem)?.Content?.ToString();
        }
    }
}
