using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class SafetyItem : Page
    {
        public SafetyItem()
        {
            this.InitializeComponent();
            msSmartScreen.IsOn = App.CoreWebView2.Settings.IsReputationCheckingRequired;
        }

        private void SmartScreenChanged(object sender, RoutedEventArgs e)
        {
            App.CoreWebView2.Settings.IsReputationCheckingRequired = (sender as ToggleSwitch).IsOn;
        }
    }
}
