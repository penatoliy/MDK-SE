using System.Windows;
using System.Windows.Media;

namespace Malware.Malformed.Controls
{
    public static class EmptyText
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(object),
            typeof(EmptyText),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static readonly DependencyProperty BrushProperty = DependencyProperty.RegisterAttached(
            "Brush",
            typeof(Brush),
            typeof(EmptyText),
            new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        public static void SetValue(FrameworkElement element, object value)
        {
            element.SetValue(ValueProperty, value);
        }

        public static object GetValue(FrameworkElement element)
        {
            var e = element.GetValue(ValueProperty);
            return e;
        }

        public static void SetBrush(FrameworkElement element, Brush value)
        {
            element.SetValue(BrushProperty, value);
        }

        public static object GetBrush(FrameworkElement element)
        {
            return element.GetValue(BrushProperty);
        }
    }
}
