using System;
using System.Windows;
using System.Windows.Interop;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public partial class ViewportEditorWindow : Window
    {
        private Screen _screen;

        public ViewportEditorWindow()
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
            var initialScale = 1 / PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;

            //Move it off the screen so windows will rescale the DPI if necessary
            Left = _screen.Bounds.X * initialScale + 1;
            Top = _screen.Bounds.Y * initialScale + 1;

            WindowState = WindowState.Maximized;

            var visual = PresentationSource.FromVisual(this);
            var scale = 1 / visual.CompositionTarget.TransformToDevice.M11;

            Width = _screen.Bounds.Width * scale;
            Height = _screen.Bounds.Height * scale;

            WindowState = WindowState.Normal;

        }
    }
}