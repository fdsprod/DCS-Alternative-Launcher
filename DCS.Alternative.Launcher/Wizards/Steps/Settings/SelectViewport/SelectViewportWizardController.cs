using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.DomainObjects;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Wizards.Steps.Settings.SelectViewport
{
    public class SelectViewportWizardController : WizardController
    {
        public ModuleViewportTemplate SelectedTemplate
        {
            get;
            set;
        }
    }
}