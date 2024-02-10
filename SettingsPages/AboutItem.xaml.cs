using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using System;

namespace Edge
{
    public sealed partial class AboutItem : Page
    {
        private static PackageVersion ver = Package.Current.Id.Version;
        public string appVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";

        public AboutItem()
        {
            this.InitializeComponent();
            edgeVersionCard.Description = $"版本：{WebViewPage.chromiumVersion}";
            appVersionCard.Description = $"版本：{appVersion}";
            this.Loaded += CheckAppVersion;
        }

        private async void CheckAppVersion(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.LatestVersion == null)
                {
                    Octokit.GitHubClient client = new(new Octokit.ProductHeaderValue("Edge"));
                    IReadOnlyList<Octokit.RepositoryTag> tags = await client.Repository.GetAllTags("wtcpython", "WinUIEdge");
                    App.LatestVersion = tags[0].Name[1..];
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
            CopyText(WebViewPage.chromiumVersion);
        }

        private void TryCopyAppVersion(object sender, RoutedEventArgs e)
        {
            CopyText(appVersion);
        }

        private async void OpenMSEdgeWebsite(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://microsoft.com/zh-cn/edge"));
        }

        private async void OpenRepoWebsite(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/wtcpython/WinUIEdge"));
        }
    }
}
 