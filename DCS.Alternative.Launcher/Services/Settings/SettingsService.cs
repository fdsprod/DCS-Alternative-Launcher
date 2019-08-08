using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Services.Settings
{
    internal class SettingsService : ISettingsService
    {
        private static readonly object _syncRoot = new object();
        private static readonly string[] _optionsCategories =
        {
            AdvancedOptions.TerrainReflection,
            AdvancedOptions.TerrainMirror,
            AdvancedOptions.Terrain,
            AdvancedOptions.CameraMirrors,
            AdvancedOptions.Camera,
            AdvancedOptions.Graphics,
            AdvancedOptions.Sound
        };

        private InstallLocation _selectedInstall;
        private Dictionary<string, AdvancedOption[]> _advancedOptionCache;

        private Dictionary<string, Dictionary<string, object>> _settings =
            new Dictionary<string, Dictionary<string, object>>();

        private ReactiveProperty<bool> _isDirty = new ReactiveProperty<bool>(mode: ReactivePropertyMode.DistinctUntilChanged);

        public SettingsService()
        {
            Load();

            _isDirty.Throttle(TimeSpan.FromMilliseconds(300)).Subscribe(x => { Save(); });

            var directory = GetValue(SettingsCategories.Installations, SettingsKeys.SelectedInstall, string.Empty);
            var installations = GetInstallations();

            _selectedInstall = installations.FirstOrDefault(i => i == directory);

            if (_selectedInstall == null && installations.Length > 0)
            {
                Tracer.Warn($"Unable to set selected install to {directory}.  Installation no longer exists in settings. ");
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

        public AdvancedOption[] GetAdvancedOptions(string category)
        {
            if (_advancedOptionCache == null)
            {
                const string path = "Resources/AdvancedOptions.json";

                var contents = File.ReadAllText(path);
                var allOptions = JsonConvert.DeserializeObject<AdvancedOption[]>(contents);

                _advancedOptionCache = new Dictionary<string, AdvancedOption[]>();

                foreach (var group in allOptions.GroupBy(o => GetCategory(o?.Id)))
                {
                    _advancedOptionCache.Add(group.Key, group.ToArray());
                }
            }

            return _advancedOptionCache[category];
        }

        private string GetCategory(string id)
        {
            return _optionsCategories.First(id.Contains);
        }

        public ModuleViewportTemplate[] GetViewportTemplates()
        {
            return GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]);
        }

        public ModuleViewportTemplate GetViewportTemplateByModule(string moduleId)
        {
            return GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]).FirstOrDefault(mv => mv.ModuleId == moduleId);
        }

        public void RemoveViewport(string moduleId, Viewport viewport)
        {
            var moduleViewports = GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]);
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);
            
            viewport = mv?.Viewports.FirstOrDefault(v => v.ViewportName == viewport.ViewportName);

            mv?.Viewports.Remove(viewport);

            SetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);

            if (mv == null)
            {
                mv = new ModuleViewportTemplate
                {
                    TemplateName = name, 
                    ModuleId = moduleId
                };

                moduleViewports.Add(mv);
            }

            var monitor = mv.Monitors.FirstOrDefault(m => m.MonitorId == screen.DeviceName);

            if (monitor == null)
            {
                monitor = new MonitorDefinition
                {
                    MonitorId = screen.DeviceName
                };

                mv.Monitors.Add(monitor);
            }

            monitor.DisplayWidth = (int) screen.Bounds.Width;
            monitor.DisplayHeight = (int) screen.Bounds.Height;

            var vp = mv.Viewports.FirstOrDefault(v => v.ViewportName == viewport.ViewportName);

            viewport.MonitorId = screen.DeviceName;

            mv.Viewports.Remove(vp);
            mv.Viewports.Add(viewport);

            SetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void RemoveViewportTemplate(string moduleId)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(GetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);

            moduleViewports.Remove(mv);

            SetValue(SettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
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
            var directories = new List<string>(GetValue<IEnumerable<string>>(SettingsCategories.Installations, SettingsKeys.Installs, new string[0]));
            var results = new List<InstallLocation>();

            foreach (var directory in directories)
            {
                results.Add(new InstallLocation(directory));
            }

            return results.ToArray();
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
                    value = (T) result;
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

        public ViewportDevice[] GetViewportDevices(string moduleId)
        {
            const string path = "Resources/ViewportDevices.json";

            var contents = File.ReadAllText(path);
            var devices = JsonConvert.DeserializeObject<Dictionary<string, List<ViewportDevice>>>(contents);
            var customDevices = GetValue<Dictionary<string, List<ViewportDevice>>>(SettingsCategories.Viewports, SettingsKeys.ViewportDevices);
            var results = new List<ViewportDevice>();

            if (devices.ContainsKey(moduleId))
            {
                results.AddRange(devices[moduleId]);
            }

            if (customDevices != null && customDevices.ContainsKey(moduleId))
            {
                results.AddRange(customDevices[moduleId]);
            }

            return results.ToArray();
        }

        public ModuleViewportTemplate[] GetDefaultViewportTemplates()
        {
            const string path = "Resources/ViewportTemplates.json";

            var contents = File.ReadAllText(path);
            var templates = JsonConvert.DeserializeObject<ModuleViewportTemplate[]>(contents);

            return templates;
        }

        private void Save()
        {
            lock (_syncRoot)
            {
                if (_isDirty.Value)
                {
                    _isDirty.Value = false;

                    Tracer.Info("Saving settings.json");
                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings));
                }
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
}