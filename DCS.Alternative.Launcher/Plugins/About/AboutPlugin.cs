using DCS.Alternative.Launcher.Plugins.About.Views;
using DCS.Alternative.Launcher.Plugins.Manuals.Views;
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
            get { return 4; }
        }
        public override string Author
        {
            get { return "Jabbers"; }
        }
        public override string SupportUrl
        {
            get { return "https://github.com/jeffboulanger/DCS-Alternative-Launcher"; }
        }

        protected override void RegisterContainerItems(IContainer container)
        {
            container.Register(new ManualsController(container));

            base.RegisterContainerItems(container);
        }

        protected override void RegisterUISiteItems(IPluginNavigationSite site)
        {
            site.RegisterPluginNavigation<AboutView, AboutViewModel>("ABOUT", this);

            base.RegisterUISiteItems(site);
        }
    }
}