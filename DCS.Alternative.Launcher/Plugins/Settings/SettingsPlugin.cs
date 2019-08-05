using DCS.Alternative.Launcher.Plugins.Settings.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Settings
{
    public class SettingsPlugin : PluginBase
    {
        public override string Name
        {
            get { return "Settings Plugin"; }
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
            container.Register(new SettingsController(container));

            base.RegisterContainerItems(container);
        }

        protected override void RegisterUISiteItems(IPluginNavigationSite site)
        {
            site.RegisterPluginNavigation<SettingsView, SettingsViewModel>("SETTINGS", this);

            base.RegisterUISiteItems(site);
        }
    }
}