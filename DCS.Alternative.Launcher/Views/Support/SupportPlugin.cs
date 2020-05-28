using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugins.Support.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Support
{
    public class SupportPlugin : PluginBase
    {
        public override string Name
        {
            get { return "Support Plugin"; }
        }
        public override int LoadOrder
        {
            get { return 30; }
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
            container.Register(new SupportController(container));

            return base.RegisterContainerItemsAsync(container);
        }

        protected override async Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            //site.RegisterPluginNavigation<SupportView, SupportViewModel>("SUPPORT", this);

            await base.RegisterUISiteItemsAsync(site);
        }
    }
}