using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Extensions
{
    public static class ScreenExtensions
    {
        public static uint GetDpi(this Screen screen)
        {
            try
            {
                var point = new Point((int)screen.Bounds.Left + 1, (int)screen.Bounds.Top + 1);
                var mon = MonitorFromPoint(point, 2 /*MONITOR_DEFAULTTONEAREST*/);
                uint dpiX, dpiY;
                GetDpiForMonitor(mon, DpiType.Effective, out dpiX, out dpiY);
                return dpiX;
            }
            catch
            {
                return 96;
            }
        }
        
        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }

        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);


        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

    }

    public static class AssemblyExtensions
    {
        public static Type[] SafeGetTypes(this Assembly assembly)
        {
            Type[] assemblies;

            try
            {
                assemblies = assembly.GetTypes();
            }
            catch (FileNotFoundException)
            {
                assemblies = new Type[]
                {
                };
            }
            catch (NotSupportedException)
            {
                assemblies = new Type[]
                {
                };
            }
            catch (ReflectionTypeLoadException)
            {
                assemblies = new Type[]
                {
                };
            }

            return assemblies;
        }
    }
}