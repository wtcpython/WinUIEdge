using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Edge
{
    public sealed partial class TextFilePage : Page
    {
        public string file;

        public EncodingInfo[] encodeList;

        public string DefaultFontFamily = "Consolas";

        public int DefaultFontSize = 14;

        public List<int> FontSizeList = Enumerable.Range(6, 36).ToList();

        public List<string> FontFamilyList = CanvasTextFormat.GetSystemFontFamilies().ToList();


        public TextFilePage(string filepath, string fileType)
        {
            this.InitializeComponent();
            FontFamilyList.Sort();

            string ext = Path.GetExtension(filepath);
            engine.SetData(ext);
            //if (ext == ".json")
            //{
            //    engine.Content = new JsonFileEngine();
            //}
            //else
            //{
            //    engine.Content = new TextFileEngine();
            //}

            // 加载文件信息
            file = filepath;

            string DefaultEncoding = "utf-8";
            string content = GetFileText(DefaultEncoding);

            // 加载编码列表
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encodeList = Encoding.GetEncodings();
            encodeBox.ItemsSource = encodeList;

            // 初始化UI 数据
            fileNameBlock.Text = file;
            EOFType.Text = GetEOF();
            textType.Text = fileType;
            FontSizeBox.ItemsSource = FontSizeList;
            FontSizeBox.SelectedIndex = FontSizeList.IndexOf(14);
            FontFamilyBox.ItemsSource = FontFamilyList;
            FontFamilyBox.SelectedIndex = FontFamilyList.IndexOf(DefaultFontFamily);

            // 设置编辑器文本
            engine.SetText(content);
            textInfo.Text = $"共 {content.Length} 个字符";

            engine.SetFontFamily(new FontFamily(DefaultFontFamily));
            engine.SetFontSize(DefaultFontSize);

            encodeBox.SelectedIndex = Array.FindIndex(encodeList, x => x.Name == DefaultEncoding);
        }

        public string GetFileText(string encoding)
        {
            using StreamReader reader = new(file, Encoding.GetEncoding(encoding));
            return reader.ReadToEnd();
        }

        private void FontFamilyChanged(object sender, SelectionChangedEventArgs e)
        {
            engine.SetFontFamily(new FontFamily((string)(sender as ComboBox).SelectedItem));
        }

        private void FontSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            engine.SetFontSize((int)(sender as ComboBox).SelectedItem);
        }

        public string GetEOF()
        {
            using StreamReader reader = File.OpenText(file);

            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (c == '\r')
                {
                    c = (char)reader.Read();

                    if (c == '\n') return "CRLF";
                    else return "CR";
                }
                else if (c == '\n') return "LF";
            }
            return "UnKnown";
        }

        private void EncodeTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            string content = GetFileText(encodeList[(sender as ComboBox).SelectedIndex].Name);
            engine.SetText(content);
        }
    }
}