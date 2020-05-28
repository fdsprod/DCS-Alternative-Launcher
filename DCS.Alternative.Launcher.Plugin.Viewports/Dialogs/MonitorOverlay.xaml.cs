using System;
using System.Runtime.InteropServices;
using System.Windows;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Dialogs
{
    public partial class MonitorOverlay : Window
    {
        private Screen _screen;

        public MonitorOverlay()
        {
            InitializeComponent();

            Screen = Screen.PrimaryScreen;

            Loaded += OnLoaded;
        }

        public Screen Screen
        {
            get { return _screen; }
            set
            {
                Guard.RequireIsNotNull(value, nameof(value));
                _screen = value;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Topmost = false;

            var initialScale = 1 / PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;

            //Move it off the screen so windows will rescale the DPI if necessary
            Left = _screen.Bounds.X * initialScale + 1;
            Top = _screen.Bounds.Y * initialScale + 1;

            WindowState = WindowState.Maximized;

            var visual = PresentationSource.FromVisual(this);
            var scale = 1 / visual.CompositionTarget.TransformToDevice.M11;

            if (initialScale != scale)
            {
                Left = _screen.Bounds.X * scale;
                Top = _screen.Bounds.Y * scale;
            }
            else
            {
                Left = _screen.Bounds.X * initialScale;
                Top = _screen.Bounds.Y * initialScale;
            }

            Width = _screen.Bounds.Width * scale;
            Height = _screen.Bounds.Height * scale;

            WindowState = WindowState.Normal;
        }

        public class ScreenInformation
        {
            public static uint PrimaryScreenDpi { get; private set; }

            static ScreenInformation()
            {
                uint dpiX;
                uint dpiY;
                GetDpi(1, 1,DpiType.EFFECTIVE, out dpiX, out dpiY);
                PrimaryScreenDpi = dpiX;
            }

            public static double GetDpi(double x, double y)
            {
                uint dpiX;
                uint dpiY;
                GetDpi((int)x, (int)y, DpiType.EFFECTIVE, out dpiX, out dpiY);
                return dpiX;
            }

            /// <summary>
            /// Returns the scaling of the given screen.
            /// </summary>
            /// <param name="dpiType">The type of dpi that should be given back..</param>
            /// <param name="dpiX">Gives the horizontal scaling back (in dpi).</param>
            /// <param name="dpiY">Gives the vertical scaling back (in dpi).</param>
            private static void GetDpi(int x, int y, DpiType dpiType, out uint dpiX, out uint dpiY)
            {
                var point = new System.Drawing.Point(x, y);
                var hmonitor = MonitorFromPoint(point, _MONITOR_DEFAULTTONEAREST);

                switch (GetDpiForMonitor(hmonitor, dpiType, out dpiX, out dpiY).ToInt32())
                {
                    case _S_OK: return;
                    case _E_INVALIDARG:
                        throw new ArgumentException("Unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
                    default:
                        throw new COMException("Unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
                }
            }

            //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062.aspx
            [DllImport("User32.dll")]
            private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

            //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx
            [DllImport("Shcore.dll")]
            private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

            const int _S_OK = 0;
            const int _MONITOR_DEFAULTTONEAREST = 2;
            const int _E_INVALIDARG = -2147024809;
        }

        /// <summary>
        /// Represents the different types of scaling.
        /// </summary>
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511.aspx"/>
        public enum DpiType
        {
            EFFECTIVE = 0,
            ANGULAR = 1,
            RAW = 2,
        }

    }
}