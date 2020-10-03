using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Data
{
    public class EnumerableCountGreaterThanOneVisibilityConverter : IValueConverter
    {
        public EnumerableCountGreaterThanOneVisibilityConverter()
        {
            HiddenParameterName = "Hidden";
            ReverseParameterName = "Reverse";
        }

        public static EnumerableCountGreaterThanOneVisibilityConverter Instance
        {
            get;
        } = new EnumerableCountGreaterThanOneVisibilityConverter();

        public string HiddenParameterName
        {
            get;
            set;
        }

        public string ReverseParameterName
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Visibility.Visible;

            if (value == null)
            {
                result = Visibility.Collapsed;
            }
            else
            {
                var list = value as IList;

                if (list != null)
                {
                    result = list.Cast<object>().Count() > 1 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    var enumerable = value as IEnumerable;

                    if (enumerable != null)
                    {
                        result = enumerable.Cast<object>().Count() > 1 ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }

            var reverse = false;
            var hidden = false;

            if (parameter != null)
            {
                var parameters = parameter.ToString().Split('|');

                hidden = parameters.Any(p =>
                    string.Compare(p, HiddenParameterName, StringComparison.OrdinalIgnoreCase) == 0);
                reverse = parameters.Any(p =>
                    string.Compare(p, ReverseParameterName, StringComparison.CurrentCultureIgnoreCase) == 0);
            }

            if (reverse)
            {
                result = result == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

            if (hidden && result == Visibility.Collapsed)
            {
                result = Visibility.Hidden;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}