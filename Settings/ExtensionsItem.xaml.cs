using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Web.WebView2.Core;

namespace Edge
{
    public class ExtensionInfo
    {
        public string Name;
        public string Id;
        public bool IsEnabled;
        public string OptionUri;
    }

    public sealed partial class ExtensionsItem : Page
    {
        public static readonly ObservableCollection<ExtensionInfo> Extensions = [];

        public static Dictionary<string, string> ExtensionOptionUriSuffixes = new()
        {
            { "暴力猴", "/options/index.html#settings" }
        };

        public static Brush TextFillColorDisabledBrush = null;

        public ExtensionsItem()
        {
            this.InitializeComponent();
            InitializeExtensionsCollection();
            extensionsList.ItemsSource = Extensions;
            setToggleInjectExtensionsStore.IsOn = App.settings.InjectExtensionsStore;
            microsoftEdgeExtensionsHome.IsClickEnabled = setToggleInjectExtensionsStore.IsOn;
            if (TextFillColorDisabledBrush == null)
            {
                TextFillColorDisabledBrush = (Brush)Application.Current.Resources["TextFillColorDisabledBrush"];
            }
            if (setToggleInjectExtensionsStore.IsOn)
            {
                microsoftEdgeExtensionsHomeHeader.ClearValue(TextBlock.ForegroundProperty);
            }
            else
            {
                microsoftEdgeExtensionsHomeHeader.Foreground = TextFillColorDisabledBrush;
            }
        }

        public static async void InitializeExtensionsCollection()
        {
            IReadOnlyList<CoreWebView2BrowserExtension> extensions = await App.CoreWebView2Profile.GetBrowserExtensionsAsync();
            foreach (var extension in extensions)
            {
                if (Extensions.All(x => x.Id != extension.Id))
                {
                    string optionUriSuffix = ExtensionOptionUriSuffixes.GetValueOrDefault(extension.Name, null);
                    Extensions.Add(new ExtensionInfo() {
                        Name = extension.Name,
                        Id = extension.Id,
                        IsEnabled = extension.IsEnabled,
                        OptionUri = optionUriSuffix == null ? null : $"chrome-extension://{extension.Id}{optionUriSuffix}"
                    });
                }
            }
        }

        private void AddExtension(object sender, RoutedEventArgs e)
        {
            ExtensionsAddAsync(sender);
        }

        public static async void ExtensionsAddAsync(object sender)
        {
            var senderButton = sender as Button;
            if (senderButton == null)
            {
                return;
            }
            senderButton.IsEnabled = false;
            FolderPicker openPicker = new();
            MainWindow window = App.GetWindowForElement(senderButton);
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add("*");
            openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                try
                {
                    CoreWebView2BrowserExtension extension = await App.CoreWebView2Profile.AddBrowserExtensionAsync(folder.Path);
                    string optionUriSuffix = ExtensionOptionUriSuffixes.GetValueOrDefault(extension.Name, null);
                    Extensions.Add(new ExtensionInfo() {
                        Name = extension.Name,
                        Id = extension.Id,
                        IsEnabled = extension.IsEnabled,
                        OptionUri = optionUriSuffix == null ? null : $"chrome-extension://{extension.Id}{optionUriSuffix}"
                    });
                    await new ContentDialog()
                    {
                        Content = $"{extension.Name} 已添加到 WinUIEdge",
                        XamlRoot = senderButton.XamlRoot,
                        CloseButtonText = "好的",
                        DefaultButton = ContentDialogButton.Close
                    }
                    .ShowAsync();
                }
                catch (Exception exception)
                {
                    await new ContentDialog()
                    {
                        Content = $"加载扩展失败：{exception}",
                        XamlRoot = senderButton.XamlRoot,
                        CloseButtonText = "好的",
                        DefaultButton = ContentDialogButton.Close
                    }
                    .ShowAsync();
                }
            }
            senderButton.IsEnabled = true;
        }

