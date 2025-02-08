using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;

namespace Edge
{
    public sealed partial class WebSearch : UserControl
    {
        public string Text
        {
            get => searchBox.Text;
            set => searchBox.Text = value;
        }

        public WebSearch()
        {
            this.InitializeComponent();
        }

        public static void StartSearch(string text, MainWindow mainWindow)
        {
            UriType uriType = text.DetectUri();
            if (uriType == UriType.WithProtocol)
            {
                Navigate(text, mainWindow);
            }
            else if (uriType == UriType.WithoutProtocol)
            {
                Navigate("https://" + text, mainWindow);
            }
            else if (File.Exists(text))
            {
                FileInfo fileInfo = new(text);
                string ext = fileInfo.Extension;
                if (Info.LanguageDict.TryGetValue(ext, out var _))
                {
                    mainWindow.AddNewTab(new TextFilePage(fileInfo), fileInfo.Name);
                }
                else if (Info.ImageDict.TryGetValue(ext, out var _))
                {
                    mainWindow.AddNewTab(new ImageViewer(fileInfo), fileInfo.Name);
                }
                else
                {
                    Navigate(text, mainWindow);
                }
            }
            else
            {
                Navigate(Info.SearchEngineList.First(x => x.Name == App.settings.SearchEngine).Uri + text, mainWindow);
            }
        }

        public static void Navigate(string site, MainWindow mainWindow)
        {
            Uri uri = new(site);
            if ((mainWindow.TabView.SelectedItem != null) && (mainWindow.SelectedItem is WebViewPage webviewPage))
            {
                webviewPage.WebView2.Source = uri;
            }
            else
            {
                mainWindow.AddNewTab(new WebViewPage(uri));
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                string text = sender.Text;
                string lastWord = text.Split(' ')[^1].ToLower();
                if (!string.IsNullOrEmpty(lastWord))
                {
                    sender.ItemsSource = App.searchEngine.SearchWords(lastWord);
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string text = sender.Text;
            string item = args.SelectedItem as string;

            int lastIndex = text.LastIndexOf(' ');
            if (lastIndex == -1)
            {
                sender.Text = item;
                return;
            }

            string prefix = text[..lastIndex];
            sender.Text = $"{prefix} {item}";
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion == null)
            {
                string text = args.QueryText;
                sender.Text = string.Empty;
                MainWindow mainWindow = App.GetWindowForElement(this);
                StartSearch(text, mainWindow);
            }
        }
    }
}