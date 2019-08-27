using System;
using System.Collections.Generic;
using System.Windows;

namespace DCS.Alternative.Launcher
{
    public static class WindowAssist
    {
        private static readonly Dictionary<WeakReference<object>, WeakReference<Window>> _windowLookup = new Dictionary<WeakReference<object>, WeakReference<Window>>();

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached("ViewModel", typeof(object), typeof(WindowAssist), new PropertyMetadata(OnViewModelPropertyChanged));

        private static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;

            if (window == null)
            {
                return;
            }

            if (e.NewValue != null)
            {
                _windowLookup.Add(new WeakReference<object>(e.NewValue), new WeakReference<Window>(window));
            }
        }

        public static object GetViewModel(Window window)
        {
            return window.GetValue(ViewModelProperty);
        }

        public static void SetViewModel(Window window, object value)
        {
            window.SetValue(ViewModelProperty, value);
        }

        public static Window GetWindow(object viewModel)
        {
            foreach (var kvp in _windowLookup)
            {
                if (!kvp.Key.TryGetTarget(out var vm))
                {
                    continue;
                }

                if (vm != viewModel)
                {
                    continue;
                }

                return kvp.Value.TryGetTarget(out var window) ? window : null;
            }

            return null;
        }
    }
}