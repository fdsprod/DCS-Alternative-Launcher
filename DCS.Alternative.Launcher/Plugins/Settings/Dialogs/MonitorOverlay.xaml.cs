using System.Windows;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public partial class MonitorOverlay : Window
    {
        public MonitorOverlay()
        {
            InitializeComponent();

            Screen = Screen.PrimaryScreen;

            Loaded += OnLoaded;
        }

        private Screen _screen;

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
            Left = _screen.Bounds.X;
            Top = _screen.Bounds.Y;
            Width = _screen.Bounds.Width;
            Height = _screen.Bounds.Height;
        }
    }
}
