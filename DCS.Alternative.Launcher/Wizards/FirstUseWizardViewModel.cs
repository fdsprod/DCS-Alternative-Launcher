using System;
using System.Linq;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Wizards.Steps;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Wizards
{
    public class FirstUseWizardViewModel
    {
        public FirstUseWizardViewModel(IContainer container, params WizardStepBase[] steps)
        {
            Controller = container.Resolve<WizardController>();
            Controller.Complete += Controller_Complete;

            Array.ForEach(steps, step => Controller.Steps.Add(step));

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