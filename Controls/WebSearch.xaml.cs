using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace Edge
{
    public sealed partial class WebSearch : UserControl
    {
        public ObservableCollection<string> suggestItems = [];

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
            suggestItems.Clear();
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                suggestItems.Add($"将 {sender.Text} 作为网址搜索");
                foreach (var item in Info.SearchEngineList)
                {
                    suggestItems.Add($"使用 {item.Name} 搜索 \"{sender.Text}\"");
                }
                sender.ItemsSource = suggestItems;
            }
        }

        private void SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            int index = suggestItems.IndexOf(args.SelectedItem as string);
            if (index > 0)
            {
                Navigate(Info.SearchEngineList[index - 1].Uri + sender.Text);
            }
            else
            {
                if (!sender.Text.StartsWith("https://"))
                {
                    sender.Text = "https://" + sender.Text;
                }
                Navigate(sender.Text);
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
                        if (Info.LanguageDict.ContainsKey(ext))
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

                        else if (Info.ImageDict.ContainsKey(ext))
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

        private void AutoSuggestBox_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "粘贴文件路径";
            }
        }

        private async void AutoSuggestBox_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
        {
            var items = await e.DataView.GetStorageItemsAsync();
            foreach (var item in items)
            {
                box.Text = item.Path;
            }
        }
    }
}