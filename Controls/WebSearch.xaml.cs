using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

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
            string lastWord = text.Split(' ')[^1].ToLower();
            if (!string.IsNullOrEmpty(lastWord))
            {
                var result = App.searchEngine.SearchWords(lastWord).Take(10).ToList();
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
            SearchBox.Text = string.Empty;
            UriType uriType = text.DetectUri();
            if (uriType == UriType.WithProtocol)
            {
                Navigate(text);
            }
            else if (uriType == UriType.WithoutProtocol)
            {
                Navigate("https://" + text);
            }
            else if (File.Exists(text))
            {
                FileInfo fileInfo = new(text);
                string ext = fileInfo.Extension;
                MainWindow mainWindow = App.GetWindowForElement(this);
                if (Info.LanguageDict.TryGetValue(ext, out var _))
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
                else if (Info.ImageDict.TryGetValue(ext, out var _))
                {
                    mainWindow.AddNewTab(new ImageViewer(text), fileInfo.Name);
                }
                else
                {
                    Navigate(text);
                }
            }
            else
            {
                Navigate(Info.SearchEngineList.First(x => x.Name == App.settings.SearchEngine).Uri + text);
            }
        }

        private void Navigate(string site)
        {
            Uri uri = new(site);
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

        private void SearchBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void SearchBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                var file = items.OfType<StorageFile>().FirstOrDefault();
                if (file != null)
                {
                    SearchBox.Text = file.Path;
                }
            }
        }
    }
}