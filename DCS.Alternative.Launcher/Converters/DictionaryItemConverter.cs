using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Data
{
    public class DictionaryItemConverter : IValueConverter
    {
        public static DictionaryItemConverter Instance
        {
            get;
        } = new DictionaryItemConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dict = value as IDictionary;

            if (dict != null)
            {
                Guard.RequireIsNotNull(parameter, nameof(parameter));

                return dict[parameter];
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}