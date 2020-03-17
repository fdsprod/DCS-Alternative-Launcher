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
        private readonly IAutoUpdateService _autoUpdateService;
        private readonly AboutController _controller;

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
            var version = name.Version;

            var displayableVersion = $"{version.Major}";

            Version.Value = displayableVersion;

            return base.InitializeAsync();
        }

        public override Task ActivateAsync()
        {
            Task.Run(async () =>
            {
                var result = await _autoUpdateService.CheckAsync();
                await result.UpdatingTask;
                IsUpdateAvailable.Value = result.IsUpdateAvailable;
            });

            return base.ActivateAsync();
        }

        private void OnOpenUrl(string value)
        {
            var ps = new ProcessStartInfo(value.Replace("$amp;", "&"))
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}