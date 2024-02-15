using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;

namespace Edge
{
    public sealed partial class SearchModule : Page
    {
        public SearchModule()
        {
            this.InitializeComponent();

            SearchEngineBox.ItemsSource = Info.SearchEngineList;

            SearchEngineBox.SelectedIndex = Info.SearchEngineList.Select(x => x.Name).ToList().IndexOf(Info.data.SearchEngine);
        }

        private void SearchKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (Uri.TryCreate(text, UriKind.Absolute, out Uri uriResult))
                {
                    if (uriResult.Scheme == "file")
                    {
                        if (File.Exists(uriResult.OriginalString))
                        {
                            string ext = Path.GetExtension(text);

                            MainWindow mainWindow = App.GetWindowForElement(this) as MainWindow;
                            if (Info.LanguageDict.ContainsKey(ext))
                            {
                                mainWindow.AddNewTab(new TextFilePage(text), Path.GetFileName(text));
                            }

                            else if (Info.ImageDict.ContainsKey(ext))
                            {
                                mainWindow.AddNewTab(new ImageViewer(text), Path.GetFileName(text));
                            }

                            else
                            {
                                this.Navigate(text);
                            }
                        }
                    }
                    else if (Info.ProtocolList.Contains(uriResult.Scheme))
                    {
                        this.Navigate(text);
                    }
                }
                else Navigate(Info.SearchEngineList[SearchEngineBox.SelectedIndex].Uri + text);
            }
        }

        private void Navigate(string uri)
        {
            MainWindow mainWindow = App.GetWindowForElement(this) as MainWindow;
            var selectedItem = mainWindow.SelectedItem;
            if (selectedItem is WebViewPage webviewPage)
            {
                webviewPage.WebUri = uri;
            }
            else
            {
                mainWindow.AddNewTab(new WebViewPage() { WebUri = uri });
            }
        }

        public string Text
        {
            get => InputBox.Text;
            set => InputBox.Text = value;
        }

        public new CornerRadius CornerRadius
        {
            get => InputBox.CornerRadius;
            set => InputBox.CornerRadius = value;
        }

        public new double FontSize
        {
            get => InputBox.FontSize;
            set => InputBox.FontSize = value;
        }
    }
}