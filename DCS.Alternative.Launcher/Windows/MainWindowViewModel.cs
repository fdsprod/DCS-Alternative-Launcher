using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Navigation;
using DCS.Alternative.Launcher.Storage.Profiles;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Windows
{
    public class MainWindowViewModel
    {
        private readonly DispatcherTimer _autoUpdateCheckTimer = new DispatcherTimer();
        private readonly IAutoUpdateService _autoUpdateService;
        private readonly IContainer _container;
        private readonly List<string> _images = new List<string>();
        private readonly INavigationService _navigationService;
        private readonly IProfileService _profileSettingsService;
        private readonly ILauncherSettingsService _settingsService;
        private readonly DispatcherTimer _slideShowTimer = new DispatcherTimer();
        private readonly ApplicationEventRegistry _eventRegistry;
        private int _nextIndex;

        private string _supportedExtensions = "*.jpg,*.png";

        public MainWindowViewModel(IContainer container)
        {
            _container = container;
            _eventRegistry = container.Resolve<ApplicationEventRegistry>();
            _navigationService = container.Resolve<INavigationService>();
            _autoUpdateService = container.Resolve<IAutoUpdateService>();
            _profileSettingsService = container.Resolve<IProfileService>();
            _settingsService = container.Resolve<ILauncherSettingsService>();

            _eventRegistry.CurrentProfileChanged += OnSelectedProfileChanged;
            _eventRegistry.ProfilesChanged += OnProfilesChanged;

            ShowPluginCommand.Subscribe(OnShowPlugin);

            _eventRegistry.PluginRegistered += OnPluginRegistered;

            var files = 
                new List<string>(
                    Directory.GetFiles(ApplicationPaths.WallpaperPath)
                .Where(s => string.IsNullOrEmpty(_supportedExtensions) || _supportedExtensions.Contains(Path.GetExtension(s))));

            if (files.Count == 0)
            {
                files.AddRange(
                    Directory.GetFiles(
                            Path.Combine(ApplicationPaths.ApplicationPath, "Resources", "Images", "Wallpaper"))
                        .Where(s => string.IsNullOrEmpty(_supportedExtensions) || _supportedExtensions.Contains(Path.GetExtension(s))));
            }

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
                _slideShowTimer.Interval = TimeSpan.FromMinutes(1);
                _slideShowTimer.Tick += _timer_Tick;
                _slideShowTimer.Start();

                NextImage();
            }

            SelectProfileCommand.Subscribe(OnSelectProfile);

            _autoUpdateCheckTimer.Interval = TimeSpan.FromMinutes(30);
            _autoUpdateCheckTimer.Tick += _autoUpdateCheckTimer_Tick;
            _autoUpdateCheckTimer.Start();

            UpdateProfiles();
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

        public ReactiveCollection<SettingsProfileModel> Profiles
        {
            get;
        } = new ReactiveCollection<SettingsProfileModel>();

        public ReactiveProperty<SettingsProfileModel> SelectedProfile
        {
            get;
        } = new ReactiveProperty<SettingsProfileModel>();

        public ReactiveCommand<SettingsProfileModel> SelectProfileCommand
        {
            get;
        } = new ReactiveCommand<SettingsProfileModel>();

        private void _timer_Tick(object sender, EventArgs e)
        {
            NextImage();
        }

        private void _autoUpdateCheckTimer_Tick(object sender, EventArgs e)
        {
            CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            await _autoUpdateService.CheckAsync();
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

        private void UpdateProfiles()
        {
            var profiles = ProfileStorageAdapter.GetAll();

            Profiles.Clear();

            foreach (var profile in profiles)
            {
                var model = new SettingsProfileModel(profile.Name);
                Profiles.Add(model);
            }

            var selectedProfileName = _profileSettingsService.SelectedProfileName;

            SelectedProfile.Value = Profiles.FirstOrDefault(p => p.Name.Value == selectedProfileName) ?? Profiles.FirstOrDefault();
        }
        
        private void OnPluginRegistered(object sender, PluginRegisteredEventArgs e)
        {
            PluginsButtons.Add(new PluginNavigationButton(e.Name, e.ViewType, e.ViewModelType));
        }

        private async void OnShowPlugin(PluginNavigationButton plugin)
        {
            foreach (var button in PluginsButtons)
            {
                button.IsSelected.Value = false;
            }

            plugin.IsSelected.Value = true;

            var viewModel = (INavigationAware) _container.Resolve(plugin.ViewModelType);

            await _navigationService.NavigateAsync(plugin.ViewType, viewModel);
        }

        private void OnSelectedProfileChanged(object sender, Services.Settings.SelectedProfileChangedEventArgs e)
        {
            if (SelectedProfile.Value?.Name.Value == e.ProfileName)
            {
                return;
            }

            var profile = Profiles.FirstOrDefault(p => p.Name.Value == e.ProfileName);

            if (profile == null)
            {
                UpdateProfiles();
            }
            else
            {
                SelectedProfile.Value = profile;
            }
        }
        private void OnProfilesChanged(object sender, EventArgs e)
        {
            UpdateProfiles();
        }

        private void OnSelectProfile(SettingsProfileModel value)
        {
            SelectedProfile.Value = value;
            _profileSettingsService.SelectedProfileName = value.Name.Value;
        }
    }
}