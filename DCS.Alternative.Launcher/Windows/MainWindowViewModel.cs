using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
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

        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly List<string> _images = new List<string>();

        private int _nextIndex;

        public MainWindowViewModel(IContainer container)
        {
            _container = container;
            _navigationService = container.Resolve<INavigationService>();
            var pluginNavigationSite = container.Resolve<IPluginNavigationSite>();
            ShowPluginCommand.Subscribe(OnShowPlugin);

            pluginNavigationSite.PluginRegistered += PluginNavigationSite_PluginRegistered;

            var files = new List<string>(Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Images", "Wallpaper"), "*.jpg"));
            var rand = new Random();

            while (files.Count > 0)
            {
                var index = rand.Next(files.Count);
                var file = files[index];
                files.RemoveAt(index);
                _images.Add(file);
            }

            if (_images.Count > 0)
            {
                _timer.Interval = TimeSpan.FromMinutes(1);
                _timer.Tick += _timer_Tick;
                _timer.Start();

                NextImage();
            }
        }

        public ReactiveProperty<string> ImageUrl
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveCollection<PluginNavigationButton> PluginsButtons
        {
            get;
        } = new ReactiveCollection<PluginNavigationButton>();

        public ReactiveCommand<PluginNavigationButton> ShowPluginCommand
        {
            get;
        } = new ReactiveCommand<PluginNavigationButton>();

        private void _timer_Tick(object sender, EventArgs e)
        {
            NextImage();
        }

        private void NextImage()
        {
            ImageUrl.Value = _images[_nextIndex];
            _nextIndex++;

            if (_nextIndex >= _images.Count)
            {
                _nextIndex = 0;
            }
        }

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