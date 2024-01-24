using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class AppearanceItem : Page
    {
        public AppearanceItem()
        {
            this.InitializeComponent();
            appearanceBox.ItemsSource = JsonDataList.AppearanceList;
            appearanceBox.SelectedIndex = JsonDataList.AppearanceList.IndexOf(Utils.data.Appearance);
            effectBox.ItemsSource = JsonDataList.WindowEffectList;
            effectBox.SelectedIndex = JsonDataList.WindowEffectList.IndexOf(Utils.data.WindowEffect);
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            var appearance = Utils.data.Appearance = appearanceBox.SelectedItem.ToString();
            if (appearance == JsonDataList.AppearanceList[0])
            {
                if (App.Window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = ElementTheme.Default;
                }
            }
            else if (appearance == JsonDataList.AppearanceList[1])
            {
                if (App.Window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = ElementTheme.Light;
                }
            }
            else if (appearance == JsonDataList.AppearanceList[2])
            {
                if (App.Window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = ElementTheme.Dark;
                }
            }
            
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            Utils.data.WindowEffect = effectBox.SelectedItem.ToString();
            (App.Window as MainWindow).SystemBackdrop = Utils.SetBackDrop();
        }
    }
}
