using Edge.Data;
using Edge.Utilities;
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
            appearanceBox.ItemsSource = Info.AppearanceList;
            appearanceBox.SelectedIndex = Info.AppearanceList.IndexOf(Info.data.Appearance);
            effectBox.ItemsSource = Info.WindowEffectList;
            effectBox.SelectedIndex = Info.WindowEffectList.IndexOf(Info.data.WindowEffect);
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            string appearance = Info.data.Appearance = appearanceBox.SelectedItem.ToString();

            if (App.Window.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Theme.Convert(appearance);
                App.Window.AppWindow.TitleBar.ButtonForegroundColor = 
                    rootElement.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
            }
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            Info.data.WindowEffect = effectBox.SelectedItem.ToString();
            (App.Window as MainWindow).SystemBackdrop = Theme.SetBackDrop();
        }
    }
}
