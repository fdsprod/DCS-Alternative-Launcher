using System;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Plugins.Settings.Views;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.StartupApps.Views.Settings;

namespace DCS.Alternative.Launcher.Plugin.Documentation
{
    public class StartupAppsPlugin : PluginBase
    {
        public override string Name
        {
            get { return "Startup Apps Plugin"; }
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
            var eventRegistry = container.Resolve<ApplicationEventRegistry>();

            eventRegistry.PopulateSettings += PopulateSettings;

            return base.RegisterContainerItemsAsync(container);
        }

        private async void PopulateSettings(object sender, Plugins.Settings.Views.PopulateSettingsEventArgs e)
        {
            using (e.GetDeferral())
            {
                try
                {
                    var container = e.Controller.GetChildContainer();
                    var controller = container.Resolve<StartupAppsController>();

                    e.AddCategory("STARTUP", new StartupAppsSettingsViewModel(controller, e.Controller));
                }
                catch (Exception ex)
                {
                    GeneralExceptionHandler.Instance.OnError(ex);
                }
            }
        }
    }
}