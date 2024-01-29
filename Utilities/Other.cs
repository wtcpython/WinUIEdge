using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System;

namespace Edge.Utilities
{
    public static class Other
    {
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

        public static string FormatFileSize(long size)
        {
            if (size < 1024) return size.ToString() + "B";
            else if (size < 1024 * 1024) return (size / 1024).ToString("0.0") + "KiB";
            else if (size < 1024 * 1024 * 1024) return (size / (1024 * 1024)).ToString("0.0") + "MiB";
            else return (size / (1024 * 1024 * 1024)).ToString("0.0") + "GiB";
        }

        public static async Task OpenWebsiteUri(string uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }
    }
}