using System.Windows;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Dialogs
{
    public partial class SelectViewportsDialog : Window
    {
        public static readonly DependencyProperty SelectedViewportProperty =
            DependencyProperty.Register("SelectededModule", typeof(ViewportModel), typeof(SelectModuleDialog), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewportsProperty =
            DependencyProperty.Register("ViewportsProperty", typeof(ReactiveCollection<ViewportModel>), typeof(SelectModuleDialog), new PropertyMetadata(null));

        public SelectViewportsDialog()
        {
            InitializeComponent();

            Viewports = new ReactiveCollection<ViewportModel>();
        }

        public ViewportModel SelectedViewport
        {
            get { return (ViewportModel) GetValue(SelectedViewportProperty); }
            set { SetValue(SelectedViewportProperty, value); }
        }
        public ReactiveCollection<ViewportModel> Viewports
        {
            get { return (ReactiveCollection<ViewportModel>) GetValue(ViewportsProperty); }
            set { SetValue(ViewportsProperty, value); }
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