using Edge.Data;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Edge.Utilities;

public static class Utilities
{
    public static void SetBackdrop(this Window window)
    {
        if (App.settings["ShowMicaIfEnabled"].ToObject<bool>())
        {
            if (MicaController.IsSupported())
            {
                window.SystemBackdrop = new MicaBackdrop();
            }
            else window.SystemBackdrop = new DesktopAcrylicBackdrop();
        }
        else window.SystemBackdrop = null;
    }

    public static void SetThemeColor(this Window window)
    {
        string appearance = App.settings["Appearance"].ToString();
        if (window.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Enum.Parse<ElementTheme>(appearance);
            window.AppWindow.TitleBar.ButtonForegroundColor =
                rootElement.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
        }
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


    public static string ToGlyph(this string name)
    {
        return name switch
        {
            "back" => "\ue72b",
            "forward" => "\ue72a",
            "reload" => "\ue72c",
            "saveAs" => "\ue792",
            "print" => "\ue749",
            "share" => "\ue72d",
            "emoji" => "\ue899",
            "undo" => "\ue7a7",
            "redo" => "\ue7a6",
            "cut" => "\ue8c6",
            "copy" => "\ue8c8",
            "paste" => "\ue77f",
            "openLinkInNewWindow" => "\ue737",
            "copyLinkLocation" => "\ue71b",
            _ => string.Empty,
        };
    }
}