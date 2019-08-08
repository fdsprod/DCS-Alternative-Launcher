using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public class CameraMirrorsSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public CameraMirrorsSettingsViewModel(SettingsController controller)
            : base("CAMERA MIRRORS", AdvancedOptionCategory.CameraMirrors, controller)
        {

        }
    }
}