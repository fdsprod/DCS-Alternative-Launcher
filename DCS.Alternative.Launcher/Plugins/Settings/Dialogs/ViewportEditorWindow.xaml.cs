using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public partial class ViewportEditorWindow : Window
    {
        public ViewportEditorWindow()
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
