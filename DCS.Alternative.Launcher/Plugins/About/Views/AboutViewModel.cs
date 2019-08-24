using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.About.Views
{
    public class AboutViewModel : NavigationAwareBase
    {
        private readonly AboutController _controller;
        private readonly IAutoUpdateService _autoUpdateService;

        public AboutViewModel(IContainer container)
        {
            _controller = container.Resolve<AboutController>();
            _autoUpdateService = container.Resolve<IAutoUpdateService>();

            OpenUrlCommand.Subscribe(OnOpenUrl);
        }

        public ReactiveCommand<string> OpenUrlCommand
        {
            get;
        } = new ReactiveCommand<string>();

        public ReactiveProperty<bool> IsUpdateAvailable
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> Version
        {
            get;
        } = new ReactiveProperty<string>();

        protected override Task InitializeAsync()
        {
            var assembly = Assembly.GetAssembly(typeof(AboutViewModel));
            var name = assembly.GetName();

            Version.Value = name.Version.ToString();

            return base.InitializeAsync();
        }

        public override Task ActivateAsync()
        {
            Task.Run(async () => { IsUpdateAvailable.Value = await _autoUpdateService.CheckAsync(); });

            return base.ActivateAsync();
        }

        private void OnOpenUrl(string value)
        {
            Process.Start(value);
        }
    }
}