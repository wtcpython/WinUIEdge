using Edge.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Edge
{
    public sealed partial class TextFilePage : Page
    {
        public string file;

        public EncodingInfo[] encodeList;

        public string DefaultFontFamily = "Consolas";

        public int DefaultFontSize = 14;

        public string typeName;

        public TextFilePage(string filepath)
        {
            this.InitializeComponent();

            file = filepath;
            string ext = Path.GetExtension(filepath);
            typeName = Info.LanguageDict.RootElement.GetProperty(ext).ToString();

            // 加载编码列表
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encodeList = Encoding.GetEncodings();
            EncodingComboBox.ItemsSource = encodeList;
            EncodingComboBox.SelectedItem = encodeList.Last();

            // 初始化UI 数据
            FullPath.Text = file;
            TypeName.Text = typeName;

            block.FontFamily = new FontFamily(DefaultFontFamily);
            block.FontSize = DefaultFontSize;
        }

        public string GetEOF(string content)
        {
            if (content.Contains("\r\n"))
            {
                return "CRLF";
            }
            else if (content.Contains('\r'))
            {
                return "CR";
            }
            else if (content.Contains('\n'))
            {
                return "LF";
            }
            else
            {
                return "UnKnown";
            }
        }

        private void EncodeTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            Encoding encoding = encodeList[(sender as ComboBox).SelectedIndex].GetEncoding();
            // 分析文件信息，包含文本内容，文本长度，文本行尾序列和文本编码格式
            StreamReader reader;
            if (encoding == Encoding.Default)
            {
                reader = new StreamReader(file, Encoding.Default, true);
            }
            else
            {
                reader = new StreamReader(file, encoding);
            }

            string content = reader.ReadToEnd();
            LengthInfo.Text = $"共 {content.Length} 个字符";
            EOF.Text = GetEOF(content);

            SetHighlightContent(content);
        }

        private void SetHighlightContent(string content)
        {
            if (!Info.HighlightKeyWords.TryGetProperty(typeName, out var property))
            {
                block.Text = content;
                return;
            }
            IEnumerable<string> keywords = property.EnumerateArray().ToList().Select(x => x.ToString());

            string pattern = @"\b(?:" + string.Join("|", keywords) + @")\b";
            Regex regex = new(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(content);

            int lastEnd = 0;
            foreach (Match match in matches)
            {
                int start = match.Index;
                int length = match.Length;

                if (lastEnd != start)
                {
                    Run nonKeywordRun = new() { Text = content.Substring(lastEnd, start - lastEnd) };
                    block.Inlines.Add(nonKeywordRun);
                }

                Run keywordRun = new()
                {
                    Text = content.Substring(start, length),
                    Foreground = new SolidColorBrush(Colors.DarkBlue)
                };
                block.Inlines.Add(keywordRun);

                lastEnd = start + length;
            }

            if (lastEnd < content.Length)
            {
                Run remainingTextRun = new() { Text = content.Substring(lastEnd) };
                block.Inlines.Add(remainingTextRun);
            }
        }
    }
}