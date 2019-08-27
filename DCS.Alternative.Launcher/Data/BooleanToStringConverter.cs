using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Data
{
    public class BooleanToStringConverter : IValueConverter
    {
        public string TrueString
        {
            get;
            set;
        }

        public string FalseString
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool) value ? TrueString : FalseString;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}