using System.Windows;
using System.Windows.Media;

namespace DCS.Alternative.Launcher.Controls.MessageBoxEx
{
    /// <summary>
    ///     Interaction logic for MessageBoxEx.xaml
    /// </summary>
    public partial class MessageBoxEx : Window
    {
        private MessageBoxResult _result;

        private MessageBoxEx()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            Loaded -= OnLoaded;
        }

        public static MessageBoxResult Show(string message, string caption = "", MessageBoxButton buttons = MessageBoxButton.OK, ImageSource icon = null, Window parent = null)
        {
            var messageBox = new MessageBoxEx();

            messageBox.txtCaption.Text = caption;
            messageBox.txtMessage.Text = message;
            messageBox.imgIcon.Source = icon;
            messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            messageBox.Owner = parent ?? Application.Current.MainWindow;

            messageBox.btnOK.Visibility =
                buttons == MessageBoxButton.OK || buttons == MessageBoxButton.OKCancel
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            messageBox.btnCancel.Visibility =
                buttons == MessageBoxButton.OKCancel || buttons == MessageBoxButton.YesNoCancel
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            messageBox.btnYes.Visibility =
                buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            messageBox.btnNo.Visibility =
                buttons == MessageBoxButton.YesNo || buttons == MessageBoxButton.YesNoCancel
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            messageBox.ShowDialog();

            return messageBox._result;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            DialogResult = true;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            DialogResult = null;
        }
    }
}