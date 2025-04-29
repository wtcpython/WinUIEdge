using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge
{
    public class ToolBarVisual
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public bool Visual { get; set; }
    }

    public sealed partial class ToolBarItem : Page
    {
        public List<string> menuStatusList = ["Always", "Never", "OnlyOnNewTab"];

        public List<ToolBarVisual> ToolBarVisualList = [];
        public Dictionary<string, string> ToolBarName = new(){
            {"HomeButton", "“开始”按钮"},
            {"ForwardButton", "“前进”按钮"},
            {"ExtensionsButton", "扩展按钮"},
            {"HistoryButton", "历史按钮"},
            {"DownloadButton", "下载按钮"}
        };

        public ToolBarItem()
        {
            this.InitializeComponent();
            menuStatusBox.ItemsSource = menuStatusList;
            menuStatusBox.SelectedIndex = menuStatusList.IndexOf(App.settings.MenuStatus);

            ToolBarVisualList = App.settings.ToolBar.Select(x => new ToolBarVisual()
            {
                Text = x.Key,
                Description = ToolBarName[x.Key],
                Visual = x.Value
            }).ToList();
            toolBarVisualView.ItemsSource = ToolBarVisualList;
        }

        private void MenuStatusChanged(object sender, SelectionChangedEventArgs e)
        {
            App.settings.MenuStatus = menuStatusList[menuStatusBox.SelectedIndex];
        }

        private void VisualChanged(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch { DataContext: ToolBarVisual visual } toggle) {
                Dictionary<string, bool> toolbar = App.settings.ToolBar;
                if (toolbar.TryGetValue(visual.Text, out bool oldValue) && oldValue != toggle.IsOn) {
                    toolbar[visual.Text] = !oldValue;
                    visual.Visual = !oldValue;
                }
            }
        }
    }
}
