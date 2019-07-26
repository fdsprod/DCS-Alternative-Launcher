using System;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Navigation;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Windows
{
    public class MainWindowViewModel
    {
        private readonly IContainer _container;
        private readonly INavigationService _navigationService;

        public MainWindowViewModel(IContainer container)
        {
            _container = container;
            _navigationService = container.Resolve<INavigationService>();

            var settingsService = container.Resolve<ISettingsService>();
            var pluginNavigationSite = container.Resolve<IPluginNavigationSite>();
            ShowPluginCommand.Subscribe(OnShowPlugin);

            pluginNavigationSite.PluginRegistered += PluginNavigationSite_PluginRegistered;
        }

        public ReactiveCollection<PluginNavigationButton> PluginsButtons
        {
            get;

        } = new ReactiveCollection<PluginNavigationButton>();

        public ReactiveCommand<PluginNavigationButton> ShowPluginCommand
        {
            get;
        } = new ReactiveCommand<PluginNavigationButton>();

        private void PluginNavigationSite_PluginRegistered(object sender, PluginRegisteredEventArgs e)
        {
            PluginsButtons.Insert(0, new PluginNavigationButton(e.Name, e.ViewType, e.ViewModelType));
        }

        private async void OnShowPlugin(PluginNavigationButton plugin)
        {
            var viewModel = (INavigationAware) _container.Resolve(plugin.ViewModelType);

            await _navigationService.NavigateAsync(plugin.ViewType, viewModel);
        }
    }

    public class PluginNavigationButton
    {
        public PluginNavigationButton(string name, Type viewType, Type viewModelType)
        {
            Name = name;
            ViewType = viewType;
            ViewModelType = viewModelType;
        }

        public string Name { get; }

        public Type ViewType { get; }

        public Type ViewModelType { get; }
    }
}