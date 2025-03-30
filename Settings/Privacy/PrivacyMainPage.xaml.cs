using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace Edge
{
    public sealed partial class PrivacyMainPage : Page
    {
        public PrivacyMainPage()
        {
            this.InitializeComponent();
        }

        private void JumpToTrackItem(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TrackItem));
            SettingsPage.BreadcrumbBarItems.Add(new()
            {
                Header = "跟踪防护",
                Type = typeof(TrackItem)
            });
        }

        private void JumpToClearDataItem(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClearDataItem));
            SettingsPage.BreadcrumbBarItems.Add(new()
            {
                Header = "清除浏览数据",
                Type = typeof(ClearDataItem)
            });
        }

        private void JumpToSafetyItem(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SafetyItem));
            SettingsPage.BreadcrumbBarItems.Add(new()
            {
                Header = "安全性",
                Type = typeof(SafetyItem)
            });
        }
    }
}
