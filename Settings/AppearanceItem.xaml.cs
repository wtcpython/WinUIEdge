using Edge.Data;
using Edge.Utilities;
using Microsoft.UI;
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
            var themeList = Enum.GetNames(typeof(ElementTheme));
            appearanceBox.ItemsSource = themeList;
            appearanceBox.SelectedIndex = Array.IndexOf(themeList, App.settings["Appearance"].ToString());
            effectBox.ItemsSource = Info.WindowEffectList;
            effectBox.SelectedIndex = Info.WindowEffectList.IndexOf(App.settings["WindowEffect"].ToString());
            inLoading = false;
        }

        private void AppearanceChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                string appearance = appearanceBox.SelectedItem.ToString();
                App.settings["Appearance"] = appearance;

                foreach (Window window in App.mainWindows)
                {
                    if (window.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = Enum.Parse<ElementTheme>(appearance);
                        window.AppWindow.TitleBar.ButtonForegroundColor =
                            rootElement.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
                        Theme.UpdateTitleBarContextMenu(rootElement.RequestedTheme);
                    }
                }
            }
        }

        private void EffectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!inLoading)
            {
                App.settings["WindowEffect"] = effectBox.SelectedItem.ToString();

                foreach (Window window in App.mainWindows)
                {
                    window.SystemBackdrop = Theme.SetBackDrop();
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
    }
}
