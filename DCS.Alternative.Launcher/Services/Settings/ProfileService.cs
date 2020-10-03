using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Storage.Profiles;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Services.Settings
{
    internal class ProfileService : IProfileService
    {
        private readonly ILauncherSettingsService _settingsService;
        private readonly ApplicationEventRegistry _eventRegistry;
        private readonly List<Profile> _profiles = new List<Profile>();
        private readonly Dictionary<Profile, IDisposable> _dirtySubscriptions = new Dictionary<Profile, IDisposable>();

        private Dictionary<string, Option[]> _advancedOptionCache;
        private Dictionary<string, Option[]> _defaultAdvancedOptionCache;

        private GameOptionsCategory[] _gameOptions;
        private Profile _selectedProfile;

        public ProfileService(IContainer container)
        {
            _settingsService = container.Resolve<ILauncherSettingsService>();

            _eventRegistry = container.Resolve<ApplicationEventRegistry>();
            _eventRegistry.ApplicationStartup += ApplicationStartup;
        }

        public string SelectedProfileName
        {
            get { return _selectedProfile?.Name; }
            set
            {
                if (_selectedProfile?.Name != value)
                {
                    var profile = _profiles.FirstOrDefault(p => p.Name == value) ?? _profiles.FirstOrDefault();

                    _selectedProfile = profile;
                    _settingsService.SetValue(LauncherCategories.Launcher, LauncherSettingKeys.LastProfileName, profile.Name);

                    SelectedProfileChanged(profile);
                }
            }
        }

        public Option[] GetAdvancedOptions(string category)
        {
            if (_advancedOptionCache == null)
            {
                _advancedOptionCache = GetAdvancedOptions();
                _defaultAdvancedOptionCache = GetAdvancedOptions();
            }

            return _advancedOptionCache[category];
        }

        public InstallLocation GetSelectedInstall()
        {
            var directory = GetValue<string>(ProfileCategories.Profile, ProfileSettingKeys.SelectedInstall);
            
            return new InstallLocation(directory);
        }

        public void SetSelectedInstall(InstallLocation install)
        {
            SetValue(ProfileCategories.Profile, ProfileSettingKeys.SelectedInstall, install.Directory);
        }

        public object GetAdvancedOptionDefaultValue(string category, string optionId)
        {
            if (_advancedOptionCache == null)
            {
                _advancedOptionCache = GetAdvancedOptions();
                _defaultAdvancedOptionCache = GetAdvancedOptions();
            }

            return _defaultAdvancedOptionCache[category].FirstOrDefault(o => o.Id == optionId)?.Value;
        }


        public void RemoveProfile(string profileName)
        {
            var profile = _profiles.FirstOrDefault(p => p.Name == profileName);

            if (profile == null)
            {
                return;
            }

            _profiles.Remove(profile);

            UnsubscribeDirtyCheck(profile);

            var path = Path.Combine(ApplicationPaths.ProfilesPath, $"{profile.Name}.json");

            File.Delete(path);

            ProfilesChanged(_profiles);

            if (SelectedProfileName == profile.Name)
            {
                SelectedProfileName = _profiles.First().Name;
            }
        }

        public void AddProfile(Profile profile)
        {
            _profiles.Add(profile);
            SubscribeDirtyCheck(profile);

            ProfileStorageAdapter.PersistAsync(profile).Wait();
            ProfilesChanged(_profiles);
        }

        public void RemoveInstalls(string[] directories)
        {
            var installations = new List<string>(GetValue(ProfileCategories.Profile, ProfileSettingKeys.Installs, new string[0]));

            foreach (var directory in directories)
            {
                installations.Remove(directory);
            }

            SetValue(ProfileCategories.Profile, ProfileSettingKeys.Installs, installations.ToArray());
        }

        public void AddInstalls(params string[] directories)
        {
            var installations = new List<string>(GetValue(ProfileCategories.Profile, ProfileSettingKeys.Installs, new string[0]));

            foreach (var directory in directories)
            {
                if (!installations.Contains(directory))
                {
                    installations.Add(directory);
                }
            }

            SetValue(ProfileCategories.Profile, ProfileSettingKeys.Installs, installations.ToArray());
        }

        public IEnumerable<InstallLocation> GetInstallations()
        {
            var defaultInstalls = InstallationLocator.Locate().Select(i => i.Directory);
            var installs = GetValue(ProfileCategories.Profile, ProfileSettingKeys.Installs, defaultInstalls.ToArray());

            return installs.Select(i => new InstallLocation(i));
        }

        public bool TryGetValue<T>(string category, string key, out T value)
        {
            return _selectedProfile.TryGetValue<T>(category, key, out value);
        }

        public T GetValue<T>(string category, string key, T defaultValue = default(T))
        {
            return _selectedProfile.GetValue<T>(category, key, defaultValue);
        }

        public void SetValue(string category, string key, object value)
        {
            _selectedProfile.SetValue(category, key, value);
        }

        public void DeleteValue(string category, string key)
        {
            _selectedProfile.DeleteValue(category, key);
        }

        public GameOptionsCategory[] GetGameOptions()
        {
            if (_advancedOptionCache == null)
            {
                var options = new List<GameOptionsCategory>();
                var path = "Data/Options/GameOptions.json";
                var contents = File.ReadAllText(path);
                var allOptions = JsonConvert.DeserializeObject<GameOptionsCategory[]>(contents);

                options.AddRange(allOptions);

                path = Path.Combine(ApplicationPaths.OptionsPath, "GameOptions.json");
                contents = File.ReadAllText(path);

                var customOptions = JsonConvert.DeserializeObject<GameOptionsCategory[]>(contents);

                foreach (var option in customOptions)
                {
                    var existingOption = allOptions.FirstOrDefault(o => o.Id == option.Id);

                    if (existingOption == null)
                    {
                        options.Add(option);
                    }
                    else
                    {
                        options[options.IndexOf(existingOption)] = option;
                    }
                }


                _gameOptions = options.ToArray();
            }

            return _gameOptions;
        }

        private Dictionary<string, Option[]> GetAdvancedOptions()
        {
            var path = "Data/Options/AutoexecOptions.json";
            var contents = File.ReadAllText(path);
            var allOptions = JsonConvert.DeserializeObject<Option[]>(contents);
            var advancedOptions = new Dictionary<string, Option[]>();

            foreach (var group in allOptions.GroupBy(o => GetCategory(o?.Id)))
            {
                advancedOptions[group.Key] = group.ToArray();
            }

            try
            {
                path = Path.Combine(ApplicationPaths.OptionsPath, "AutoexecOptions.json");
                contents = File.ReadAllText(path);
                allOptions = JsonConvert.DeserializeObject<Option[]>(contents);

                foreach (var group in allOptions.GroupBy(o => GetCategory(o?.Id)))
                {
                    var options = new List<Option>();

                    if (advancedOptions.ContainsKey(group.Key))
                    {
                        options.AddRange(advancedOptions[group.Key]);
                    }

                    foreach (var option in group)
                    {
                        var existingOption = options.FirstOrDefault(o => o.Id == option.Id);

                        if (existingOption == null)
                        {
                            options.Add(option);
                        }
                        else
                        {
                            options[options.IndexOf(existingOption)] = option;
                        }
                    }

                    advancedOptions[group.Key] = options.ToArray();
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }

            return advancedOptions;
        }


        private void ApplicationStartup(object sender, DeferredEventArgs e)
        {
            Load();
        }

        private void Load()
        {
            Tracer.Info("Loading profiles");

            try
            {
                var profiles = ProfileStorageAdapter.GetAll();

                foreach (var profile in profiles)
                {
                    SubscribeDirtyCheck(profile);
                    _profiles.Add(profile);
                }

                ProfilesChanged(_profiles);

                var lastProfileName = _settingsService.GetValue<string>(LauncherCategories.Launcher, LauncherSettingKeys.LastProfileName);
                var selectedProfile = _profiles.FirstOrDefault(p => p.Name == lastProfileName) ?? _profiles.FirstOrDefault();

                _selectedProfile = selectedProfile;

                if (_selectedProfile != null)
                {
                    SelectedProfileChanged(_selectedProfile);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private void SubscribeDirtyCheck(Profile profile)
        {
            if (_dirtySubscriptions.ContainsKey(profile))
            {
                return;
            }

            var subscription = profile.IsDirty.Throttle(TimeSpan.FromMilliseconds(300)).Subscribe(x => { Save(profile); });

            _dirtySubscriptions.Add(profile, subscription);
        }

        private void UnsubscribeDirtyCheck(Profile profile)
        {
            if (!_dirtySubscriptions.ContainsKey(profile))
            {
                return;
            }

            var subscription = _dirtySubscriptions[profile];
            _dirtySubscriptions.Remove(profile);
            subscription.Dispose();
        }

        private void Save(Profile profile)
        {
            lock (profile)
            {
                if (!profile.IsDirty.Value)
                {
                    return;
                }

                profile.IsDirty.Value = false;

                ProfileStorageAdapter.PersistAsync(profile).Wait();
            }
        }


        private static string GetCategory(string id)
        {
            return OptionCategory.All.First(id.Contains);
        }

        private async void ProfilesChanged(IEnumerable<Profile> profiles)
        {
            try
            {
                await _eventRegistry.InvokeProfilesChangedAsync(this, new ProfilesChangedEvenArgs(profiles));
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private async void SelectedProfileChanged(Profile profile)
        {
            try
            {
                await _eventRegistry.InvokeCurrentProfileChangedAsync(this, new SelectedProfileChangedEventArgs(profile.Name));
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }
    }
}