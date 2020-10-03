﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DCS.Alternative.Launcher.Data
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public abstract class MarkupConverter : MarkupExtension, IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Convert(value, targetType, parameter, culture);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return ConvertBack(value, targetType, parameter, culture);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        protected abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}