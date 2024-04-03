using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace Edge
{
    public sealed partial class AboutItem : Page
    {
        public string appVersion = Package.Current.Id.Version.ToFormattedString();
        public string browserVersion = App.webView2.CoreWebView2.Environment.BrowserVersionString;

        public AboutItem()
        {
            this.InitializeComponent();
            this.Loaded += CheckAppVersion;
        }

        private async void CheckAppVersion(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.LatestVersion == null)
                {
                    Octokit.GitHubClient client = new(new Octokit.ProductHeaderValue("Edge"));
                    App.LatestVersion = (await client.Repository.GetAllTags("wtcpython", "WinUIEdge"))[0].Name[1..];
                }
                if (App.LatestVersion.CompareTo(appVersion) > 0)
                {
                    var builder = new AppNotificationBuilder()
                        .AddText($"发现新版本：{App.LatestVersion}，是否要更新？\n当前版本：{appVersion}")
                        .AddArgument("UpdateAppRequest", "ReleaseWebsitePage")
                        .AddButton(new AppNotificationButton("确定")
                            .AddArgument("UpdateAppRequest", "DownloadApp"))
                        .AddButton(new AppNotificationButton("取消"));

                    var notificationManager = AppNotificationManager.Default;
                    notificationManager.Show(builder.BuildNotification());
                }
            }
            catch (Octokit.RateLimitExceededException) { }
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
            mainWindow.AddNewTab(new WebViewPage() { WebUri = "https://github.com/wtcpython/WinUIEdge" });
        }
    }
}
 