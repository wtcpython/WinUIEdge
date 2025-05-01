using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Web.WebView2.Core;


namespace Edge
{
    public partial class App : Application
    {
        public static List<MainWindow> mainWindows = [];
        public static Settings settings;
        public static WebView2 WebView2;
        public static CoreWebView2 CoreWebView2;
        public static CoreWebView2Environment CoreWebView2Environment;
        public static CoreWebView2Profile CoreWebView2Profile;
        public static ObservableCollection<WebViewHistory> Histories = [];
        public static ObservableCollection<DownloadObject> DownloadList = [];
        public static WordSearchEngine searchEngine;
        public static bool NeedRestartEnvironment;

        public App()
        {
            Console.OutputEncoding = Encoding.UTF8;
            this.InitializeComponent();
            searchEngine = new("./Assets/words.txt");
        }

        public async void EnsureWebView2Async() {
            List<string> additionalBrowserArguments = [];
            if (settings.DisableGpu)
            {
                additionalBrowserArguments.Add("--disable-gpu");
            }
            if (settings.DisableBackgroundTimerThrottling) {
                additionalBrowserArguments.Add("--disable-background-timer-throttling");
            }
            CoreWebView2Environment = await CoreWebView2Environment.CreateWithOptionsAsync(
                null,
                null,
                new CoreWebView2EnvironmentOptions() {
                    AreBrowserExtensionsEnabled = true,
                    AdditionalBrowserArguments = string.Join(" ", additionalBrowserArguments)
                });
            CoreWebView2Environment.BrowserProcessExited += BrowserProcessExited;
            WebView2 = new WebView2();
            await WebView2.EnsureCoreWebView2Async(CoreWebView2Environment);
            CoreWebView2 =  WebView2.CoreWebView2;
            CoreWebView2Profile = CoreWebView2.Profile;
        }

        private void BrowserProcessExited(CoreWebView2Environment sender, CoreWebView2BrowserProcessExitedEventArgs args)
        {
            Console.WriteLine($"Browser process exited with exit code {args.BrowserProcessExitKind}");
            if (NeedRestartEnvironment)
            {
                NeedRestartEnvironment = false;
                EnsureWebView2Async();
            }
        }

        public static MainWindow CreateNewWindow()
        {
            MainWindow window = new();
            window.Closed += (sender, e) =>
            {
                mainWindows.Remove(window);
                if (mainWindows.Count == 0)
                {
                    NeedRestartEnvironment = false;
                    File.WriteAllText("./settings.json", JsonSerializer.Serialize(settings, JsonContext.Default.Settings));
                }
                else if (NeedRestartEnvironment && !AnyWebviewPageExists())
                {
                    WebView2.Close();
                }
            };
            mainWindows.Add(window);
            return window;
        }

        public static MainWindow GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (MainWindow window in mainWindows)
                {
                    if (element.XamlRoot == window.Content.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        public static WebView2 GetWebView2(UIElement element)
        {
            WebViewPage page = GetWindowForElement(element).SelectedItem as WebViewPage;
            return page.WebView2;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            settings = Info.LoadSettings();

            EnsureWebView2Async();

            window = CreateNewWindow();

            string[] arguments = Environment.GetCommandLineArgs()[1..];
            if (arguments.Length > 0)
            {
                foreach (string arg in arguments)
                {
                    Utilities.Search(arg, window);
                }
            }
            else
            {
                window.AddHomePage();
            }
            window.Activate();

            AppNotificationManager notificationManager = AppNotificationManager.Default;
            notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;
            notificationManager.Register();

            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var activationKind = activatedArgs.Kind;
            if (activationKind != ExtendedActivationKind.AppNotification)
            {
                OverlappedPresenter presenter = window.AppWindow.Presenter as OverlappedPresenter;
                presenter.Restore(true);
            }
            else
            {
                HandleNotification((AppNotificationActivatedEventArgs)activatedArgs.Data);
            }
        }

        private void HandleNotification(AppNotificationActivatedEventArgs args)
        {
            var dispatcherQueue = window?.DispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            dispatcherQueue.TryEnqueue(async delegate
            {
                switch (args.Arguments["Notification"])
                {
                    case "LaunchReleaseWebsite":
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/wtcpython/WinUIEdge/releases/latest/"));
                        break;
                    case "ChangeStartUri":
                        window.Content.Focus(FocusState.Programmatic);
                        break;
                }
            });
        }

        private void NotificationManager_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleNotification(args);
        }

        private static MainWindow window;

        public static bool AnyWebviewPageExists() {
            foreach (MainWindow mainWindow in App.mainWindows)
            {
                foreach (object tabItem in mainWindow.TabView.TabItems)
                {
                    if (tabItem is TabViewItem { Content: WebViewPage webViewPage } tabViewItem)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
