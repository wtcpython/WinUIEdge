using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;


namespace Edge
{
    public sealed class FileControl : ContentControl
    {
        public event SelectionChangedEventHandler SelectionChanged;

        public FileControl()
        {
            DefaultStyleKey = typeof(FileControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ComboBox comboBox = GetTemplateChild("EncodingComboBox") as ComboBox;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
            EncodingSelectedIndex = (sender as ComboBox).SelectedIndex;
        }

        public static readonly DependencyProperty FullPathProperty = DependencyProperty.Register(
            nameof(FullPath),
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(null));

        public string FullPath
        {
            get => (string)GetValue(FullPathProperty);
            set => SetValue(FullPathProperty, value);
        }

        public static readonly DependencyProperty FileContentProperty = DependencyProperty.Register(
            nameof(FileContent),
            typeof(object),
            typeof(FileControl),
            new PropertyMetadata(null));

        public object FileContent
        {
            get => GetValue(FileContentProperty);
            set => SetValue(FileContentProperty, value);
        }

        public static readonly DependencyProperty TypeNameProperty = DependencyProperty.Register(
            nameof(TypeName),
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(null));

        public string TypeName
        {
            get => (string)GetValue(TypeNameProperty);
            set => SetValue(TypeNameProperty, value);
        }

        public static readonly DependencyProperty LengthInfoProperty = DependencyProperty.Register(
            nameof(LengthInfo),
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(null));

        public string LengthInfo
        {
            get => (string)GetValue(LengthInfoProperty);
            set => SetValue(LengthInfoProperty, value);
        }

        public static readonly DependencyProperty EOFProperty = DependencyProperty.Register(
            nameof(EOF),
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(null));

        public string EOF
        {
            get => (string)GetValue(EOFProperty);
            set => SetValue(EOFProperty, value);
        }

        public static readonly DependencyProperty EncodingItemsProperty = DependencyProperty.Register(
            nameof(EncodingItems),
            typeof(EncodingInfo[]),
            typeof(FileControl),
            new PropertyMetadata(null));

        public EncodingInfo[] EncodingItems
        {
            get => (EncodingInfo[])GetValue(EncodingItemsProperty);
            set => SetValue(EncodingItemsProperty, value);
        }

        public static readonly DependencyProperty EncodingSelectedIndexProperty = DependencyProperty.Register(
            nameof(EncodingSelectedIndex),
            typeof(int),
            typeof(FileControl),
            new PropertyMetadata(null));

        public int EncodingSelectedIndex
        {
            get => (int)GetValue(EncodingSelectedIndexProperty);
            set => SetValue(EncodingSelectedIndexProperty, value);
        }
    }
}
