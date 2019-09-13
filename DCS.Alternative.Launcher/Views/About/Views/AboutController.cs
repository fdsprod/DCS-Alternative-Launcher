using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.About.Views
{
    public class AboutController
    {
        private readonly IDcsWorldService _dcsWorldService;
        private readonly ISettingsService _settingsService;

        public AboutController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }
    }
}