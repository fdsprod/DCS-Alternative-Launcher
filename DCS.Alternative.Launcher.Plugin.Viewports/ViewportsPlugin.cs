using System;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Plugin.Viewports.Services;
using DCS.Alternative.Launcher.Plugin.Viewports.Views.Settings;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugin.Viewports
{
    public class ViewportsPlugin : PluginBase
    {
        private IDcsWorldService _dcsWorldService;
        private IViewportService _viewportService;

        public ViewportsPlugin()
        {
            var resources = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/DCS.Alternative.Launcher.Plugin.Viewports;component/ViewportResources.xaml")
            };

            ApplicationResources = resources;
        }

        public override string Name
        {
            get { return "Viewports Plugin"; }
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

        public override ResourceDictionary ApplicationResources
        {
            get;
        }

        protected override Task RegisterContainerItemsAsync(IContainer container)
        {
            container.Register<IViewportService, ViewportService>().AsSingleton().UsingConstructor(() => new ViewportService(container));

            var eventRegistry = container.Resolve<ApplicationEventRegistry>();

            eventRegistry.PopulateSettings += PopulateSettings;

            _dcsWorldService = container.Resolve<IDcsWorldService>();
            _viewportService = container.Resolve<IViewportService>();

            return base.RegisterContainerItemsAsync(container);
        }
        
        private async void PopulateSettings(object sender, Plugins.Settings.Views.PopulateSettingsEventArgs e)
        {
            using (e.GetDeferral())
            {
                try
                {
                    var modules = await _dcsWorldService.GetInstalledAircraftModulesAsync();
                    var container = e.Controller.GetChildContainer();
                    var viewportController = container.Resolve<ViewportController>();

                    e.AddCategory("VIEWPORTS", new ViewportSettingsViewModel(viewportController, e.Controller));

                    foreach (var module in modules)
                    {
                        var options = _viewportService.GetViewportOptionsByModuleId(module.ModuleId);

                        if (options.Length > 0)
                        {
                            e.AddCategory("VIEWPORTS", new ViewportOptionsViewModel(module, options, viewportController, e.Controller));
                        }
                    }
                }
                catch (Exception ex)
                {
                    GeneralExceptionHandler.Instance.OnError(ex);
                }
            }
        }

        protected override async Task RegisterUISiteItemsAsync(IPluginNavigationSite site)
        {
            await base.RegisterUISiteItemsAsync(site);
        }
    }
}