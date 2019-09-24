using System.Windows;

namespace DCS.Alternative.Launcher.Interaction.Behaviors
{
    public static class PlaceholderService
    {
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(PlaceholderService), new PropertyMetadata(string.Empty));

        public static string GetPlaceholderText(DependencyObject obj)
        {
            return (string) obj.GetValue(PlaceholderTextProperty);
        }

        public static void SetPlaceholderText(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderTextProperty, value);
        }
    }
}