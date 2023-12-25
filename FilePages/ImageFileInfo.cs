using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Edge
{
    public class ImageFileInfo(ImageProperties properties,
        StorageFile imageFile,
        string name,
        string type)
    {
        public StorageFile ImageFile { get; } = imageFile;

        public ImageProperties ImageProperties { get; } = properties;

        public async Task<BitmapImage> GetImageSourceAsync()
        {
            using IRandomAccessStream fileStream = await ImageFile.OpenReadAsync();

            // Create a bitmap to be the image source.
            BitmapImage bitmapImage = new();
            bitmapImage.SetSource(fileStream);

            return bitmapImage;
        }

        public async Task<BitmapImage> GetImageThumbnailAsync()
        {
            StorageItemThumbnail thumbnail =
                await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);
            // Create a bitmap to be the image source.
            BitmapImage bitmapImage = new();
            bitmapImage.SetSource(thumbnail);
            thumbnail.Dispose();

            return bitmapImage;
        }

        public string ImageName { get; } = name;

        public string ImageType { get; } = type;

        public string ImageDimensions => $"{ImageProperties.Width} x {ImageProperties.Height}";

        public uint ImagePixelWidth => ImageProperties.Width;

        public uint ImagePixelHeight => ImageProperties.Height;

        public uint ImageRating = properties.Rating;
    }
}