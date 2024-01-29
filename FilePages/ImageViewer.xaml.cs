using Edge.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
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

            imageNameBlock.Text = filePath;
            imageName.Text = fileName = file.DisplayName;
            imageType.Text = file.DisplayType;
            imagePixel.Text = $"{properties.Width} x {properties.Height}";
            imageSize.Text = Other.FormatFileSize(ImageFileSize);

            imageRating.Value = properties.Rating;
        }

        private async void FileNameChanged(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (fileName != imageName.Text)
                {
                    FileInfo fileInfo = new(filePath);
                    string fileExt = fileInfo.Extension;
                    bool changed = await Dialog.ShowMsgDialog("文件名称变更确认", $"是否要将名称从 {fileName + fileExt} 更改为 {imageName.Text + fileExt} ?", "取消", "确定");
                    if (changed)
                    {
                        fileInfo.MoveTo(fileInfo.Directory + "\\" + imageName.Text + fileExt);
                    }
                    else
                    {
                        imageName.Text = fileName;
                    }
                }
            }
        }

        private async void ImageDeleteRequest(object sender, RoutedEventArgs e)
        {
            bool deleted = await Dialog.ShowMsgDialog("文件删除确认", $"是否要删除文件 {filePath} ?", "取消", "确定");
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

        private void ImageCropRequest(object sender, RoutedEventArgs e)
        {
            (App.Window as MainWindow).AddNewTab(new ImageCropPage(filePath));
        }
    }
}