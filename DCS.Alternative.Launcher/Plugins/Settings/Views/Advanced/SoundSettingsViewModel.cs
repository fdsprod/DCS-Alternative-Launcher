using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class SoundSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public SoundSettingsViewModel(SettingsController controller)
            : base("SOUND", OptionCategory.Sound, controller)
        {

        }
    }
}