using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class GraphicsSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        public GraphicsSettingsViewModel(SettingsController controller)
            : base("GRAPHICS", OptionCategory.Graphics, controller)
        {
        }
    }
}