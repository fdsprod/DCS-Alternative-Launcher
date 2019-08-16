using DCS.Alternative.Launcher.Plugins.Game.Views;
using DCS.Alternative.Launcher.Plugins.Manuals.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Manuals
{
    public class ManualsPlugin : PluginBase
    {
        public override string Name
        {
            get { return "Manuals Plugin"; }
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
            site.RegisterPluginNavigation<ManualsView, ManualsViewModel>("DOCUMENTATION", this);

            base.RegisterUISiteItems(site);
        }
    }
}