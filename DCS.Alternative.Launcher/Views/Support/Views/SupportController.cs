using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Support.Views
{
    public class SupportController
    {
        private readonly IDcsWorldService _dcsWorldService;
        private readonly ISettingsService _settingsService;

        public SupportController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }
    }
}