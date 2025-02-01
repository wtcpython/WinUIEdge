using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.DataTransfer;

namespace Edge
{
    public sealed partial class AboutItem : Page
    {
        public string appVersion = File.ReadAllText("./Assets/version.txt");
        public string browserVersion = CoreWebView2Environment.GetAvailableBrowserVersionString();

        public AboutItem()
        {
            this.InitializeComponent();
        }

        private async void CheckUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileUri = "https://raw.githubusercontent.com/wtcpython/WinUIEdge/main/Assets/version.txt";
                using HttpClient client = new();
                App.LatestVersion = await client.GetStringAsync(fileUri);

                if (App.LatestVersion.CompareTo(appVersion) > 0)
                {
                    var builder = new AppNotificationBuilder()
                        .AddText($"发现新版本：{App.LatestVersion}，是否要更新？\n当前版本：{appVersion}")
                        .AddArgument("Notification", "LaunchReleaseWebsite")
                        .AddButton(new AppNotificationButton("确定")
                            .AddArgument("Notification", "LaunchReleaseWebsite"))
                        .AddButton(new AppNotificationButton("取消"));

                    var notificationManager = AppNotificationManager.Default;
                    notificationManager.Show(builder.BuildNotification());
                }
            }
            catch (Exception) { }
        }

        private void CopyText(string text)
        {
            DataPackage package = new()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            package.SetText(text);
            Clipboard.SetContent(package);
        }

        private void TryCopyChromiumVersion(object sender, RoutedEventArgs e)
        {
            CopyText(browserVersion);
        }

        private void TryCopyAppVersion(object sender, RoutedEventArgs e)
        {
            CopyText(appVersion);
        }

        private void OpenRepoWebsite(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = App.GetWindowForElement(this);
            mainWindow.AddNewTab(new WebViewPage(new Uri("https://github.com/wtcpython/WinUIEdge")));
        }
    }
}
 