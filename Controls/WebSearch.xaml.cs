using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.Foundation;

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
            this.PointerPressed += OnGlobalPointerPressed;
            View.ItemsSource = Info.SearchEngineList;
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

        private void OnGlobalPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerPosition = e.GetCurrentPoint(this).Position;
            var rect = SuggestionPopup.TransformToVisual(this).TransformBounds(new Rect(0, 0, SuggestionPopup.ActualWidth, SuggestionPopup.ActualHeight));

            if (!rect.Contains(pointerPosition))
            {
                showPopup(false);
            }
        }

        private void showPopup(bool visible)
        {
            panel.Width = SearchBox.ActualWidth;
            if (visible)
            {
                SuggestionPopup.IsOpen = visible;
            }

            DoubleAnimation animation = new()
            {
                From = visible ? 0 : 1,
                To = visible ? 1 : 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(200))
            };
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, SuggestionPopup);
            Storyboard.SetTargetProperty(animation, "Opacity");
            if (!visible)
            {
                storyboard.Completed += (s, e) => SuggestionPopup.IsOpen = visible;
            }
            storyboard.Begin();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (text == string.Empty)
            {
                box.Visibility = Visibility.Collapsed;
            }
            else
            {
                box.Visibility = Visibility.Visible;
                box.Text = $"使用默认搜索引擎 (或作为网址) 搜索：{text}";
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                StartToSearch();
            }
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

        private void SearchEngineClick(object sender, ItemClickEventArgs e)
        {
            Navigate((e.ClickedItem as WebsiteInfo).Uri + SearchBox.Text);
        }
    }
}