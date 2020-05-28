using System.Collections.Generic;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Services.Settings;

namespace DCS.Alternative.Launcher.Wizards
{
    public class AppendFirstUseWizardStepsEventArgs : DeferredEventArgs
    {
        public List<WizardStepBase> Steps
        {
            get;
        } = new List<WizardStepBase>();
    }
}