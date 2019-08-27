using System.Windows;
using System.Windows.Threading;

namespace DCS.Alternative.Launcher.Controls
{
    public static class UiDispatcher
    {
        private static Dispatcher _current;
        private static Application _application;
        public static Dispatcher Current
        {
            get { return _current ?? (_current = Application.Dispatcher); }
        }
        public static Application Application
        {
            get { return _application ?? (_application = Application.Current) ?? (_application = new Application()); }
        }

        public static void Initialize()
        {
            if (_current == null)
            {
                _current = Application.Current?.Dispatcher ?? new Application().Dispatcher;
            }
        }
    }
}