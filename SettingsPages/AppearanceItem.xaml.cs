using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class AppearanceItem : Page
    {
        public AppearanceItem()
        {
            this.InitializeComponent();
            this.Loaded += AppearanceItemLoaded;
        }

        private void AppearanceItemLoaded(object sender, RoutedEventArgs e)
        {
            appearanceBox.ItemsSource = JsonDataList.AppearanceList;
            appearanceBox.SelectedIndex = JsonDataList.AppearanceList.IndexOf(Utils.data.Appearance);
            effectBox.ItemsSource = JsonDataList.WindowEffectList;
            effectBox.SelectedIndex = JsonDataList.WindowEffectList.IndexOf(Utils.data.WindowEffect);
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            string appearance = Utils.data.Appearance = appearanceBox.SelectedItem.ToString();

            if (App.Window.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Utils.ConvertTheme(appearance);
                App.Window.AppWindow.TitleBar.ButtonForegroundColor = 
                    rootElement.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
            }
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            Utils.data.WindowEffect = effectBox.SelectedItem.ToString();
            (App.Window as MainWindow).SystemBackdrop = Utils.SetBackDrop();
        }
    }
}
