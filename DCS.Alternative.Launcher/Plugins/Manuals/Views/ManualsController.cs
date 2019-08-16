using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Manuals.Views
{
    public class ManualsController
    {
        private readonly ISettingsService _settingsService;
        private readonly IDcsWorldService _dcsWorldService;

        public ManualsController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }

        public Task<Module[]> GetModulesAsync()
        {
            return _dcsWorldService.GetInstalledAircraftModulesAsync();
        }

        public AdditionalResource[] GetAdditionResources(string moduleId)
        {
            return _settingsService.GetAdditionalResourcesByModule(moduleId);
        }
    }
}