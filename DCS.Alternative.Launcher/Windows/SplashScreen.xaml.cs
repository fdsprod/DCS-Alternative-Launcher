using System.Windows;

namespace DCS.Alternative.Launcher.Windows
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        public string Status
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}