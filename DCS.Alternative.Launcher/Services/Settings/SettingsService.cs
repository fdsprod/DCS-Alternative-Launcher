using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Storage.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Services.Settings
{
    internal class SettingsService : ISettingsService
    {
        private static readonly object _syncRoot = new object();

        private readonly ReactiveProperty<bool> _isDirty = new ReactiveProperty<bool>(mode: ReactivePropertyMode.DistinctUntilChanged);

        private List<InstallLocation> _installationCache;
        private InstallLocation _selectedInstall;

        private Dictionary<string, Dictionary<string, object>> _settings =
            new Dictionary<string, Dictionary<string, object>>();

        public SettingsService()
        {
            _isDirty.Throttle(TimeSpan.FromMilliseconds(300)).Subscribe(x => { Save(); });

            Load();

            var directory = GetValue(SettingsCategories.Installations, SettingsKeys.SelectedInstall, string.Empty);

            GetInstallations();

            SelectedInstall = _installationCache.FirstOrDefault(i => i.Directory == directory);

            if (SelectedInstall == null && _installationCache.Count > 0)
            {
                Tracer.Warn($"Unable to set selected install to {directory}.  Installation no longer exists in settings. ");
                SelectedInstall =  _installationCache.FirstOrDefault();
            }
        }

        public InstallLocation SelectedInstall
        {
            get { return _selectedInstall; }
            set
            {
                if (_selectedInstall != value)
                {
                    _selectedInstall = value;

                    if (value != null)
                    {
                        Tracer.Info($"Selected Install set to {value.Directory}");
                    }
                    else
                    {
                        Tracer.Warn("Selected Install set to (null)");
                    }

                    SetValue(SettingsCategories.Installations, SettingsKeys.SelectedInstall, value.Directory);
                }
            }
        }

        public bool TryGetValue<T>(string category, string key, out T value)
        {
            lock (_settings)
            {
                value = default(T);

                if (!_settings.TryGetValue(category, out var keyLookup))
                {
                    return false;
                }

                if (!keyLookup.TryGetValue(key, out var result))
                {
                    return false;
                }

                if (result is JToken token)
                {
                    value = token.ToObject<T>();
                }
                else
                {
                    value = (T)result;
                }

                return true;
            }
        }

        public T GetValue<T>(string category, string key, T defaultValue = default(T))
        {
            lock (_settings)
            {
                if (!_settings.TryGetValue(category, out var keyLookup))
                {
                    _settings[category] = keyLookup = new Dictionary<string, object>();
                }

                if (!keyLookup.TryGetValue(key, out var result))
                {
                    keyLookup[key] = result = defaultValue;
                }

                if (result is JToken token)
                {
                    return token.ToObject<T>();
                }

                return (T)result;
            }
        }

        public void SetValue(string category, string key, object value)
        {
            lock (_settings)
            {
                if (!_settings.TryGetValue(category, out var keyLookup))
                {
                    _settings[category] = keyLookup = new Dictionary<string, object>();
                }

                keyLookup[key] = value;
                _isDirty.Value = true;
            }
        }

        public void DeleteValue(string category, string key)
        {
            lock (_settings)
            {
                if (_settings.TryGetValue(category, out var keyLookup))
                {
                    _settings[category].Remove(key);
                    _isDirty.Value = true;
                }
            }
        }

        private void Save()
        {
            lock (_syncRoot)
            {
                if (!_isDirty.Value)
                {
                    return;
                }

                _isDirty.Value = false;
                SettingsStorageAdapter.PersistAsync(_settings).Wait();
            }
        }

        private void Load()
        {
            lock (_syncRoot)
            {
                Tracer.Info("Loading settings.json");
                _settings = SettingsStorageAdapter.GetAll();
            }
        }

        public void RemoveInstalls(params string[] directories)
        {
            _installationCache.RemoveAll(i => directories.Contains(i.Directory));

            SetValue(SettingsCategories.Installations, SettingsKeys.Installs, _installationCache.Select(i => i.Directory).ToArray());
        }

        public void AddInstalls(params string[] directories)
        {
            foreach (var dir in directories)
            {
                if (_installationCache.All(i => i.Directory != dir))
                {
                    var install = new InstallLocation(dir);

                    _installationCache.Add(install);

                    if (SelectedInstall == null)
                    {
                        SelectedInstall = install;
                    }
                }
            }

            SetValue(SettingsCategories.Installations, SettingsKeys.Installs, _installationCache.Select(i => i.Directory).ToArray());
        }

        public InstallLocation[] GetInstallations()
        {
            if (_installationCache == null)
            {
                var directories = new List<string>(GetValue<IEnumerable<string>>(SettingsCategories.Installations, SettingsKeys.Installs, new string[0]));
                var results = new List<InstallLocation>();

                foreach (var directory in directories)
                {
                    results.Add(new InstallLocation(directory));
                }

                _installationCache = new List<InstallLocation>(results.ToArray());
            }

            return _installationCache.ToArray();
        }
    }
}