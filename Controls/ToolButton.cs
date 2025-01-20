using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace Edge
{
    public sealed partial class ToolButton : ContentControl
    {
        public event TextChangedEventHandler TextChanged;

        public ToolButton()
        {
            DefaultStyleKey = typeof(ToolButton);
        }

        public void ShowFlyout()
        {
            Flyout flyout = GetTemplateChild("ButtonFlyout") as Flyout;
            flyout?.ShowAt(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox SearchTextBox = GetTemplateChild("SearchTextBox") as TextBox;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }

        public static readonly DependencyProperty FontIconGlyphProperty = DependencyProperty.Register(
            nameof(FontIconGlyph),
            typeof(string),
            typeof(ToolButton),
            new PropertyMetadata(null));

        public string FontIconGlyph
        {
            get => (string)GetValue(FontIconGlyphProperty);
            set => SetValue(FontIconGlyphProperty, value);
        }

        public static readonly DependencyProperty ButtonTipProperty = DependencyProperty.Register(
            nameof(ButtonTip),
            typeof(string),
            typeof(ToolButton),
            new PropertyMetadata(null));

        public string ButtonTip
        {
            get => (string)GetValue(ButtonTipProperty);
            set => SetValue(ButtonTipProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(ToolButton),
            new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
            nameof(PlaceholderText),
            typeof(string),
            typeof(ToolButton),
            new PropertyMetadata(null));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }
    }
}
