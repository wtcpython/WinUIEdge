using IWshRuntimeLibrary;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;


namespace Edge
{
    public sealed partial class InkFilePage : Page
    {
        string realLocation;
        public InkFilePage(string filepath)
        {
            this.InitializeComponent();

            string ext = Path.GetExtension(filepath);

            WshShell shell = new();
            IWshShortcut shortcut = shell.CreateShortcut(filepath);

            shortcutName.Text = Path.GetFileNameWithoutExtension(shortcut.FullName);

            realLocation = shortcut.TargetPath;
            targetPath.Text = shortcut.TargetPath;
            arguments.Text = shortcut.Arguments;

            workingDirectory.Text = shortcut.WorkingDirectory;

            hotkey.Text = shortcut.Hotkey;

            windowStyle.Text = shortcut.WindowStyle switch
            {
                1 => "常规窗口",
                3 => "最大化",
                7 => "最小化",
                _ => "Unknown"
            };


            if (shortcut.Description != string.Empty)
            {
                description.Text = shortcut.Description;
            }
            else
            {
                description.Text = "无备注";
            }
            LoadIcon(shortcut.TargetPath);
        }

        private async void LoadIcon(string filepath)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(filepath);
                StorageItemThumbnail iconStream = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);
                if (iconStream != null)
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(iconStream);
                    image.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine("Error loading icon: " + ex.Message);
            }
        }

        private void OpenLinkRealLocation(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Process.Start("explorer.exe", $"/select,\"{realLocation}\"");
        }
    }
}
