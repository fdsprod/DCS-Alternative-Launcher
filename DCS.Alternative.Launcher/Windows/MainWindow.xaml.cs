using System.Windows;

namespace DCS.Alternative.Launcher.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AcrylicSource = BackgroundImage;
        }

        public static FrameworkElement AcrylicSource
        {
            get;
            private set;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}