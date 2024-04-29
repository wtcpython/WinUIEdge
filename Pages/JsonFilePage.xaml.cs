using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Edge
{
    public sealed partial class JsonFilePage : Page
    {
        public string file;

        public EncodingInfo[] encodeList;

        public string DefaultFontFamily = "Consolas";

        public int DefaultFontSize = 14;

        public JsonFilePage(string filepath)
        {
            this.InitializeComponent();

            string ext = Path.GetExtension(filepath);

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
            fileControl.TypeName = Info.LanguageDict[ext].ToString();

            view.FontFamily = new FontFamily(DefaultFontFamily);
            view.FontSize = DefaultFontSize;
        }

        public string GetEOF(string content)
        {
            if (content.Contains("\r\n")) return "CRLF";
            else if (content.Contains('\r')) return "CR";
            else if (content.Contains('\n')) return "LF";
            else return "UnKnown";
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
            else reader = new StreamReader(filePath, encoding);
            string content = reader.ReadToEnd();
            LoadTree(view, content);
            fileControl.LengthInfo = $"共 {content.Length} 个字符";
            fileControl.EOF = GetEOF(content);
            fileControl.EncodingSelectedIndex = Array.FindIndex(encodeList, x => x.Name == reader.CurrentEncoding.BodyName);
        }

        public void LoadTree(TreeView obj, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            obj.RootNodes.Clear();

            JContainer json;
            try
            {
                if (content.StartsWith('['))
                {
                    json = JArray.Parse(content);
                    obj.RootNodes.Add(JsonToTree((JArray)json, "Root"));
                }
                else
                {
                    json = JObject.Parse(content);
                    obj.RootNodes.Add(JsonToTree((JObject)json, "Root"));
                }
            }
            catch (JsonReaderException)
            {
                // invalid json
            }
        }

        public TreeViewNode JsonToTree(JArray obj, string nodeName)
        {
            if (obj == null)
                return null;

            string itemCountString = obj.Count.ToString() + " item" + (obj.Count > 1 ? "s" : "");
            TreeViewNode parent = new() { Content = $"{nodeName}:  {itemCountString}" };

            foreach ((JToken token, int index) in obj.Select((value, i) => (value, i)))
            {
                if (token.Type == JTokenType.Object)
                {
                    parent.Children.Add(JsonToTree((JObject)token, $"{nodeName}[{index}]"));
                }
                else if (token.Type == JTokenType.Array)
                {
                    parent.Children.Add(JsonToTree((JArray)token, $"{nodeName}[{index}]"));
                }
                else
                {
                    parent.Children.Add(new TreeViewNode()
                    {
                        Content = $"{nodeName}[{index}]:  {token}"
                    });
                }
            }

            return parent;
        }

        public TreeViewNode JsonToTree(JObject obj, string nodeName)
        {
            if (obj == null)
                return null;

            TreeViewNode parent = new() { Content = $"{nodeName}:  {obj.Count} items" };

            foreach (KeyValuePair<string, JToken> pair in obj)
            {
                if (pair.Value.Type == JTokenType.Object)
                {
                    parent.Children.Add(JsonToTree((JObject)pair.Value, pair.Key));
                }
                else if (pair.Value.Type == JTokenType.Array)
                {
                    parent.Children.Add(JsonToTree((JArray)pair.Value, pair.Key));
                }
                else
                {
                    parent.Children.Add(new TreeViewNode()
                    {
                        Content = $"{pair.Key}:  {pair.Value}"
                    });
                }
            }

            return parent;
        }
    }
}