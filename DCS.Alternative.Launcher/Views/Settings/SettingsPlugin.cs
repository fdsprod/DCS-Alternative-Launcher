using System.Threading.Tasks;
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
        public override int LoadOrder
        {
            get { return 10; }
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
            container.Register(new SettingsController(container));

            return base.RegisterContainerItemsAsync(container);
        }

        protected override async Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            await site.RegisterPluginNavigationAsync<SettingsView, SettingsViewModel>("SETTINGS", this);
            await base.RegisterUISiteItemsAsync(site);
        }
    }
}