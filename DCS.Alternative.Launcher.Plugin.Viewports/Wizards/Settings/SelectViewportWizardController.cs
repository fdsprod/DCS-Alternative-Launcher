using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Wizards.Settings
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