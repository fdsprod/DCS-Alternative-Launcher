using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class GeneralSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public GeneralSettingsViewModel(SettingsController controller)
            : base("    GENERAL", OptionCategory.General, controller)
        {
        }
    }
}