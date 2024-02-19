using Edge.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Edge.Utilities
{
    public static class Dialog
    {
        public static async Task<bool> ShowMsgDialog(XamlRoot xamlRoot, string title, string content, string closeButtonText, string primaryButtonText = null)
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = content,
                CloseButtonText = closeButtonText,
                PrimaryButtonText = primaryButtonText,
                XamlRoot = xamlRoot,
                RequestedTheme = Enum.Parse<ElementTheme>(Info.data.Appearance)
            };
            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        public static async Task<StorageFile> SaveFile(string suggestFile, IntPtr hwnd)
        {
            FileSavePicker picker = new()
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
                SuggestedFileName = Path.GetFileName(suggestFile),
                FileTypeChoices = { { Path.GetExtension(suggestFile), [Path.GetExtension(suggestFile)] } }
            };

            InitializeWithWindow.Initialize(picker, hwnd);

            return await picker.PickSaveFileAsync();
        }
    }
}