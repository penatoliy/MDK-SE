using System;
using System.Globalization;
using System.Windows.Media;

namespace Malware.MDKUI.Malformed.Conversion
{
    public class ImageBrushConverter : ValueConverter<ImageSource, ImageBrush>
    {
        protected override ImageBrush Convert(ImageSource value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var brush = new ImageBrush(value);
            this.ConfigureBrush(brush);
            return brush;
        }

        protected override ImageSource ConvertBack(ImageBrush value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ImageSource;
        }

        protected virtual void ConfigureBrush(ImageBrush brush)
        { }
    }
}