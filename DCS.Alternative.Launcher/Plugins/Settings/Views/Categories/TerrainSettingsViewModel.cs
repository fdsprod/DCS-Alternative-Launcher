using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public class TerrainSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public TerrainSettingsViewModel(SettingsController controller)
            : base("TERRAIN", AdvancedOptionCategory.Terrain, controller)
        {

        }
    }
}