using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Edge
{
    public class SuggestItem
    {
        public string Text { get; set; }
        public string Uri { get; set; }
    }

    public sealed partial class WebSearch : UserControl
    {
        public string Text
        {
            get => box.Text;
            set => box.Text = value;
        }

        public WebSearch()
        {
            this.InitializeComponent();
        }

        private void SuggestTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            List<SuggestItem> suggestItems = [];
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (Uri.TryCreate(sender.Text, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps))
                {
                    suggestItems.Add(new()
                    {
                        Text = $"搜索网址",
                        Uri = sender.Text
                    });
                }
                else
                {
                    suggestItems.AddRange(Info.SearchEngineList.Select(x => new SuggestItem()
                    {
                        Text = $"使用 {x.Name} 搜索",
                        Uri = x.Uri + sender.Text
                    }));
                }
                sender.ItemsSource = suggestItems;
            }
        }

        private void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Navigate((args.SelectedItem as SuggestItem).Uri);
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

        private void StartToSearch(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string text = sender.Text;
            if (Uri.TryCreate(text, UriKind.Absolute, out var uri))
            {
                if (uri.Scheme == Uri.UriSchemeFile)
                {
                    FileInfo fileInfo = new(text);
                    if (fileInfo.Exists)
                    {
                        string ext = fileInfo.Extension;

                        MainWindow mainWindow = App.GetWindowForElement(this);
                        if (Info.LanguageDict.Select(x => ((JProperty)x).Name).Any(x => x == ext))
                        {
                            mainWindow.AddNewTab(new TextFilePage(text), fileInfo.Name);
                        }

                        else if (Info.ImageDict.Select(x => ((JProperty)x).Name).Any(x => x == ext))
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
    }
}