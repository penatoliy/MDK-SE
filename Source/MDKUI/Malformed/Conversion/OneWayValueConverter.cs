using System;
using System.Globalization;
using System.Windows.Data;

namespace Malware.MDKUI.Malformed.Conversion
{
    public abstract class OneWayValueConverter<TFrom, TTo> : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TFrom)
                return this.Convert((TFrom)value, targetType, parameter, culture);
            return this.GetDefaultToValue();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        protected virtual TTo GetDefaultToValue()
        {
            return default(TTo);
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);
    }
}