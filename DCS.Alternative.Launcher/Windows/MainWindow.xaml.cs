using System;
using System.Windows;
using System.Windows.Interop;

namespace DCS.Alternative.Launcher.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;

        public MainWindow()
        {
            InitializeComponent();

            AcrylicSource = BackgroundImage;
            SourceInitialized += onSourceInitialized;
        }

        public static FrameworkElement AcrylicSource
        {
            get;
            private set;
        }

        private void onSourceInitialized(object sender, EventArgs e)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCLBUTTONDBLCLK)
            {
                handled = true; //prevent double click from maximizing the window.
            }

            return IntPtr.Zero;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}