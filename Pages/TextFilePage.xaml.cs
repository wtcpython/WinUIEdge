using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;


namespace Edge
{
    public sealed partial class TextFilePage : Page
    {
        public string file;

        public EncodingInfo[] encodeList;

        public string DefaultFontFamily = "Consolas";

        public int DefaultFontSize = 14;

        public List<int> FontSizeList = Enumerable.Range(8, 58).Where(x => x % 2 == 0).ToList();

        public List<string> FontNameList = [];


        public TextFilePage(string filepath)
        {
            this.InitializeComponent();
            EnumerateFonts();
            FontNameList.Sort();

            string ext = Path.GetExtension(filepath);
            engine.SetData(ext);

            // 加载文件信息
            file = filepath;

            string content = File.ReadAllText(filepath, Encoding.UTF8);

            // 加载编码列表
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            encodeList = Encoding.GetEncodings();
            encodeBox.ItemsSource = encodeList;

            // 初始化UI 数据
            fileNameBlock.Text = file;
            EOFType.Text = GetEOF();
            textType.Text = Info.LanguageDict[ext].ToString();
            FontSizeBox.ItemsSource = FontSizeList;
            FontSizeBox.SelectedIndex = FontSizeList.IndexOf(14);
            FontFamilyBox.ItemsSource = FontNameList;
            FontFamilyBox.SelectedIndex = FontNameList.IndexOf(DefaultFontFamily);

            // 设置编辑器文本
            engine.SetText(content);
            textInfo.Text = $"共 {content.Length} 个字符";

            engine.SetFontFamily(new FontFamily(DefaultFontFamily));
            engine.SetFontSize(DefaultFontSize);

            encodeBox.SelectedIndex = Array.FindIndex(encodeList, x => x.Name == Encoding.UTF8.BodyName);
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
            string content = File.ReadAllText(file, encodeList[(sender as ComboBox).SelectedIndex].GetEncoding());
            engine.SetText(content);
        }

        private void SearchText(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(engine);
        }

        private void SearchKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ResultNumber.Text = engine.GetSearchTextNumber((sender as TextBox).Text).ToString();
            }
        }

        private unsafe void EnumerateFonts()
        {
            HWND hWND = (HWND)IntPtr.Zero;
            HDC hdc = PInvoke.GetDC(hWND);
            LOGFONTW logFont;
            logFont.lfCharSet = FONT_CHARSET.DEFAULT_CHARSET;

            PInvoke.EnumFontFamiliesEx(hdc, &logFont, EnumFontCallback, 0, 0);

            PInvoke.ReleaseDC(hWND, hdc); // 释放设备上下文
        }

        private unsafe int EnumFontCallback(LOGFONTW* lplf, TEXTMETRICW* lpntm, uint FontType, LPARAM lParam)
        {
            string name = lplf->lfFaceName.ToString();
            if (!name.StartsWith('@') && !FontNameList.Exists(x => x == name)) FontNameList.Add(name);
            return 1;
        }
    }
}