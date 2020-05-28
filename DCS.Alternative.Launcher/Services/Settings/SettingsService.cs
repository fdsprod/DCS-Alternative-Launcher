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
    internal class LauncherSettingsService : ILauncherSettingsService
    {
        private static readonly object _syncRoot = new object();

        private readonly ReactiveProperty<bool> _isDirty = new ReactiveProperty<bool>(mode: ReactivePropertyMode.DistinctUntilChanged);
        
        private Dictionary<string, Dictionary<string, object>> _settings =
            new Dictionary<string, Dictionary<string, object>>();

        public LauncherSettingsService()
        {
            _isDirty.Throttle(TimeSpan.FromMilliseconds(300)).Subscribe(x => { Save(); });

            Load();
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
    }
}