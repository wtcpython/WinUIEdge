using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Edge
{
    public static class Utils
    {
        public static List<string> byteList = ["B", "KB", "MB", "GB", "TB"];

        public static Dictionary<string, string> LanguageTypeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(LoadFile("/Assets/LanguageType.json"))!;
        public static Dictionary<string, string> WebFileTypeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(LoadFile("/Assets/WebFileType.json"))!;
        public static Dictionary<string, string> ImageTypeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(LoadFile("/Assets/ImageType.json"))!;

        public static Dictionary<string, string> UserAgentDictionary = new()
        {
            { "Microsoft Edge", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0" },
            { "Microsoft Edge Legacy", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19042" },
            { "Google Chrome", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36" },
            { "FireFox", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:70.0) Gecko/20100101 Firefox/70.0" },
            { "Internet Explorer 11", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko" },
            { "Internet Explorer 7", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)" },
            { "Safari Mac", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Safari/605.1.15" }
        };

        public static JsonData data = JsonSerializer.Deserialize<JsonData>(LoadFile("/Assets/DefaultSettings.json"))!;

        public static string LoadFile(string filePath)
        {
            string fullPath = Package.Current.InstalledPath + filePath;
            using FileStream stream = new(fullPath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }

        public static void CreateNewWindow(object content)
        {
            Window window = new();
            Frame frame = new()
            {
                Content = content
            };
            window.Content = frame;
            window.Activate();
        }

        public static string ConvertBytesToString(long bytes)
        {
            int index = (int)Math.Log(bytes, 1024);
            double res = Math.Round(bytes / Math.Pow(1024, index), 2);
            return $"{res} {byteList[index]}";
        }

        public static async void ShowContentDialog(string title, string content, string closeButtonText, XamlRoot xamlRoot)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText,
                XamlRoot = xamlRoot
            };
            await dialog.ShowAsync();
        }

        public static async Task<bool> ShowMultiChoiceDialog(string title, string content, string closeButtonText, string primaryButtonText, XamlRoot xamlRoot)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText,
                PrimaryButtonText = primaryButtonText,
                XamlRoot = xamlRoot
            };
            ContentDialogResult result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        public static async void OpenWebsite(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        public static SystemBackdrop SetBackDrop()
        {
            if (data.WindowEffect == JsonDataList.WindowEffectList[0])
            {
                return new MicaBackdrop();
            }
            else if (data.WindowEffect == JsonDataList.WindowEffectList[1])
            {
                return new MicaBackdrop()
                {
                    Kind = MicaKind.BaseAlt
                };
            }
            else if (data.WindowEffect == JsonDataList.WindowEffectList[2])
            {
                return new DesktopAcrylicBackdrop();
            }
            return null;
        }
    }
}