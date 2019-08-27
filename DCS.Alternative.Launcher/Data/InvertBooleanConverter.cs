using System;
using System.Globalization;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Data
{
    public class InvertBooleanConverter : IValueConverter
    {
        public static InvertBooleanConverter Instance
        {
            get;
        } = new InvertBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }
    }
}