using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            Effect effect = App.settings.BackgroundEffect;
            if (effect == Effect.Mica && MicaController.IsSupported())
            {
                window.SystemBackdrop = new MicaBackdrop();
            }
            else if (effect == Effect.MicaAlt && MicaController.IsSupported())
            {
                window.SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.BaseAlt };
            }
            else if (effect == Effect.Acrylic)
            {
                window.SystemBackdrop = new DesktopAcrylicBackdrop();
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
                appearance = appearance == "Default" ? "UseDefaultAppMode" : appearance;
                window.AppWindow.TitleBar.PreferredTheme = Enum.Parse<TitleBarTheme>(appearance);
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

        public static WindowId GetWindowId(this UIElement element)
        {
            Window window = App.GetWindowForElement(element);
            return window.AppWindow.Id;
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

        public static async Task<string> GetBingImageUrlAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync("https://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1");
            var json = JsonDocument.Parse(response);
            return "https://cn.bing.com" + json.RootElement.GetProperty("images")[0].GetProperty("url").GetString();
        }

        public static void Search(string text, MainWindow mainWindow)
        {
            UriType uriType = text.DetectUri();
            if (uriType == UriType.WithProtocol)
            {
                Navigate(text, mainWindow);
            }
            else if (uriType == UriType.WithoutProtocol)
            {
                Navigate("https://" + text, mainWindow);
            }
            else if (File.Exists(text))
            {
                FileInfo fileInfo = new(text);
                string ext = fileInfo.Extension;
                if (Info.LanguageDict.TryGetValue(ext, out var _))
                {
                    mainWindow.AddNewTab(new TextFilePage(fileInfo), fileInfo.Name);
                }
                else if (Info.ImageDict.TryGetValue(ext, out var _))
                {
                    mainWindow.AddNewTab(new ImageViewer(fileInfo), fileInfo.Name);
                }
                else
                {
                    Navigate(text, mainWindow);
                }
            }
            else
            {
                Navigate(Info.SearchEngineList.First(x => x.Name == App.settings.SearchEngine).Uri + text, mainWindow);
            }
        }

        public static void Navigate(string site, MainWindow mainWindow)
        {
            Uri uri = new(site);
            if ((mainWindow.TabView.SelectedItem != null) && (mainWindow.SelectedItem is WebViewPage webviewPage))
            {
                webviewPage.WebView2.Source = uri;
            }
            else
            {
                mainWindow.AddNewTab(new WebViewPage(uri));
            }
        }
    }
}
