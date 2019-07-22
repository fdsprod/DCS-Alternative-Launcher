using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Services.Settings
{
    internal class SettingsService : ISettingsService
    {
        private static readonly object _syncRoot = new object();
        private Dictionary<string, Dictionary<string, object>> _settings = new Dictionary<string, Dictionary<string, object>>();

        public SettingsService()
        {
            Installations = InstallationLocator.Locate().ToArray();
            SelectedInstall = Installations.FirstOrDefault();

            Load();
        }

        public InstallLocation SelectedInstall
        {
            get;
            set;
        }

        public InstallLocation[] Installations
        {
            get;
        }

        public T GetValue<T>(string category, string key, T defaultValue = default(T))
        {
            if (!_settings.TryGetValue(category, out var keyLookup)) _settings[category] = keyLookup = new Dictionary<string, object>();

            if (!keyLookup.TryGetValue(key, out var result))
            {
                keyLookup[key] = result = defaultValue;
                Save();
            }

            return (T) result;
        }

        public void SetValue(string category, string key, object value)
        {
            if (!_settings.TryGetValue(category, out var keyLookup)) _settings[category] = keyLookup = new Dictionary<string, object>();

            keyLookup[key] = value;
            Save();
        }

        private void Save()
        {
            lock (_syncRoot)
            {
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings));
            }
        }

        private void Load()
        {
            lock (_syncRoot)
            {
                var json = File.ReadAllText("settings.json");
                _settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
            }
        }
    }
}