using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.Storage.FileProperties;


namespace Edge
{
    public sealed partial class ImageViewer : Page
    {
        public ImageViewer(string filepath, string fileType)
        {
            this.InitializeComponent();
            GetItemsAsync(filepath);
        }

        private async void GetItemsAsync(string filepath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(filepath);

            // Load Image
            ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
            ImageFileInfo imageInfo = new(
                properties, file, file.DisplayName, file.DisplayType);

            // Set Image Information
            ItemImage.Source = await imageInfo.GetImageThumbnailAsync();
            imageTitle.Text = imageInfo.ImageTitle;
            imageFileType.Text = imageInfo.ImageFileType;
            imageDimensions.Text = imageInfo.ImageDimensions;
            imageRating.Value = imageInfo.ImageRating;
        }
    }
}