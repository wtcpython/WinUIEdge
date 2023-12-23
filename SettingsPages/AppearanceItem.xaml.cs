using Microsoft.UI.Xaml.Controls;

namespace Edge
{
    public sealed partial class AppearanceItem : Page
    {
        public AppearanceItem()
        {
            this.InitializeComponent();
            themeBox.ItemsSource = JsonDataList.ApplicationThemeList;
            themeBox.SelectedIndex = JsonDataList.ApplicationThemeList.IndexOf(Utils.data.Theme);
            if (themeBox.SelectedIndex == -1 ) { themeBox.SelectedIndex = 0; };
        }

        private void ThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            Utils.data.Theme = themeBox.SelectedItem.ToString();
            (App.Window as MainWindow).SystemBackdrop = Utils.SetBackDrop();
        }
    }
}
