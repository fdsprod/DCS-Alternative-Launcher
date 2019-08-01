using System.Windows;
using System.Windows.Data;

namespace DCS.Alternative.Launcher.Controls
{
    public static class BindingHelper
    {
        public static readonly Binding NullBinding = new Binding
        {
            Source = null,
            Path = new PropertyPath(""),
            Mode = BindingMode.OneTime
        };

        public static void BindProperty(object source, FrameworkElement target, DependencyProperty dp, string propertyName, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            Binding binding = new Binding
            {
                Source = source,
                Path = new PropertyPath(propertyName),
                Mode = mode,
                Converter = converter
            };

            target.SetBinding(dp, binding);
        }
    }
}