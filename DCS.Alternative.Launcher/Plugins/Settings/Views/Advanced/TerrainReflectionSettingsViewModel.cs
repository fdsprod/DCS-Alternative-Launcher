using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class TerrainReflectionSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public TerrainReflectionSettingsViewModel(SettingsController controller)
            : base("TERRAIN REFLECTION", OptionCategory.TerrainReflection, controller)
        {

        }
    }
}