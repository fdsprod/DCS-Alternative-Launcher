using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class CameraMirrorsSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public CameraMirrorsSettingsViewModel(SettingsController controller)
            : base("CAMERA MIRRORS", OptionCategory.CameraMirrors, controller)
        {
        }
    }
}