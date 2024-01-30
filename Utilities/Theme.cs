using Edge.Data;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Edge.Utilities
{
    public static class Theme
    {
        public static ElementTheme Convert(string theme)
        {
            if (theme == Info.AppearanceList[0]) return ElementTheme.Default;
            else if (theme == Info.AppearanceList[1]) return ElementTheme.Light;
            else return ElementTheme.Dark;
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