using System.Windows;
using DCS.Alternative.Launcher.Modules;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public partial class SelectModuleDialog : Window
    {
        public static readonly DependencyProperty SelectedModuleProperty =
            DependencyProperty.Register("SelectedModule", typeof(Module), typeof(SelectModuleDialog), new PropertyMetadata(null));

        public static readonly DependencyProperty ModulesProperty =
            DependencyProperty.Register("ModulesProperty", typeof(ReactiveCollection<Module>), typeof(SelectModuleDialog), new PropertyMetadata(new ReactiveCollection<Module>()));


        public SelectModuleDialog()
        {
            InitializeComponent();
        }
        
        public Module SelectedModule
        {
            get { return (Module) GetValue(SelectedModuleProperty); }
            set { SetValue(SelectedModuleProperty, value); }
        }
        
        public ReactiveCollection<Module> Modules
        {
            get { return (ReactiveCollection<Module>) GetValue(ModulesProperty); }
            set { SetValue(ModulesProperty, value); }
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