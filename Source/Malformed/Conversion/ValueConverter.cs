using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Malware.Malformed.Conversion
{
    public abstract class ValueConverter<TFrom, TTo> : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TFrom || (value == null && !typeof(TFrom).IsValueType))
                return Convert((TFrom)value, targetType, parameter, culture);
            CheckValueForConvert(value);
            return GetDefaultToValue();
        }

        [Conditional("DEBUG")]
        void CheckValueForConvert(object value)
        {
            if (value == null)
                Debug.WriteLine($"{GetType().FullName} cannot convert value to {typeof(TTo).FullName} because it is null.");
            else
                Debug.WriteLine($"{GetType().FullName} cannot convert value to {typeof(TTo).FullName} because it is of the type {value.GetType().FullName}.");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TTo || (value == null && !typeof(TTo).IsValueType))
                return ConvertBack((TTo)value, targetType, parameter, culture);
            CheckValueForConvertBack(value);
            return GetDefaultFromValue();
        }

        [Conditional("DEBUG")]
        void CheckValueForConvertBack(object value)
        {
            if (value == null)
                Debug.WriteLine($"{GetType().FullName} cannot convert value back to {typeof(TFrom).FullName} because it is null.");
            else
                Debug.WriteLine($"{GetType().FullName} cannot convert value back to {typeof(TFrom).FullName} because it is of the type {value.GetType().FullName}.");
        }

        protected virtual TFrom GetDefaultFromValue()
        {
            return default(TFrom);
        }

        protected virtual TTo GetDefaultToValue()
        {
            return default(TTo);
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);

        protected abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);
    }
}
