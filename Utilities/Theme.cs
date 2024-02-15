using Edge.Data;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edge.Utilities
{
    public static class Theme
    {
        public static ElementTheme Convert(string theme)
        {
            var themeList = Enum.GetNames(typeof(ElementTheme)).ToList();
            List<ElementTheme> array = Enum.GetValues(typeof(ElementTheme)).OfType<ElementTheme>().ToList();
            return array[themeList.IndexOf(theme)];
        }

        public static SystemBackdrop SetBackDrop()
        {
            if (Info.data.WindowEffect == Info.WindowEffectList[0])
            {
                return new MicaBackdrop();
            }
            else if (Info.data.WindowEffect == Info.WindowEffectList[1])
            {
                return new MicaBackdrop()
                {
                    Kind = MicaKind.BaseAlt
                };
            }
            else if (Info.data.WindowEffect == Info.WindowEffectList[2])
            {
                return new DesktopAcrylicBackdrop();
            }
            return null;
        }
    }
}