using Edge.Data;
using Edge.Utilities;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class AppearanceItem : Page
    {
        public static bool inLoading = false;
        public AppearanceItem()
        {
            this.InitializeComponent();
            this.Loaded += AppearanceItemLoaded;
        }

        private void AppearanceItemLoaded(object sender, RoutedEventArgs e)
        {
            inLoading = true;
            appearanceBox.ItemsSource = Info.AppearanceList;
            appearanceBox.SelectedIndex = Info.AppearanceList.IndexOf(Info.data.Appearance);
            effectBox.ItemsSource = Info.WindowEffectList;
            effectBox.SelectedIndex = Info.WindowEffectList.IndexOf(Info.data.WindowEffect);
            inLoading = false;
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                string appearance = Info.data.Appearance = appearanceBox.SelectedItem.ToString();

                foreach (Window window in App.mainWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = Theme.Convert(appearance);
                        window.AppWindow.TitleBar.ButtonForegroundColor =
                            rootElement.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
                    }
                }
            }
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                Info.data.WindowEffect = effectBox.SelectedItem.ToString();

                foreach (Window window in App.mainWindows)
                {
                    window.SystemBackdrop = Theme.SetBackDrop();
                }
            }
        }
    }
}
