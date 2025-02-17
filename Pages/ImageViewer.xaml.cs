using CommunityToolkit.Common;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using System;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.System.UserProfile;


namespace Edge
{
    public sealed partial class ImageViewer : Page
    {
        public FileInfo info;
        public BitmapImage source;

        private PrintManager printManager;
        private PrintDocument printDocument;
        private IPrintDocumentSource printDocumentSource;

        public ImageViewer(FileInfo fileInfo)
        {
            this.InitializeComponent();
            info = fileInfo;
            source = new(new Uri(info.FullName));
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
            PrintTask task = args.Request.CreatePrintTask(info.Name, PrintTaskSourceRequested);
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

        private void ImageOpened(object sender, RoutedEventArgs e)
        {
            imageNameBlock.Text = info.Name;
            imagePixel.Text = $"{source.PixelWidth} x {source.PixelHeight}";
            imageSize.Text = Converters.ToFileSizeString(info.Length);
        }

        private async void ImageDeleteRequest(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await deleteDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                info.Delete();
            }
        }

        private void ImageRotateRequest(object sender, RoutedEventArgs e)
        {
            ImageRotation.Angle = (ImageRotation.Angle + 90) % 360;
        }

        private void OpenFileLocation(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{info.FullName}\"");
        }

        private void Image_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(image).Properties.MouseWheelDelta;
            double scale = 1 + (delta / 1200.0);

            ImageScale.ScaleX *= scale;
            ImageScale.ScaleY *= scale;
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

        private void SaveImageAs(object sender, RoutedEventArgs e)
        {
            string path = Utilities.Win32SaveFile(info.FullName, this.GetWindowHandle());
            if (path != string.Empty)
            {
                info.CopyTo(path, true);
            }
        }

        private void CopyFilePath(object sender, RoutedEventArgs e)
        {
            DataPackage package = new();
            package.SetText(info.FullName);
            Clipboard.SetContent(package);
        }

        private async void SetImageAsLockScreen(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(info.FullName);
                await settings.TrySetLockScreenImageAsync(imageFile);
            }
        }

        private async void SetImageAsWallpaper(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                UserProfilePersonalizationSettings settings = UserProfilePersonalizationSettings.Current;
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(info.FullName);
                await settings.TrySetWallpaperImageAsync(imageFile);
            }
        }
    }
}