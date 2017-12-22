using System;
using System.Globalization;
using System.Windows;

namespace Malware.MDKUI.Malformed.Conversion
{
    public class VisibilityConverter : OneWayValueConverter<object, Visibility>
    {
        public static readonly VisibilityConverter CollapseIfFalsy = new VisibilityConverter {
            TrueValue = Visibility.Visible,
            FalseValue = Visibility.Collapsed
        };

        public static readonly VisibilityConverter HideIfFalsy = new VisibilityConverter {
            TrueValue = Visibility.Visible,
            FalseValue = Visibility.Hidden
        };

        public static readonly VisibilityConverter CollapseIfTruthy = new VisibilityConverter {
            TrueValue = Visibility.Collapsed,
            FalseValue = Visibility.Visible
        };

        public static readonly VisibilityConverter HideIfTruthy = new VisibilityConverter {
            TrueValue = Visibility.Hidden,
            FalseValue = Visibility.Visible
        };

        public Visibility FalseValue { get; set; }

        public Visibility TrueValue { get; set; }

        protected override Visibility Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.IsFalsy() ? FalseValue : TrueValue;
        }
    }
}
