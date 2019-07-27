using System;
using System.Windows;
using System.Windows.Media;

namespace DCS.Alternative.Launcher.Controls.MessageBoxEx
{
    /// <summary>
    ///     Interaction logic for MessageBoxEx.xaml
    /// </summary>
    public partial class MessageBoxEx : Window
    {
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

        public static MessageBoxResult Show(string message, string caption = "", MessageBoxButton buttons = MessageBoxButton.OK, ImageSource icon = null)
        {
            var messageBox = new MessageBoxEx();

            messageBox.txtCaption.Text = caption;
            messageBox.txtMessage.Text = message;
            messageBox.imgIcon.Source = icon;

            //messageBox.btnAbort.Visibility =
            //    buttons == MessageBoxButton.AbortRetryIgnore 
            //        ? Visibility.Visible
            //        : Visibility.Collapsed;
            //messageBox.btnRetry.Visibility =
            //    buttons == MessageBoxButton.AbortRetryIgnore
            //        ? Visibility.Visible
            //        : Visibility.Collapsed;
            //messageBox.btnIgnore.Visibility =
            //    buttons == MessageBoxButton.AbortRetryIgnore
            //        ? Visibility.Visible
            //        : Visibility.Collapsed;

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

            var result = messageBox.ShowDialog();

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    return MessageBoxResult.OK;
                case MessageBoxButton.OKCancel:
                    return result ?? false
                        ? MessageBoxResult.OK
                        : MessageBoxResult.Cancel;
                case MessageBoxButton.YesNoCancel:
                    return result == null
                        ? MessageBoxResult.Cancel
                        : result.Value
                            ? MessageBoxResult.Yes
                            : MessageBoxResult.No;
                case MessageBoxButton.YesNo:
                    return result ?? false
                        ? MessageBoxResult.No
                        : MessageBoxResult.Yes;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
            }
        }

        private void BtnAbort_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnRetry_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnIgnore_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
        }
    }
}