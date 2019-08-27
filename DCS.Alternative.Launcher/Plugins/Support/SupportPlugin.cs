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
            get { return 3; }
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
            container.Register(new SupportController(container));

            base.RegisterContainerItems(container);
        }

        protected override void RegisterUISiteItems(IPluginNavigationSite site)
        {
            //site.RegisterPluginNavigation<SupportView, SupportViewModel>("SUPPORT", this);

            base.RegisterUISiteItems(site);
        }
    }
}