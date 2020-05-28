using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugin.Documentation.Views;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugin.Documentation
{
    public class ManualsPlugin : PluginBase
    {
        public override string Name
        {
            get { return "Manuals Plugin"; }
        }
        public override int LoadOrder
        {
            get { return 20; }
        }
        public override string Author
        {
            get { return "Jabbers"; }
        }
        public override string SupportUrl
        {
            get { return "https://github.com/jeffboulanger/DCS-Alternative-Launcher"; }
        }

        protected override Task RegisterContainerItemsAsync(IContainer container)
        {
            container.Register(new ManualsController(container));

            return base.RegisterContainerItemsAsync(container);
        }

        protected override async Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            await site.RegisterPluginNavigationAsync<ManualsView, ManualsViewModel>("DOCUMENTATION", this);

            await base.RegisterUISiteItemsAsync(site);
        }
    }
}