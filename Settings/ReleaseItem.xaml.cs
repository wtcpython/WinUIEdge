using CommunityToolkit.Labs.WinUI.MarkdownTextBlock;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace Edge
{
    public class GitHubRelease
    {
        public string name { get; set; }
        public string body { get; set; }
    }

    public sealed partial class ReleaseItem : Page
    {
        public ReleaseItem()
        {
            this.InitializeComponent();
            markdown.Config = new MarkdownConfig();
            SetText();
        }

        public async void SetText()
        {
            List<GitHubRelease> releases = await GetReleasesAsync();
            string contents = string.Empty;
            releases.ForEach(item =>
            {
                string content = "# " + item.name + "\n" + item.body + "\n";
                contents += content;
            });
            markdown.Text = contents;
        }

        public static async Task<List<GitHubRelease>> GetReleasesAsync()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WinUIEdge/1.0");
            var response = await client.GetAsync("https://api.github.com/repos/wtcpython/WinUIEdge/releases");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize(json, JsonContext.Default.ListGitHubRelease);
            }
            return null;
        }
    }
}
