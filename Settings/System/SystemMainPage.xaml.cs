using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace Edge
{
    public sealed partial class SystemMainPage : Page
    {
        public SystemMainPage()
        {
            this.InitializeComponent();
        }
        
        private void JumpToSystemItem(object sender, RoutedEventArgs e)
        {
            Frame.Navigate((typeof(SystemItem)));
            SettingsPage.BreadcrumbBarItems.Add(new()
            {
                Header = "系统",
                Type = typeof(SystemItem)
            });
        }
        
        private void JumpToPerformanceItem(object sender, RoutedEventArgs e)
        {
        }
        
        private async void OpenSettingsPrinter(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:printers"));
        }
    }
}