        private void ToggleExtension(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch { DataContext: ExtensionInfo extensionInfo })
            {
                ExtensionsToggleEnabledAsync(extensionInfo, this.XamlRoot);
            }
        }

        public static async void ExtensionsToggleEnabledAsync(ExtensionInfo extensionInfo, XamlRoot xamlRoot)
        {
            IReadOnlyList<CoreWebView2BrowserExtension> extensions = await App.CoreWebView2Profile.GetBrowserExtensionsAsync();
            bool found = false;
            foreach (CoreWebView2BrowserExtension extension in extensions)
            {
                if (extension.Id == extensionInfo.Id)
                {
                    try
                    {
                        await extension.EnableAsync(!extension.IsEnabled);
                    }
                    catch (Exception exception) {
                        await new ContentDialog() {
                            Content = $"开关扩展失败，原因: {exception}",
                            XamlRoot = xamlRoot,
                            CloseButtonText = "好的",
                            DefaultButton = ContentDialogButton.Close
                        }
                        .ShowAsync();
                    }
                    extensionInfo.IsEnabled = extension.IsEnabled;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Extensions.Remove(extensionInfo);
                await new ContentDialog()
                {
                    Content = "开关失败，原因：未找到该扩展",
                    XamlRoot = xamlRoot,
                    CloseButtonText = "好的",
                    DefaultButton = ContentDialogButton.Close
                }
                .ShowAsync();
            }
        }

        private void RemoveExtension(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: ExtensionInfo extensionInfo }) {
                ExtensionsRemoveAsync(extensionInfo, this.XamlRoot);
            }
        }


        public static async void ExtensionsRemoveAsync(ExtensionInfo extensionInfo, XamlRoot xamlRoot)
        {
            if (await new ContentDialog()
                {
                    Content = $"是否从 WinUIEdge 中删除 \"{extensionInfo.Name}\"？",
                    PrimaryButtonText = "删除",
                    SecondaryButtonText = "取消",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = xamlRoot
                }
                .ShowAsync() == ContentDialogResult.Primary)
            {
                IReadOnlyList<CoreWebView2BrowserExtension> extensions = await App.CoreWebView2Profile.GetBrowserExtensionsAsync();
                bool found = false;
                foreach (CoreWebView2BrowserExtension extension in extensions) {
                    if (extension.Id == extensionInfo.Id)
                    {
                        try
                        {
                            await extension.RemoveAsync();
                            Extensions.Remove(extensionInfo);
                        }
                        catch (Exception exception)
                        {
                            await new ContentDialog()
                            {
                                Content = $"移除失败，原因：{exception}",
                                XamlRoot = xamlRoot,
                                CloseButtonText = "好的",
                                DefaultButton = ContentDialogButton.Close
                            }
                            .ShowAsync();
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Extensions.Remove(extensionInfo);
                    await new ContentDialog()
                    {
                        Content = "移除失败，原因：未找到该扩展",
                        XamlRoot = xamlRoot,
                        CloseButtonText = "好的",
                        DefaultButton = ContentDialogButton.Close
                    }
                    .ShowAsync();
                }
            }
        }

        private void OpenExtensionOption(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: ExtensionInfo { OptionUri: not null } extensionInfo }) {
                MainWindow mainWindow = App.GetWindowForElement(this);
                mainWindow.AddNewTab(new WebViewPage(new Uri(extensionInfo.OptionUri)));
            }
        }

        void ToggleInjectExtensionsStore(object sender, RoutedEventArgs e)
        {
            if (App.settings.InjectExtensionsStore != setToggleInjectExtensionsStore.IsOn)
            {
                App.settings.InjectExtensionsStore = setToggleInjectExtensionsStore.IsOn;
                microsoftEdgeExtensionsHome.IsClickEnabled = setToggleInjectExtensionsStore.IsOn;
                if (setToggleInjectExtensionsStore.IsOn)
                {
                    microsoftEdgeExtensionsHomeHeader.ClearValue(TextBlock.ForegroundProperty);
                }
                else
                {
                    microsoftEdgeExtensionsHomeHeader.Foreground = TextFillColorDisabledBrush;
                }
            }
        }

        private void OpenMicrosoftEdgeExtensionsHome(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage(new Uri("https://microsoftedge.microsoft.com/addons/Microsoft-Edge-Extensions-Home")));
        }
    }
}
