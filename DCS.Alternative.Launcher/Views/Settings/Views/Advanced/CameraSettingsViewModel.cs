using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class CameraSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public CameraSettingsViewModel(SettingsController controller)
            : base("    CAMERA", OptionCategory.Camera, controller)
        {
        }
    }
}