using Microsoft.UI.Xaml.Controls;
using System;

namespace Edge
{
    public sealed partial class PrinterItem : Page
    {
        public PrinterItem()
        {
            this.InitializeComponent();
        }

        private async void OpenSettingsPrinter(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:printers"));
        }
    }
}
