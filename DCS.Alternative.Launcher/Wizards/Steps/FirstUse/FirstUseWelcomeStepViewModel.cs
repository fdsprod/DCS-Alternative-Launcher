using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.ServiceModel;

namespace DCS.Alternative.Launcher.Wizards.Steps.FirstUse
{
    public class FirstUseWelcomeStepViewModel : WizardStepBase
    {
        public FirstUseWelcomeStepViewModel(IContainer container)
            : base(container)
        {
        }

        public override Task ActivateAsync()
        {
            Controller.CanClose.Value = false;
            Controller.CanGoBack.Value = false;

            return base.ActivateAsync();
        }
    }
}