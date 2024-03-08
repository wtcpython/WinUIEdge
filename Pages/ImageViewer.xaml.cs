using CommunityToolkit.Common;
using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;


namespace Edge
{
    public sealed partial class ImageViewer : Page
    {
        public string filePath;

        public string fileName;

        public int Angle = 0;

        public Dictionary<string, string> dict = [];

        public ImageViewer(string filepath)
        {
            this.InitializeComponent();
            filePath = filepath;
            GetItemsAsync();
        }

        private async void GetItemsAsync()
        {
            // 获取文件大小
            long ImageFileSize = new FileInfo(filePath).Length;

            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
            
            // Load Image
            ImageProperties properties = await file.Properties.GetImagePropertiesAsync();

            ItemImage.Source = new BitmapImage(new Uri(filePath));

            imageNameBlock.Text = file.Name;
            dict["文件名称"] = file.Name;
            dict["文件类型"] = file.DisplayType;
            dict["像素大小"] = $"{properties.Width} x {properties.Height}";
            dict["文件大小"] = Converters.ToFileSizeString(ImageFileSize);
            view.ItemsSource = dict;
            fileLocation.Text = filePath;

            imageRating.Value = properties.Rating;
        }

        private async void ImageDeleteRequest(object sender, RoutedEventArgs e)
        {
            bool deleted = await Dialog.ShowMsgDialog(
                App.GetWindowForElement(this).Content.XamlRoot,
                "文件删除确认", $"是否要删除文件 {filePath} ?", "取消", "确定");
            if (deleted)
            {
                FileInfo fileInfo = new(filePath);
                fileInfo.Delete();
            }
        }

        private void ImageRotateRequest(object sender, RoutedEventArgs e)
        {
            Angle = (Angle + 180) % 360;

            RotateTransform rotateTransform = new()
            {
                CenterX = ItemImage.Width / 2,
                CenterY = ItemImage.Height / 2,
                Angle = Angle
            };
            ItemImage.RenderTransform = rotateTransform;
        }

        private void ScrollViewer_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ZoomToFactor(1);
            }
            else
            {
                scrollViewer.ZoomToFactor(2);
                scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
                scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
            }
        }

        private void OpenFileLocation(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", filePath);
        }
    }
}