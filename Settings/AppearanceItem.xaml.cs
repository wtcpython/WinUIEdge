using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Edge
{
    public class ToolBarVisual
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public bool Visual { get; set; }
    }

    public sealed partial class AppearanceItem : Page
    {
        private static bool inLoading = false;
        public Dictionary<string, bool> ToolBar = App.settings.ToolBar;
        public List<ToolBarVisual> ToolBarVisualList = [];
        public List<string> themeList = [.. Enum.GetNames<ElementTheme>()];
        public Dictionary<string, string> ToolBarName = new(){
            {"HomeButton", "“开始”按钮"},
            {"ForwardButton", "“前进”按钮"},
            {"HistoryButton", "历史按钮"},
            {"DownloadButton", "下载按钮"}
        };
        public AppearanceItem()
        {
            this.InitializeComponent();
            ToolBarVisualList = ToolBar.Select(x => new ToolBarVisual()
            {
                Text = x.Key,
                Description = ToolBarName[x.Key],
                Visual = x.Value
            }).ToList();
            toolBarVisualView.ItemsSource = ToolBarVisualList;

            inLoading = true;
            appearanceView.SelectedIndex = themeList.IndexOf(App.settings.Appearance);
            showMicaSwitch.IsOn = App.settings.ShowMicaIfEnabled;
            inLoading = false;
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                int index = appearanceView.SelectedIndex;
                App.settings.Appearance = themeList[index];

                foreach (Window window in App.mainWindows)
                {
                    window.SetThemeColor();
                }
            }
        }

        private void VisualChanged(object sender, RoutedEventArgs e)
        {
            ToolBarVisual visual = (sender as ToggleSwitch).DataContext as ToolBarVisual;
            ToolBar[visual.Text] = (sender as ToggleSwitch).IsOn;
            App.settings.ToolBar = ToolBar;
        }

        private void MicaEffectChanged(object sender, RoutedEventArgs e)
        {
            if (!inLoading)
            {
                App.settings.ShowMicaIfEnabled = (sender as ToggleSwitch).IsOn;

                foreach (Window window in App.mainWindows)
                {
                    window.SetBackdrop();
                }
            }
        }
    }
}
