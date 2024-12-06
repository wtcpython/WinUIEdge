using Edge.Data;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Windows.Storage;


namespace Edge
{
    public partial class App : Application
    {
        public static List<MainWindow> mainWindows = [];
        public static string LatestVersion = null;
        public static JsonNode settings;
        public static WebView2 webView2 = new();
        public static ObservableCollection<WebViewHistory> Histories = [];

        public App()
        {
            this.InitializeComponent();
            EnsureWebView2Async();
        }

        public async void EnsureWebView2Async()
        {
            await webView2.EnsureCoreWebView2Async();
        }

        public static MainWindow CreateNewWindow()
        {
            MainWindow window = new();
            window.AddHomePage();
            window.Closed += (sender, e) =>
            {
                mainWindows.Remove(window);
                File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "/settings.json", settings.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
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

        public static CoreWebView2 GetCoreWebView2(UIElement element)
        {
            WebViewPage page = GetWindowForElement(element).SelectedItem as WebViewPage;
            return page.CoreWebView2;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            string path = Info.CheckUserSettingData();
            settings = JsonNode.Parse(File.ReadAllText(path));

            m_window = CreateNewWindow();
            m_window.Activate();

            AppNotificationManager notificationManager = AppNotificationManager.Default;
            notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;
            notificationManager.Register();

            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var activationKind = activatedArgs.Kind;
            if (activationKind != ExtendedActivationKind.AppNotification)
            {
                OverlappedPresenter presenter = m_window.AppWindow.Presenter as OverlappedPresenter;
                presenter.Restore(true);
            }
            else
            {
                HandleNotification((AppNotificationActivatedEventArgs)activatedArgs.Data);
            }
        }

        private void HandleNotification(AppNotificationActivatedEventArgs args)
        {
            var dispatcherQueue = m_window?.DispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            dispatcherQueue.TryEnqueue(delegate
            {
                switch (args.Arguments["UpdateAppRequest"])
                {
                    case "ReleaseWebsitePage":
                        mainWindows[0].AddNewTab(new WebViewPage() { WebUri = "https://github.com/wtcpython/WinUIEdge/releases/latest/" });
                        break;
                    case "DownloadApp":
                        mainWindows[0].AddNewTab(new WebViewPage() { WebUri = $"https://github.com/wtcpython/WinUIEdge/releases/latest/download/Edge_{LatestVersion}_x64.7z" });
                        break;
                }
            });
        }

        private void NotificationManager_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleNotification(args);
        }

        private static Window m_window;
    }
}
