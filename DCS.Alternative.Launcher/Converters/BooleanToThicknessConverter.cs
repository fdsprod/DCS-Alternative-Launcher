using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Data
{
    public class BooleanToThicknessConverter : IValueConverter
    {
        public Thickness TrueThickness
        {
            get;
            set;
        }

        public Thickness FalseThickness
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool) value ? TrueThickness : FalseThickness;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}