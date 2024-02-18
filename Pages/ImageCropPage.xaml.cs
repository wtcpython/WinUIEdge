using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;


namespace Edge
{
    public sealed partial class ImageCropPage : Page
    {
        public string filePath;

        public string fileName;

        public int Angle = 0;

        public ImageCropPage(string filepath)
        {
            this.InitializeComponent();
            filePath = filepath;
            imageNameBlock.Text = filePath;
            GetItemsAsync();
        }

        private async void GetItemsAsync()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);

            await imageCropper.LoadImageFromFile(file);
        }

        public static ThumbPlacement ConvertStringToThumbPlacement(string placement) => placement switch
        {
            "All" => ThumbPlacement.All,
            "Corners" => ThumbPlacement.Corners,
            _ => throw new NotImplementedException(),
        };

        public static CropShape ConvertStringToCropShape(string cropshape) => cropshape switch
        {
            "圆形" => CropShape.Circular,
            "矩形" => CropShape.Rectangular,
            _ => throw new NotImplementedException(),
        };

        public static double? ConvertStringToAspectRatio(string ratio) => ratio switch
        {
            "任意" => null,
            "1:1" => 1,
            "16:9" => 16d / 9d,
            "9:16" => 9d / 16d,
            "4:3" => 4d / 3d,
            "3:2" => 3d / 2d,
            _ => throw new NotImplementedException(),
        };

        private void ThumbPlacementChanged(object sender, SelectionChangedEventArgs e)
        {
            imageCropper.ThumbPlacement = ConvertStringToThumbPlacement((string)(thumbPlaceBox.SelectedItem as ComboBoxItem).Content);
        }

        private void CropShapeChanged(object sender, SelectionChangedEventArgs e)
        {
            imageCropper.CropShape = ConvertStringToCropShape((string)(cropShapeBox.SelectedItem as ComboBoxItem).Content);
        }

        private void AspectRatioChanged(object sender, SelectionChangedEventArgs e)
        {
            imageCropper.AspectRatio = ConvertStringToAspectRatio((string)(aspectRatioBox.SelectedItem as ComboBoxItem).Content);
        }

        private async void SaveCropImage(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            FileSavePicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = fileName + " 裁剪",
                FileTypeChoices =
                {
                    { "PNG 文件", new List<string> { ".png" } },
                    { "JPEG 文件", new List<string> { ".jpg" } }
                }
            };

            IntPtr hwnd = App.GetWindowHandle(this);
            InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile imageFile = await picker.PickSaveFileAsync();
            if (imageFile != null)
            {
                var bitmapFileFormat = imageFile.FileType.ToLower() switch
                {
                    ".jpg" => BitmapFileFormat.Jpeg,
                    _ => BitmapFileFormat.Png,
                };
                using var fileStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None);
                await imageCropper.SaveAsync(fileStream, bitmapFileFormat);
            }
        }

        private void ResetCropImage(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            imageCropper.Reset();
        }
    }
}