using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Edge
{
    public sealed partial class WebSearch : UserControl
    {
        public string Text
        {
            get => SearchBox.Text;
            set => SearchBox.Text = value;
        }

        public WebSearch()
        {
            this.InitializeComponent();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            showPopup(true);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (SuggestionPopup.IsOpen)
                {
                    showPopup(false);
                }
            });
        }

        private void showPopup(bool visible)
        {
            SuggestionPopup.IsOpen = visible;
            listView.Width = SearchBox.ActualWidth;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            string lastWord = text.Split(' ').Last().ToLower();
            if (!string.IsNullOrEmpty(lastWord))
            {
                var result = App.searchEngine.SearchWords(lastWord).Take(10);
                listView.ItemsSource = result;
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                StartToSearch();
            }
        }

        private void SuggestItemClick(object sender, ItemClickEventArgs e)
        {
            string item = e.ClickedItem as string;
            string text = SearchBox.Text;
            int lastIndex = text.LastIndexOf(' ');
            if (lastIndex == -1)
            {
                SearchBox.Text = item;
                return;
            }

            string prefix = text[..lastIndex];
            SearchBox.Text = $"{prefix} {item}";
        }

        private void StartToSearch()
        {
            string text = SearchBox.Text;
            if (Uri.TryCreate(text, UriKind.Absolute, out var uri))
            {
                if (uri.Scheme == Uri.UriSchemeFile)
                {
                    FileInfo fileInfo = new(text);
                    if (fileInfo.Exists)
                    {
                        string ext = fileInfo.Extension;

                        MainWindow mainWindow = App.GetWindowForElement(this);
                        if (Info.LanguageDict.RootElement.TryGetProperty(ext, out JsonElement _))
                        {
                            if (ext == ".lnk")
                            {
                                mainWindow.AddNewTab(new InkFilePage(text), fileInfo.Name);
                            }
                            else
                            {
                                mainWindow.AddNewTab(new TextFilePage(text), fileInfo.Name);
                            }
                        }

                        else if (Info.ImageDict.RootElement.TryGetProperty(ext, out JsonElement _))
                        {
                            mainWindow.AddNewTab(new ImageViewer(text), fileInfo.Name);
                        }

                        else
                        {
                            Navigate(text);
                        }
                    }
                }
                else
                {
                    Navigate(uri.OriginalString);
                }
            }
            else
            {
                Navigate(Info.SearchEngineList.First(x => x.Name == App.settings["SearchEngine"].ToString()).Uri + text);
            }
        }

        private void Navigate(string uri)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
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
    }
}