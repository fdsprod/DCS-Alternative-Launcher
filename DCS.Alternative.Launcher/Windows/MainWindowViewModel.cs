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

            foreach (var install in settingsService.GetInstallations())
            {
                Installations.Add(install);
                Installations.Add(install);
            }

            SelectedInstall.Value = settingsService.SelectedInstall;
            SelectInstallCommand.Subscribe(OnSelectInstall);

            ShowPluginCommand.Subscribe(OnShowPlugin);

            pluginNavigationSite.PluginRegistered += PluginNavigationSite_PluginRegistered;
        }

        public ReactiveCollection<PluginNavigationButton> PluginsButtons { get; } =
            new ReactiveCollection<PluginNavigationButton>();

        public ReactiveCollection<InstallLocation> Installations { get; } = new ReactiveCollection<InstallLocation>();

        public ReactiveProperty<InstallLocation> SelectedInstall { get; } = new ReactiveProperty<InstallLocation>();

        public ReactiveCommand<InstallLocation> SelectInstallCommand { get; } = new ReactiveCommand<InstallLocation>();

        public ReactiveCommand<PluginNavigationButton> ShowPluginCommand { get; } =
            new ReactiveCommand<PluginNavigationButton>();

        private void PluginNavigationSite_PluginRegistered(object sender, PluginRegisteredEventArgs e)
        {
            PluginsButtons.Insert(0, new PluginNavigationButton(e.Name, e.ViewType, e.ViewModelType));
        }

        private async void OnShowPlugin(PluginNavigationButton plugin)
        {
            var viewModel = (INavigationAware) _container.Resolve(plugin.ViewModelType);

            await _navigationService.NavigateAsync(plugin.ViewType, viewModel);
        }

        private void OnSelectInstall(InstallLocation install)
        {
            SelectedInstall.Value = install;
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