using DCS.Alternative.Launcher.Plugins.Settings.Views;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Settings
{
    public class SettingsPlugin : PluginBase
    {
        public override string Name => "Settings Plugin";

        public override string Author => "Jabbers";

        public override string SupportUrl => "https://github.com/jeffboulanger/DCS-Alternative-Launcher";

        protected override void RegisterUISiteItems(IPluginNavigationSite site)
        {
            site.RegisterPluginNavigation<SettingsView, SettingsViewModel>("SETTINGS", this);

            base.RegisterUISiteItems(site);
        }
    }
}