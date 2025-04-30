using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml;

namespace Edge
{
    public sealed partial class SystemItem : Page
    {
        public SystemItem()
        {
            this.InitializeComponent();
            setToggleEnableGpu.IsOn = !App.settings.DisableGpu;
            setToggleDisableBackgroundTimerThrottling.IsOn = App.settings.DisableBackgroundTimerThrottling;
        }

        private void ToggleEnableGpu(object sender, RoutedEventArgs e)
        {
            if (App.settings.DisableGpu == setToggleEnableGpu.IsOn)
            {
                App.settings.DisableGpu = !setToggleEnableGpu.IsOn;
                restartInfoBar.IsOpen = true;
            }
        }

        private async void OpenSettingsProxy(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network-proxy"));
        }

        private void ToggleDisableBackgroundTimerThrottling(object sender, RoutedEventArgs e)
        {
            if (App.settings.DisableBackgroundTimerThrottling != setToggleDisableBackgroundTimerThrottling.IsOn)
            {
                App.settings.DisableBackgroundTimerThrottling = setToggleDisableBackgroundTimerThrottling.IsOn;
                restartInfoBar.IsOpen = true;
            }
        }
    }
}
