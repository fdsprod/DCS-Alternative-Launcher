using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Support.Views
{
    public class SupportController
    {
        private readonly IDcsWorldManager _dcsWorldManager;
        private readonly ILauncherSettingsService _settingsService;

        public SupportController(IContainer container)
        {
            _settingsService = container.Resolve<ILauncherSettingsService>();
            _dcsWorldManager = container.Resolve<IDcsWorldManager>();
        }
    }
}