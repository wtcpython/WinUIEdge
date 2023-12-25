using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.Storage.FileProperties;


namespace Edge
{
    public sealed partial class ImageViewer : Page
    {
        public string filePath;
        public ImageViewer(string filepath, string fileType)
        {
            this.InitializeComponent();
            filePath = filepath;
            GetItemsAsync();
        }

        private async void GetItemsAsync()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
            uint h = 0;
            uint ImageFileSize = GetCompressedFileSize(filePath, ref h);

            // Load Image
            ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
            ImageFileInfo imageInfo = new(
                properties, file, file.DisplayName, file.DisplayType);

            // Set Image Information
            ItemImage.Source = await imageInfo.GetImageThumbnailAsync();

            imageName.Text = imageInfo.ImageName;
            imageType.Text = imageInfo.ImageType;
            imagePixel.Text = imageInfo.ImageDimensions;
            imageSize.Text = Utils.ConvertBytesToString(ImageFileSize);

            imageRating.Value = imageInfo.ImageRating;
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetCompressedFileSize(string fileName, ref uint fileSizeHigh);
    }
}