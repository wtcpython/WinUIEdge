using Edge.Data;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;

namespace Edge.Utilities
{
    public enum PreferredAppMode
    {
        Default,
        AllowDark,
        ForceDark,
        ForceLight,
        Max
    };

    public static class Theme
    {
        public static SystemBackdrop SetBackDrop()
        {
            string effect = App.settings["WindowEffect"].ToString();
            if (effect == Info.WindowEffectList[0])
            {
                return new MicaBackdrop();
            }
            else if (effect == Info.WindowEffectList[1])
            {
                return new MicaBackdrop()
                {
                    Kind = MicaKind.BaseAlt
                };
            }
            else if (effect == Info.WindowEffectList[2])
            {
                return new DesktopAcrylicBackdrop();
            }
            return null;
        }

        [DllImport("uxtheme.dll", EntryPoint = "#135")]
        private static extern IntPtr SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [DllImport("uxtheme.dll", EntryPoint = "#136")]
        private static extern IntPtr FlushMenuThemes();

        public static void UpdateTitleBarContextMenu(ElementTheme theme)
        {
            var mode = theme switch
            {
                ElementTheme.Light => PreferredAppMode.ForceLight,
                ElementTheme.Dark => PreferredAppMode.ForceDark,
                _ => PreferredAppMode.AllowDark,
            };
            SetPreferredAppMode(mode);
            FlushMenuThemes();
        }
    }
}