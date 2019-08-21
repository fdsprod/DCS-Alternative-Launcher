using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.About.Views
{
    public class AboutController
    {
        private readonly ISettingsService _settingsService;
        private readonly IDcsWorldService _dcsWorldService;

        public AboutController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }
    }
}