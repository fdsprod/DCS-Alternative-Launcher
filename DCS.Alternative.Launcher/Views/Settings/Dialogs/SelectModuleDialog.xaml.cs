using System.Windows;
using DCS.Alternative.Launcher.DomainObjects;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public partial class SelectModuleDialog : Window
    {
        public static readonly DependencyProperty SelectedModuleProperty =
            DependencyProperty.Register("SelectedModule", typeof(ModuleBase), typeof(SelectModuleDialog), new PropertyMetadata(null));

        public static readonly DependencyProperty ModulesProperty =
            DependencyProperty.Register("ModulesProperty", typeof(ReactiveCollection<ModuleBase>), typeof(SelectModuleDialog), new PropertyMetadata(new ReactiveCollection<ModuleBase>()));

        public SelectModuleDialog()
        {
            InitializeComponent();

            Modules = new ReactiveCollection<ModuleBase>();
        }

        public ModuleBase SelectedModule
        {
            get { return (ModuleBase) GetValue(SelectedModuleProperty); }
            set { SetValue(SelectedModuleProperty, value); }
        }
        public ReactiveCollection<ModuleBase> Modules
        {
            get { return (ReactiveCollection<ModuleBase>) GetValue(ModulesProperty); }
            set { SetValue(ModulesProperty, value); }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}