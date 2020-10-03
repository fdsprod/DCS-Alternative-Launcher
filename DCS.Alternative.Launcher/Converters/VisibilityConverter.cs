using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Data
{
    public class VisibilityConverter : IValueConverter
    {
        public VisibilityConverter()
        {
            HiddenParameterName = "Hidden";
            ReverseParameterName = "Reverse";
        }

        public static VisibilityConverter Instance
        {
            get;
        } = new VisibilityConverter();

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
                var str = value as string;

                if (str != null)
                {
                    result = str.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    var list = value as IList;

                    if (list != null)
                    {
                        result = list.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                    }

                    else
                    {
                        var enumerable = value as IEnumerable;

                        if (enumerable != null)
                        {
                            result = enumerable.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else if (value is int)
                        {
                            result = (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else if (value is double)
                        {
                            result = (double)value > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else if (value is bool)
                        {
                            result = (bool)value ? Visibility.Visible : Visibility.Collapsed;
                        }
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