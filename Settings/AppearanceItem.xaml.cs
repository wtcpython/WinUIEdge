using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace Edge
{
    public class ToolBarVisual
    {
        public string Text { get; set; }
        public bool Visual { get; set; }
    }

    public sealed partial class AppearanceItem : Page
    {
        public static bool inLoading = false;
        public Dictionary<string, bool> ToolBar = JsonSerializer.Deserialize<Dictionary<string, bool>>(App.settings["ToolBar"].ToJsonString());
        public List<ToolBarVisual> ToolBarVisualList = [];
        public List<string> themeList = [.. Enum.GetNames(typeof(ElementTheme))];
        public AppearanceItem()
        {
            this.InitializeComponent();
            ToolBarVisualList = ToolBar.Select(x => new ToolBarVisual()
            {
                Text = x.Key,
                Visual = x.Value
            }).ToList();
            toolBarVisualView.ItemsSource = ToolBarVisualList;

            inLoading = true;
            appearanceView.SelectedIndex = themeList.IndexOf(App.settings["Appearance"].ToString());
            showMicaSwitch.IsOn = App.settings["ShowMicaIfEnabled"].GetValue<bool>();
            inLoading = false;
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                int index = appearanceView.SelectedIndex;
                App.settings["Appearance"] = themeList[index];

                foreach (Window window in App.mainWindows)
                {
                    window.SetThemeColor();
                }
            }
        }

        private void VisualChanged(object sender, RoutedEventArgs e)
        {
            foreach (var visual in ToolBarVisualList)
            {
                if ((sender as ToggleSwitch).Name == visual.Text)
                {
                    ToolBar[visual.Text] = (sender as ToggleSwitch).IsOn;
                    App.settings["ToolBar"] = JsonSerializer.SerializeToNode(ToolBar);
                }
            }
        }

        private void MicaEffectChanged(object sender, RoutedEventArgs e)
        {
            if (!inLoading)
            {
                App.settings["ShowMicaIfEnabled"] = (sender as ToggleSwitch).IsOn;

                foreach (Window window in App.mainWindows)
                {
                    window.SetBackdrop();
                }
            }
        }
    }
}
