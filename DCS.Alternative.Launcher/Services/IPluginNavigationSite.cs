using System;
using System.Windows.Controls;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.Services
{
    public interface IPluginNavigationSite
    {
        void RegisterPluginNavigation<TView, TViewModel>(string buttonText, IPlugin plugin)
            where TView : UserControl, new()
            where TViewModel : class, INavigationAware;

        event EventHandler<PluginRegisteredEventArgs> PluginRegistered;
    }
}