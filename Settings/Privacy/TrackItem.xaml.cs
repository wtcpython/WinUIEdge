using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;

namespace Edge
{
    public sealed partial class TrackItem : Page
    {
        public List<string> tracks = ["None", "基本", "平衡（推荐）", "严格"];

        public List<CoreWebView2TrackingPreventionLevel> trackLevelList = [.. Enum.GetValues<CoreWebView2TrackingPreventionLevel>()];

        public TrackItem()
        {
            this.InitializeComponent();
            trackBox.ItemsSource = tracks;

            var level = App.CoreWebView2Profile.PreferredTrackingPreventionLevel;
            trackBox.SelectedIndex = trackLevelList.IndexOf(level);
        }

        private void TrackLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            App.CoreWebView2Profile.PreferredTrackingPreventionLevel = trackLevelList[trackBox.SelectedIndex];
        }
    }
}
