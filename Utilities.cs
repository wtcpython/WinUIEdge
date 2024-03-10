using Edge.Data;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System;
using System.IO;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace Edge.Utilities;

public static class Utilities
{
    public static void SetBackdrop(this Window window)
    {
        string effect = App.settings["WindowEffect"].ToString();
        if (effect == Info.WindowEffectList[0])
        {
            window.SystemBackdrop = new MicaBackdrop();
        }
        else if (effect == Info.WindowEffectList[1])
        {
            window.SystemBackdrop = new MicaBackdrop()
            {
                Kind = MicaKind.BaseAlt
            };
        }
        else if (effect == Info.WindowEffectList[2])
        {
            window.SystemBackdrop = new DesktopAcrylicBackdrop();
        }
        else window.SystemBackdrop = null;
    }

    public static IntPtr GetWindowHandle(this Window window)
    {
        return WindowNative.GetWindowHandle(window);
    }

    public static IntPtr GetWindowHandle(this UIElement element)
    {
        Window window = App.GetWindowForElement(element);
        return window.GetWindowHandle();
    }

    public static async Task<bool> ShowMsgDialog(this XamlRoot xamlRoot, string title, string content, string closeButtonText, string primaryButtonText = null)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            Content = content,
            CloseButtonText = closeButtonText,
            PrimaryButtonText = primaryButtonText,
            XamlRoot = xamlRoot,
            RequestedTheme = Enum.Parse<ElementTheme>(App.settings["Appearance"].ToString())
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public static async Task<StorageFile> SaveFile(string suggestFile, IntPtr hwnd)
    {
        FileInfo fileInfo = new(suggestFile);
        FileSavePicker picker = new()
        {
            SuggestedStartLocation = PickerLocationId.Downloads,
            SuggestedFileName = fileInfo.Name,
            FileTypeChoices = { { fileInfo.Extension, [fileInfo.Extension] } }
        };

        InitializeWithWindow.Initialize(picker, hwnd);

        return await picker.PickSaveFileAsync();
    }
}