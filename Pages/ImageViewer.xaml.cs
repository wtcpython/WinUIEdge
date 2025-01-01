using CommunityToolkit.Common;
using Edge.Utilities;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using System;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.System.UserProfile;


namespace Edge
{
    public sealed partial class ImageViewer : Page
    {
        public FileInfo fileInfo;
        public BitmapImage source;

        private PrintManager printManager;
        private PrintDocument printDocument;
        private IPrintDocumentSource printDocumentSource;

        public ImageViewer(string filepath)
        {
            this.InitializeComponent();
            fileInfo = new(filepath);
            source = new(new Uri(filepath));
            image.Source = source;
            source.ImageOpened += ImageOpened;
        }

        private void RegisterPrint()
        {
            IntPtr hwnd = this.GetWindowHandle();
            printManager = PrintManagerInterop.GetForWindow(hwnd);
            printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;

            printDocument = new();
            printDocumentSource = printDocument.DocumentSource;
            printDocument.Paginate += PrintDocument_Paginate;
            printDocument.GetPreviewPage += PrintDocument_GetPreviewPage;
            printDocument.AddPages += PrintDocument_AddPages;
        }

        private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        {
            printDocument.AddPage(image);
            printDocument.AddPagesComplete();
        }

        private void PrintDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDocument.SetPreviewPage(e.PageNumber, image);
        }

        private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        {
            printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask task = args.Request.CreatePrintTask(fileInfo.Name, PrintTaskSourceRequested);
        }

        private void PrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(printDocumentSource);
        }

        private async void PrintImageRequest(object sender, RoutedEventArgs e)
        {
            RegisterPrint();
            if (PrintManager.IsSupported())
            {
                IntPtr hwnd = this.GetWindowHandle();
                await PrintManagerInterop.ShowPrintUIForWindowAsync(hwnd);
            }
        }

        private async void ImageOpened(object sender, RoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(fileInfo.FullName);
            imageNameBlock.Text = file.Name;
            imagePixel.Text = $"{source.PixelWidth} x {source.PixelHeight}";
            imageSize.Text = Converters.ToFileSizeString(fileInfo.Length);
        }

        private async void ImageDeleteRequest(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await deleteDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                fileInfo.Delete();
            }
        }

        private void ImageRotateRequest(object sender, RoutedEventArgs e)
        {
            CompositeTransform transform = image.RenderTransform as CompositeTransform ?? new CompositeTransform();
            transform.Rotation = (transform.Rotation + 90) % 360;
            image.RenderTransform = transform;
        }

        private void OpenFileLocation(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fileInfo.FullName}\"");
        }

        private void Image_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(image).Properties.MouseWheelDelta;

            double scale = 1 + (delta / 1200.0);

            image.RenderTransformOrigin = new Point(0.5, 0.5);

            CompositeTransform transform = image.RenderTransform as CompositeTransform ?? new CompositeTransform();
            transform.ScaleX *= scale;
            transform.ScaleY *= scale;
            image.RenderTransform = transform;
        }

        private void ImageFullScreen(object sender, RoutedEventArgs e)
        {
            AppWindow appWindow = App.GetWindowForElement(this).AppWindow;
            if (appWindow.Presenter.Kind != AppWindowPresenterKind.FullScreen)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            }
            else
            {
                appWindow.SetPresenter(AppWindowPresenterKind.Default);
            }
        }

        private async void SaveImageAs(object sender, RoutedEventArgs e)
        {
            StorageFile storageFile = await Utilities.Utilities.SaveFile(fileInfo.FullName, this.GetWindowHandle());
            fileInfo.CopyTo(storageFile.Path, true);
        }

        private void CopyFilePath(object sender, RoutedEventArgs e)
        {
            DataPackage package = new();
            package.SetText(fileInfo.FullName);
            Clipboard.SetContent(package);
        }

        private async void SetImageAsLockScreen(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(fileInfo.FullName);
                await settings.TrySetLockScreenImageAsync(imageFile);
            }
        }

        private async void SetImageAsWallpaper(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(fileInfo.FullName);
                await settings.TrySetWallpaperImageAsync(imageFile);
            }
        }
    }
}