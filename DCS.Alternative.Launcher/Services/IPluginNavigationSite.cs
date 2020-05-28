using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DCS.Alternative.Launcher.Plugins;
using DCS.Alternative.Launcher.Services.Navigation;

namespace DCS.Alternative.Launcher.Services
{
    public interface IPluginNavigationSite
    {
        Task RegisterPluginNavigationAsync<TView, TViewModel>(string buttonText, IPlugin plugin)
            where TView : UserControl, new()
            where TViewModel : class, INavigationAware;
    }
}