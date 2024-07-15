using Edge.Data;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
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

            string ext = Path.GetExtension(filepath);
            typeName = Info.LanguageDict[ext].ToString();

            // 加载编码列表
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encodeList = Encoding.GetEncodings();
            fileControl.EncodingItems = encodeList;
            fileControl.SelectionChanged += EncodeTypeChanged;

            // 加载文件信息
            file = filepath;
            AnalyzeFile(file, Encoding.Default);

            // 初始化UI 数据
            fileControl.FullPath = file;
            fileControl.TypeName = typeName;

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
            AnalyzeFile(file, encodeList[(sender as FileControl).EncodingSelectedIndex].GetEncoding());
        }

        private void AnalyzeFile(string filePath, Encoding encoding)
        {
            // 分析文件信息，包含文本内容，文本长度，文本行尾序列和文本编码格式
            StreamReader reader = null;
            if (encoding == Encoding.Default)
            {
                reader = new StreamReader(filePath, Encoding.Default, true);
            }
            else
            {
                reader = new StreamReader(filePath, encoding);
            }

            string content = reader.ReadToEnd();
            fileControl.LengthInfo = $"共 {content.Length} 个字符";
            fileControl.EOF = GetEOF(content);
            fileControl.EncodingSelectedIndex = Array.FindIndex(encodeList, x => x.Name == reader.CurrentEncoding.BodyName);

            SetHighlightContent(content);
        }

        private void SetHighlightContent(string content)
        {
            List<string> keywords;
            if (!Info.HighlightKeyWords.TryGetValue(typeName, out keywords))
            {
                block.Text = content;
                return;
            }

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

        private static Regex BuildRegex(string[] keywords)
        {
            string pattern = @"\b(?:" + string.Join("|", keywords) + @")\b";
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}