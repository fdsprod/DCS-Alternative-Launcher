using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DCS.Alternative.Launcher.Services.Settings
{
    internal class SettingsService : ISettingsService
    {
        private static readonly object _syncRoot = new object();
        private InstallLocation _selectedInstall;

        private Dictionary<string, Dictionary<string, object>> _settings =
            new Dictionary<string, Dictionary<string, object>>();

        public SettingsService()
        {
            Load();

            var directory = GetValue(SettingsCategories.Installations, SettingsKeys.SelectedInstall, string.Empty);
            var installations = GetInstallations();

            _selectedInstall = installations.FirstOrDefault(i => i == directory);

            if (_selectedInstall == null && installations.Length > 0)
            {
                Tracer.Warn(
                    $"Unable to set selected install to {directory}.  Installation no longer exists in settings. ");
            }
        }

        public InstallLocation SelectedInstall
        {
            get => _selectedInstall;
            set
            {
                if (_selectedInstall != value)
                {
                    _selectedInstall = value;
                    SetValue(SettingsCategories.Installations, SettingsKeys.SelectedInstall, value.Directory);
                }
            }
        }

        public ModuleViewport[] GetModuleViewports()
        {
            return GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, new ModuleViewport[0]);
        }

        public void RemoveViewport(Module module, Viewport viewport)
        {
            var moduleViewports = GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, new ModuleViewport[0]);
            var mv = moduleViewports.FirstOrDefault(m => m.Module.ModuleId == module.ModuleId);
            
            viewport = mv?.Viewports.FirstOrDefault(v => v.Name == viewport.Name);

            mv?.Viewports.Remove(viewport);

            SetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, moduleViewports.ToArray());
        }

        public void UpsertViewport(Module module, Viewport viewport)
        {
            var moduleViewports = new List<ModuleViewport>(GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, new ModuleViewport[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.Module.ModuleId == module.ModuleId);

            if (mv == null)
            {
                mv = new ModuleViewport
                {
                    Module = module
                };

                moduleViewports.Add(mv);
            }

            var vp = mv.Viewports.FirstOrDefault(v => v.Name == viewport.Name);

            mv.Viewports.Remove(vp);
            mv.Viewports.Add(viewport);

            SetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, moduleViewports.ToArray());
        }

        public void RemoveModuleViewports(Module module)
        {
            var moduleViewports = new List<ModuleViewport>(GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, new ModuleViewport[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.Module.ModuleId == module.ModuleId);

            moduleViewports.Remove(mv);

            SetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewports, moduleViewports.ToArray());
        }

        public void RemoveInstalls(params string[] directories)
        {
            var installs = new List<string>(GetValue<IEnumerable<string>>(SettingsCategories.Installations, SettingsKeys.Installs, new string[0]));

            foreach (var directory in directories)
            {
                installs.Remove(directory);
            }

            SetValue(SettingsCategories.Installations, SettingsKeys.Installs, installs.ToArray());
        }

        public void AddInstalls(params string[] directories)
        {
            var installs = new List<string>(GetValue<IEnumerable<string>>(SettingsCategories.Installations,
                SettingsKeys.Installs, new string[0]));

            foreach (var directory in directories)
            {
                installs.Add(directory);
            }

            SetValue(SettingsCategories.Installations, SettingsKeys.Installs, installs.ToArray());
        }

        public InstallLocation[] GetInstallations()
        {
            var directories = new List<string>(GetValue<IEnumerable<string>>(SettingsCategories.Installations,
                SettingsKeys.Installs, new string[0]));
            var results = new List<InstallLocation>();

            foreach (var directory in directories)
            {
                results.Add(new InstallLocation(directory));
            }

            return results.ToArray();
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
                Save();
            }
        }

        private void Save()
        {
            lock (_syncRoot)
            {
                Tracer.Info("Saving settings.json");
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings));
            }
        }

        private void Load()
        {
            lock (_syncRoot)
            {
                Tracer.Info("Loading settings.json");

                if (File.Exists("settings.json"))
                {
                    var json = File.ReadAllText("settings.json");
                    _settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
                }
                else
                {
                    _settings = new Dictionary<string, Dictionary<string, object>>();
                }
            }
        }
    }

    public static class SettingsCategories
    {
        public const string Installations = "installations";
        public const string Viewports = "viewports";
        public const string LaunchOptions = "launchOptions";
    }

    public static class SettingsKeys
    {
        public const string Installs = "installs";
        public const string SelectedInstall = "selectedInstall";
        public const string ModuleViewports = "moduleViewports";
        public const string IsVREnabled = "isVREnabled";
    }
}