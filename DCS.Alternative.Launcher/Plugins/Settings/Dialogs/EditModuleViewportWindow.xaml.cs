using System.Windows;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    /// <summary>
    /// Interaction logic for EditModuleViewportWindow.xaml
    /// </summary>
    public partial class EditModuleViewportWindow : Window
    {
        public EditModuleViewportWindow()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
