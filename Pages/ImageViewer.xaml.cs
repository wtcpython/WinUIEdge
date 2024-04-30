using CommunityToolkit.Common;
using Edge.Utilities;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.System;


namespace Edge
{
    public sealed partial class ImageViewer : Page
    {
        private double zoomFactor = 1.0;

        public FileInfo fileInfo;

        public BitmapImage source;

        private int Angle = 0;

        public Dictionary<string, string> dict = [];

        public ImageViewer(string filepath)
        {
            this.InitializeComponent();
            fileInfo = new(filepath);
            source = new(new Uri(filepath));
            image.Source = source;
            source.ImageOpened += ImageOpened;
        }

        private async void ImageOpened(object sender, RoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(fileInfo.FullName);
            imageNameBlock.Text = file.Name;
            dict["文件名称"] = file.Name;
            dict["文件类型"] = file.DisplayType;
            dict["像素大小"] = $"{source.PixelWidth} x {source.PixelHeight}";
            dict["文件大小"] = Converters.ToFileSizeString(fileInfo.Length);
            view.ItemsSource = dict;
            fileLocation.Text = fileInfo.FullName;
        }

        private async void ImageDeleteRequest(object sender, RoutedEventArgs e)
        {
            bool deleted = await App.GetWindowForElement(this).Content.XamlRoot.ShowMsgDialog(
                "文件删除确认", $"是否要删除文件 {fileInfo.FullName} ?", "取消", "确定");
            if (deleted) fileInfo.Delete();
        }

        private void ImageRotateRequest(object sender, RoutedEventArgs e)
        {
            Angle = (Angle + 180) % 360;

            RotateTransform rotateTransform = new()
            {
                CenterX = image.Width / 2,
                CenterY = image.Height / 2,
                Angle = Angle
            };
            image.RenderTransform = rotateTransform;
        }

        private void OpenFileLocation(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fileInfo.FullName}\"");
        }

        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(sender as ScrollViewer);

            if (e.KeyModifiers == VirtualKeyModifiers.Control)
            {
                double delta = pointerPoint.Properties.MouseWheelDelta / 2400.0;
                if (zoomFactor < 5.0 && zoomFactor > 0.2)
                {
                    zoomFactor += delta;
                    image.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                    image.RenderTransform = new CompositeTransform() { ScaleX = zoomFactor, ScaleY = zoomFactor };
                }
            }
        }
    }
}