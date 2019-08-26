using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Xaml.Behaviors;

namespace DCS.Alternative.Launcher.Behaviors
{
    public class WindowDisableContextMenulBehavior : Behavior<Window>
    {
        private const uint WM_SYSTEMMENU = 0xa4;
        private const uint WP_SYSTEMMENU = 0x02;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var windowHandle = new WindowInteropHelper(AssociatedObject).Handle;
            var hwndSource = HwndSource.FromHwnd(windowHandle);

            hwndSource.AddHook(WndProc);
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (((msg == WM_SYSTEMMENU) && (wParam.ToInt32() == WP_SYSTEMMENU)) || msg == 165)
            {
                handled = true;
            }

            return IntPtr.Zero;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}