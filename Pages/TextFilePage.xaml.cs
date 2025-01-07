using Edge.Data;
using Microsoft.UI.Xaml.Controls;
using System.IO;
using System.Text;


namespace Edge
{
    public sealed partial class TextFilePage : Page
    {
        public string file;

        public EncodingInfo[] encodeList;

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
            EncodingComboBox.SelectedItem = encodeList[^1];

            // 初始化UI 数据
            FullPath.Text = file;
            TypeName.Text = typeName;
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

            editor.HighlightingLanguage = GetHighlightLanguage(typeName);
            editor.Editor.ReadOnly = false;
            editor.Editor.SetText(content);
            editor.Editor.ReadOnly = true;
            EOL.Text = editor.Editor.EOLMode.ToString().ToUpper();
        }

        private static string GetHighlightLanguage(string lang)
        {
            switch (lang)
            {
                case "C++": return "cpp";
                case "C#": return "csharp";
                case "HTML": return "html";
                case "JavaScript": return "javascript";
                case "JSON": return "json";
                case "XML": return "xml";
                default: return "plaintext";
            }
        }
    }
}