using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Support.Views
{
    public class SupportController
    {
        private readonly ISettingsService _settingsService;
        private readonly IDcsWorldService _dcsWorldService;

        public SupportController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }
    }
}