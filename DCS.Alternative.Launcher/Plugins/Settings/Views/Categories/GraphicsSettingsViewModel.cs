using System.Linq;
using System.Windows.Media;
using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public class GraphicsSettingsViewModel : AdvancedOptionSettingsViewModelBase
    {
        
        public GraphicsSettingsViewModel(SettingsController controller)
            : base("GRAPHICS", AdvancedOptionCategory.Graphics, controller)
        {

        }
    }
}