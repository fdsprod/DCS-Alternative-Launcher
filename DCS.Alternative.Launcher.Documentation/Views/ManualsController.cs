using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugin.Documentation.Views
{
    public class ManualsController
    {
        private readonly IDcsWorldManager _dcsWorldManager;

        public ManualsController(IContainer container)
        {
            _dcsWorldManager = container.Resolve<IDcsWorldManager>();
        }

        public Task<ModuleBase[]> GetModulesAsync()
        {
            return _dcsWorldManager.GetInstalledAircraftModulesAsync();
        }

        public AdditionalResource[] GetAdditionResources(string moduleId)
        {
            return _dcsWorldManager.GetAdditionalResourcesByModule(moduleId);
        }
    }
}