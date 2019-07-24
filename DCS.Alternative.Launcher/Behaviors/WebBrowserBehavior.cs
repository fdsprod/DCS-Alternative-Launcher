using System.Windows;
using System.Windows.Interactivity;
using CefSharp.Wpf;

namespace DCS.Alternative.Launcher.Behaviors
{
    public class WebBrowserBehavior : Behavior<ChromiumWebBrowser>
    {
        // Using a DependencyProperty as the backing store for BindableSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.Register("BindableSource", typeof(string), typeof(WebBrowserBehavior),
                new PropertyMetadata(null, OnBindableSourcePropertyChanged));

        public string BindableSource
        {
            get => (string) GetValue(BindableSourceProperty);
            set => SetValue(BindableSourceProperty, value);
        }


        public static void OnBindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var behavior = o as WebBrowserBehavior;

            behavior?.OnBindableSourceChanged((string) e.OldValue, (string) e.NewValue);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnBindableSourceChanged(string oldValue, string newValue)
        {
            if (AssociatedObject?.IsLoaded ?? false)
            {
                AssociatedObject.Load(newValue);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BindableSource))
            {
                AssociatedObject.Load(BindableSource);
            }
        }
    }
}