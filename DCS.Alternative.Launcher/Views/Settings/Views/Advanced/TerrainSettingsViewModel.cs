using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class TerrainSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public TerrainSettingsViewModel(SettingsController controller)
            : base("    TERRAIN", OptionCategory.Terrain, controller)
        {
        }
    }
}