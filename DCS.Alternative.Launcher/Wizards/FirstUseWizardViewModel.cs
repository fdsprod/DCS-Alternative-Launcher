using System;
using System.Linq;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Wizards.Steps;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Windows.FirstUse
{
    public class FirstUseWizardViewModel
    {
        public FirstUseWizardViewModel(IContainer container)
        {
            container.Register<WizardController>().AsSingleton();

            Controller = container.Resolve<WizardController>();
            Controller.Complete += Controller_Complete;

            Controller.Steps.Add(new InstallationsWizardStepViewModel(container));

            if (WpfScreenHelper.Screen.AllScreens.Count() > 1)
            {
                Controller.Steps.Add(new QueryViewportSetupWizardStepViewModel(container));
            }

            Controller.GoNextAsync();
        }

        public ReactiveProperty<bool?> DialogResult
        {
            get;
        } = new ReactiveProperty<bool?>();

        public WizardController Controller
        {
            get;
        }

        private void Controller_Complete(object sender, EventArgs e)
        {
            DialogResult.Value = true;
        }
    }
}