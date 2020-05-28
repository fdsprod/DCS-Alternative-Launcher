using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugins.About.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.About
{
    public class About : PluginBase
    {
        public override string Name
        {
            get { return "About Plugin"; }
        }
        public override int LoadOrder
        {
            get { return 40; }
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
            container.Register(new AboutController(container));

            return base.RegisterContainerItemsAsync(container);
        }

        protected override async Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            await site.RegisterPluginNavigationAsync<AboutView, AboutViewModel>("ABOUT", this);
            await base.RegisterUISiteItemsAsync(site);
        }
    }
}