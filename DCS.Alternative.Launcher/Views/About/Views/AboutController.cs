using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.About.Views
{
    public class AboutController
    {
        private readonly IDcsWorldManager _dcsWorldManager;
        private readonly ILauncherSettingsService _settingsService;

        public AboutController(IContainer container)
        {
            _settingsService = container.Resolve<ILauncherSettingsService>();
            _dcsWorldManager = container.Resolve<IDcsWorldManager>();
        }
    }
}