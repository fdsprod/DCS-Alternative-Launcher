using System;
using System.Diagnostics;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.ServiceModel;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.About.Views
{
    public class AboutViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly AboutController _controller;

        public AboutViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<AboutController>();

            OpenUrlCommand.Subscribe(OnOpenUrl);
        }

        public ReactiveCommand<string> OpenUrlCommand
        {
            get;
        } = new ReactiveCommand<string>();

        private void OnOpenUrl(string value)
        {
            Process.Start(value);
        }
    }
}