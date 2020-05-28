using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.Services
{
    internal class PluginNavigationSite : IPluginNavigationSite
    {
        private readonly IContainer _container;
        private readonly ApplicationEventRegistry _eventRegistry;

        public PluginNavigationSite(IContainer container)
        {
            _container = container;
            _eventRegistry = container.Resolve<ApplicationEventRegistry>();
        }

        public Task RegisterPluginNavigationAsync<TView, TViewModel>(string buttonText, IPlugin plugin)
            where TView : UserControl, new()
            where TViewModel : class, INavigationAware
        {
            Tracer.Info($"Registering Navigation Site [{buttonText}] as (UI: {typeof(TView).FullName}, VM: {typeof(TViewModel).FullName}).");

            _container.Register<TViewModel>().AsSingleton();
            _container.Register<TView>().AsSingleton();

            return _eventRegistry.InvokePluginRegisteredAsync(this, new PluginRegisteredEventArgs(buttonText, typeof(TView), typeof(TViewModel)));
        }
    }
}