using System;
using System.Windows.Controls;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.Services
{
    public class PluginNavigationSite : IPluginNavigationSite
    {
        private readonly IContainer _container;

        public PluginNavigationSite(IContainer container)
        {
            _container = container;
        }

        public void RegisterPluginNavigation<TView, TViewModel>(string buttonText, IPlugin plugin)
            where TView : UserControl, new()
            where TViewModel : class, INavigationAware
        {
            Tracer.Info($"Registering Navigation Site [{buttonText}] as (UI: {typeof(TView).FullName}, VM: {typeof(TViewModel).FullName}).");

            _container.Register<TViewModel>().AsSingleton();
            _container.Register<TView>().AsSingleton();

            var handler = PluginRegistered;

            handler?.Invoke(this, new PluginRegisteredEventArgs(buttonText, typeof(TView), typeof(TViewModel)));
        }

        public event EventHandler<PluginRegisteredEventArgs> PluginRegistered;
    }
}