using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace Edge
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            if (App.settings["ShowSuggestUri"].ToObject<bool>())
            {
                View.ItemsSource = Info.SuggestWebsiteList;
            }
            else
            {
                View.Visibility = Visibility.Collapsed;
            }

            if (App.settings["ShowBackground"].ToObject<bool>())
            {
                homeGrid.Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage()
                    {
                        UriSource = new Uri(App.settings["BackgroundImage"].ToString())
                    },
                    Stretch = Stretch.UniformToFill
                };
            }
        }

        private void OpenSuggestWebsite(object sender, ItemClickEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage()
            {
                WebUri = (e.ClickedItem as WebsiteInfo).Uri
            });
        }
    }
}