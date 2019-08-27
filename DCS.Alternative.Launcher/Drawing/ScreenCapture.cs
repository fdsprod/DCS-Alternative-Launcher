using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfScreenHelper;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = System.Drawing.Size;

namespace DCS.Alternative.Launcher.Drawing
{
    public static class ScreenCapture
    {
        public static ImageSource Snapshot(Screen screen)
        {
            using (var screenBmp = new Bitmap(
                (int) screen.Bounds.Width,
                (int) screen.Bounds.Height,
                PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(
                        (int) screen.Bounds.X,
                        (int) screen.Bounds.Y,
                        0,
                        0,
                        new Size((int) screen.Bounds.Width, (int) screen.Bounds.Height));
                    var image = Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    return image;
                }
            }
        }
    }
}