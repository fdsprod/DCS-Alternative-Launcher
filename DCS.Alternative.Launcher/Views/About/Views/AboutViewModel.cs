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
            var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

            var displayableVersion = $"{version} ({buildDate.ToShortDateString()})";

            Version.Value = displayableVersion;

            return base.InitializeAsync();
        }

        public override Task ActivateAsync()
        {
            Task.Run(async () => { IsUpdateAvailable.Value = await _autoUpdateService.CheckAsync(); });

            return base.ActivateAsync();
        }

        private void OnOpenUrl(string value)
        {
            Process.Start(value.Replace("$amp;", "&"));
        }
    }
}