﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DCS.Alternative.Launcher.Data
{
    public class UriToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.DecodePixelWidth = int.TryParse(parameter?.ToString() ?? string.Empty, out var result) ? result : 500;
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(value.ToString());
            bi.EndInit();
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}