using Edge.Data;
using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<ToolBarVisual> ToolBarVisualList = [];
        public List<string> themeList = [.. Enum.GetNames(typeof(ElementTheme))];
        public AppearanceItem()
        {
            this.InitializeComponent();
            ToolBarVisualList = App.settings["ToolBar"].Select(x => new ToolBarVisual()
            {
                Text = ((JProperty)x).Name,
                Visual = x.ToObject<bool>()
            }).ToList();
            toolBarVisualView.ItemsSource = ToolBarVisualList;

            inLoading = true;
            appearanceView.SelectedIndex = themeList.IndexOf(App.settings["Appearance"].ToString());
            showMicaSwitch.IsOn = App.settings["ShowMicaIfEnabled"].ToObject<bool>();
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
                    App.settings["ToolBar"][visual.Text] = (sender as ToggleSwitch).IsOn;
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
