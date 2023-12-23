using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;

namespace Edge
{
    public sealed partial class AboutItem : Page
    {
        private static PackageVersion ver = Package.Current.Id.Version;
        public string appVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
        public AboutItem()
        {
            this.InitializeComponent();
            edgeVersionCard.Description = $"°æ±¾£º{WebViewPage.chromiumVersion}";
            appVersionCard.Description = $"°æ±¾£º{appVersion}";
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

        private void TryCopyChromiumVersion(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            CopyText(WebViewPage.chromiumVersion);
        }

        private void TryCopyAppVersion(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            CopyText(appVersion);
        }

        private void OpenMSEdgeWebsite(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Utils.OpenWebsite(new Uri("https://microsoft.com/zh-cn/edge"));
        }

        private void OpenRepoWebsite(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Utils.OpenWebsite(new Uri("https://github.com/wtcpython"));
        }
    }
}
 