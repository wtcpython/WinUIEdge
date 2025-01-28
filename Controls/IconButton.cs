using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace Edge
{
    public sealed partial class IconButton : Button
    {
        public event RoutedEventHandler ButtonClicked;

        public IconButton()
        {
            this.DefaultStyleKey = typeof(IconButton);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button IconButton = GetTemplateChild("IconButton") as Button;
            IconButton.Click += IconButton_Click;
        }

        private void IconButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(sender, e);
        }

        public static readonly DependencyProperty TipTextProperty = DependencyProperty.Register(
            nameof(TipText),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata(null));

        public string TipText
        {
            get => (string)GetValue(TipTextProperty);
            set => SetValue(TipTextProperty, value);
        }

        public static readonly DependencyProperty IconGlyphProperty = DependencyProperty.Register(
            nameof(IconGlyph),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata(null));

        public string IconGlyph
        {
            get => (string)GetValue(IconGlyphProperty);
            set => SetValue(IconGlyphProperty, value);
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            nameof(Key),
            typeof(VirtualKey),
            typeof(IconButton),
            new PropertyMetadata(null));

        public VirtualKey Key
        {
            get => (VirtualKey)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }

        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register(
            nameof(Modifiers),
            typeof(VirtualKeyModifiers),
            typeof(IconButton),
            new PropertyMetadata(null));

        public VirtualKeyModifiers Modifiers
        {
            get => (VirtualKeyModifiers)GetValue(ModifiersProperty);
            set => SetValue(ModifiersProperty, value);
        }
    }
}
