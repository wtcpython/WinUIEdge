using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;

namespace Edge
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            navigation.SelectedItem = navigation.MenuItems.OfType<NavigationViewItem>().First();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            string tag = (string)(args.SelectedItem as NavigationViewItem).Tag;
            ContentFrame.Navigate(Type.GetType("Edge." + tag));
            navigation.Header = ((NavigationViewItem)navigation.SelectedItem)?.Content?.ToString();
        }
    }
}
