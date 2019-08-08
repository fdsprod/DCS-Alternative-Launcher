using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public class CameraSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public CameraSettingsViewModel(SettingsController controller)
            : base("CAMERA", AdvancedOptionCategory.Camera, controller)
        {

        }
    }
}