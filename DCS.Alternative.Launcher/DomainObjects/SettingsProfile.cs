using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public class SettingsProfile
    {
        private readonly object _syncRoot = new object();
        private readonly ReactiveProperty<bool> _isDirty = new ReactiveProperty<bool>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public SettingsProfile()
        {
            _isDirty.Throttle(TimeSpan.FromMilliseconds(300)).Subscribe(x => { Save(); });
        }

        private void Save()
        {
            lock (_syncRoot)
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return;
                }

                if (!_isDirty.Value)
                {
                    return;
                }

                _isDirty.Value = false;

                File.WriteAllText(Path, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }

        public string Name
        {
            get;
            set;
        }

        public SettingsProfileType ProfileType
        {
            get;
            set;
        }

        public Dictionary<string, Dictionary<string, object>> Settings
        {
            get;
            set;
        } = new Dictionary<string, Dictionary<string, object>>();

        [JsonIgnore]
        public string Path
        {
            get;
            set;
        }
        
        public bool TryGetValue<T>(string category, string key, out T value)
        {
            lock (_syncRoot)
            {
                value = default(T);

                if (!Settings.TryGetValue(category, out var keyLookup))
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
            lock (_syncRoot)
            {
                if (!Settings.TryGetValue(category, out var keyLookup))
                {
                    Settings[category] = keyLookup = new Dictionary<string, object>();
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
            lock (_syncRoot)
            {
                if (!Settings.TryGetValue(category, out var keyLookup))
                {
                    Settings[category] = keyLookup = new Dictionary<string, object>();
                }

                keyLookup[key] = value;
                _isDirty.Value = true;
            }
        }

        public void DeleteValue(string category, string key)
        {
            lock (_syncRoot)
            {
                if (Settings.TryGetValue(category, out var keyLookup))
                {
                    Settings[category].Remove(key);
                    _isDirty.Value = true;
                }
            }
        }

        public void SetDirty()
        {
            lock (_syncRoot)
            {
                _isDirty.Value = true;
            }
        }
    }
}