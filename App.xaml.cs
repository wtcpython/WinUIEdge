using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using Windows.Win32;
using Windows.Win32.Foundation;


namespace Edge
{
    public partial class App : Application
    {
        public static List<MainWindow> mainWindows = [];
        public static string LatestVersion = null; 
        public App()
        {
            this.InitializeComponent();
        }

        public static MainWindow CreateNewWindow()
        {
            MainWindow window = new();
            window.Closed += (sender, e) => mainWindows.Remove(window);
            mainWindows.Add(window);
            return window;
        }

        public static Window GetWindowForElement(UIElement element)
        {
            if (element.XamlRoot != null)
            {
                foreach (Window window in mainWindows)
                {
                    if (element.XamlRoot == window.Content.XamlRoot)
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--disable-features=msSmartScreenProtection");
            Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");

            m_window = CreateNewWindow();
            m_window.Activate();

            AppNotificationManager notificationManager = AppNotificationManager.Default;
            notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;
            notificationManager.Register();

            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var activationKind = activatedArgs.Kind;
            if (activationKind != ExtendedActivationKind.AppNotification)
            {
                ShowWindow(m_window);
            }
            else
            {
                HandleNotification((AppNotificationActivatedEventArgs)activatedArgs.Data);
            }
        }

        private void HandleNotification(AppNotificationActivatedEventArgs args)
        {
            var dispatcherQueue = m_window?.DispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            dispatcherQueue.TryEnqueue(async delegate
            {
                switch (args.Arguments["UpdateAppRequest"])
                {
                    case "ReleaseWebsitePage":
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/wtcpython/WinUIEdge/releases/latest/"));
                        break;
                    case "DownloadApp":
                        await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://github.com/wtcpython/WinUIEdge/releases/latest/download/Edge_{LatestVersion}_x64.7z"));
                        break;
                }
            });
        }

        private void NotificationManager_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleNotification(args);
        }

        public static Window Window { get { return m_window; } }
        private static Window m_window;

        public static void ShowWindow(Window window)
        {
            HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(window);
            PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_RESTORE);
            PInvoke.SetForegroundWindow(hwnd);
        }
    }
}
