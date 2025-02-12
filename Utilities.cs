using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;

namespace Edge
{
    public enum UriType
    {
        WithProtocol,
        WithoutProtocol,
        PlainText
    }

    public static class Utilities
    {
        public static void SetBackdrop(this Window window)
        {
            if (App.settings.ShowMicaIfEnabled)
            {
                if (MicaController.IsSupported())
                {
                    window.SystemBackdrop = new MicaBackdrop();
                }
                else
                {
                    window.SystemBackdrop = new DesktopAcrylicBackdrop();
                }
            }
            else
            {
                window.SystemBackdrop = null;
            }
        }

        public static void SetThemeColor(this Window window)
        {
            string appearance = App.settings.Appearance;
            if (window.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Enum.Parse<ElementTheme>(appearance);
            }
        }

        public static IntPtr GetWindowHandle(this Window window)
        {
            return Win32Interop.GetWindowFromWindowId(window.AppWindow.Id);
        }

        public static IntPtr GetWindowHandle(this UIElement element)
        {
            Window window = App.GetWindowForElement(element);
            return window.GetWindowHandle();
        }

        public static unsafe string Win32SaveFile(string fileName, IntPtr hwnd)
        {
            try
            {
                FileInfo info = new(fileName);

                PInvoke.CoCreateInstance<IFileSaveDialog>(
                    typeof(FileSaveDialog).GUID,
                    null,
                    CLSCTX.CLSCTX_INPROC_SERVER,
                    out var fsd);
                List<COMDLG_FILTERSPEC> extensions = [
                    new()
                    {
                        pszSpec = (char* )Marshal.StringToHGlobalUni(info.Extension),
                        pszName = (char* )Marshal.StringToHGlobalUni(info.Extension),
                    }
                ];
                fsd.SetFileTypes(extensions.ToArray());
                string path = UserDataPaths.GetDefault().Downloads;

                PInvoke.SHCreateItemFromParsingName(
                    path,
                    null,
                    typeof(IShellItem).GUID,
                    out var directoryShellItem);

                fsd.SetFolder((IShellItem)directoryShellItem);
                fsd.SetDefaultFolder((IShellItem)directoryShellItem);
                fsd.SetFileName(info.Name);
                fsd.SetDefaultExtension(info.Extension);
                fsd.Show(new(hwnd));
                fsd.GetResult(out var ppsi);

                ppsi.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out PWSTR pwFileName);
                return pwFileName.ToString();
            }
            catch (Exception) { return string.Empty; }
        }

        public static string ToGlyph(this string name)
        {
            return name switch
            {
                "back" => "\ue72b",
                "forward" => "\ue72a",
                "reload" => "\ue72c",
                "saveAs" => "\ue792",
                "print" => "\ue749",
                "share" => "\ue72d",
                "emoji" => "\ue899",
                "undo" => "\ue7a7",
                "redo" => "\ue7a6",
                "cut" => "\ue8c6",
                "copy" => "\ue8c8",
                "paste" => "\ue77f",
                "openLinkInNewWindow" => "\ue737",
                "copyLinkLocation" => "\ue71b",
                _ => string.Empty,
            };
        }

        public static UriType DetectUri(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return UriType.PlainText;
            }
            if (text.Contains("://"))
            {
                return Uri.IsWellFormedUriString(text, UriKind.Absolute) ? UriType.WithProtocol : UriType.PlainText;
            }

            string domainPattern = @"^(?!-)[A-Za-z0-9-]+(\.[A-Za-z]{2,})+$";
            if (Regex.IsMatch(text, domainPattern))
            {
                return UriType.WithoutProtocol;
            }
            return UriType.PlainText;
        }
    }
}
