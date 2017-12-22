using System;
using System.Globalization;

namespace Malware.MDKUI.Malformed.Conversion
{
    public class BooleanConverter : OneWayValueConverter<object, object>
    {
        public object FalseValue { get; set; } = false;

        public object TrueValue { get; set; } = true;

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.IsFalsy() ? FalseValue : TrueValue;
        }
    }
}
