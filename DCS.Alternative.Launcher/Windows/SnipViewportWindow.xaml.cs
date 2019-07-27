using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfScreenHelper;
using Point = System.Windows.Point;

namespace DCS.Alternative.Launcher.Windows
{
    /// <summary>
    /// Interaction logic for SnipViewportWindow.xaml
    /// </summary>
    public partial class SnipViewportWindow : Window
    {
        private bool _isSnipping;
        private Point _mouseDownPosition;

        public Rect SnippedBounds
        {
            get;
            private set;
        }

        public SnipViewportWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            
            Deactivated += SnipViewportWindow_Deactivated;
            Loaded += SnipViewportWindow_Loaded;
            MouseDown += SnipViewportWindow_MouseDown;
            MouseUp += SnipViewportWindow_MouseUp;
            MouseMove += SnipViewportWindow_MouseMove;
            
        }

        private void SnipViewportWindow_Deactivated(object sender, EventArgs e)
        {
            Activate();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void SnipViewportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var screen = Screen.FromPoint(new Point(Left + 1, Top + 1));

            using (var screenBmp = new Bitmap(
                (int)screen.Bounds.Width,
                (int)screen.Bounds.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen((int)screen.Bounds.X, (int)screen.Bounds.Y, 0, 0, new System.Drawing.Size((int)screen.Bounds.Width, (int)screen.Bounds.Height));
                    Image.Source = Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        private void SnipViewportWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSnipping)
            {
                var position = e.GetPosition(this);
                var bounds = CalculateBounds(_mouseDownPosition, position);

                Canvas.SetLeft(Mask, bounds.Left);
                Canvas.SetTop(Mask, bounds.Top);
                Mask.Width = bounds.Width;
                Mask.Height = bounds.Height;

                VisualBrush.Viewbox = bounds;

                txtDimensions.Text = $"{bounds.Width} x {bounds.Height}";
                txtPosition.Text = $"x:{bounds.X} y:{bounds.Y}";
            }

            BringIntoView();
        }

        private void SnipViewportWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSnipping)
            {
                _isSnipping = false;

                var position = e.GetPosition(this);
                var bounds = CalculateBounds(_mouseDownPosition, position);

                SnippedBounds = bounds;
                DialogResult = true;
            }
        }

        private Rect CalculateBounds(Point position1, Point position2)
        {
            var x1 = Math.Min(position1.X, position2.X);
            var y1 = Math.Min(position1.Y, position2.Y);
            var x2 = Math.Max(position1.X, position2.X);
            var y2 = Math.Max(position1.Y, position2.Y);

            var rect = new Rect(x1, y1, x2 - x1, y2 - y1);

            return rect;
        }

        private void SnipViewportWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isSnipping)
            {
                _isSnipping = true;
                _mouseDownPosition = e.GetPosition(this);

                CaptureMouse();
            }
        }
    }
}
