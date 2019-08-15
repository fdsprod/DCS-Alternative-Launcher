using System.Windows;

namespace DCS.Alternative.Launcher.Windows
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static FrameworkElement AcrylicSource
        {
            get;
            private set;
        }

        public MainWindow()
        {
            InitializeComponent();

            AcrylicSource = this.BackgroundImage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}