using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class TerrainMirrorSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public TerrainMirrorSettingsViewModel(SettingsController controller)
            : base("    TERRAIN MIRRORS", OptionCategory.TerrainMirror, controller)
        {
        }
    }
}