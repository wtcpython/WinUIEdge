using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

namespace Edge
{
    public class NavigationItem
    {
        public string Header { get; set; }
        public Type Type { get; set; }
    }

    public sealed partial class SettingsPage : Page
    {
        public static ObservableCollection<NavigationItem> BreadcrumbBarItems = [];
        public SettingsPage()
        {
            this.InitializeComponent();
            navigation.SelectedItem = navigation.MenuItems[0];
            breadcrumbBar.ItemsSource = BreadcrumbBarItems;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            string tag = (string)(args.SelectedItem as NavigationViewItem).Tag;

            UpdateBreadcrumb(new()
            {
                Header = ((NavigationViewItem)navigation.SelectedItem)?.Content?.ToString(),
                Type = Type.GetType("Edge." + tag)
            });
        }

        private void UpdateBreadcrumb(NavigationItem item)
        {
            BreadcrumbBarItems.Clear();
            BreadcrumbBarItems.Add(item);

            ContentFrame.Navigate(item.Type);
        }

        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            for (int i = BreadcrumbBarItems.Count - 1; i >= args.Index + 1; i--)
            {
                BreadcrumbBarItems.RemoveAt(i);
            }

            ContentFrame.Navigate(BreadcrumbBarItems[args.Index].Type);
        }
    }
}
