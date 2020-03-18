using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Storage.Profiles;
using Newtonsoft.Json;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Services.Settings
{
    public class ProfileSettingsService : IProfileSettingsService
    {
        private readonly ISettingsService _settingsService;
        private readonly List<SettingsProfile> _profiles = new List<SettingsProfile>();

        private Dictionary<string, Option[]> _advancedOptionCache;
        private Dictionary<string, Option[]> _defaultAdvancedOptionCache;

        private DcsOptionsCategory[] _dcsOptions;
        private SettingsProfile _selectedProfile;

        public ProfileSettingsService(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();

            Load();

            var lastProfileName = _settingsService.GetValue<string>(SettingsCategories.Launcher, SettingsKeys.LastProfileName);
            var profile = _profiles.FirstOrDefault(p => p.Name == lastProfileName) ?? _profiles.FirstOrDefault();

            _selectedProfile = profile;
        }

        public string SelectedProfileName
        {
            get { return _selectedProfile?.Name; }
            set
            {
                if (_selectedProfile.Name != value)
                {
                    var profile = _profiles.FirstOrDefault(p => p.Name == value) ?? _profiles.FirstOrDefault();

                    _selectedProfile = profile;
                    _settingsService.SetValue(SettingsCategories.Launcher, SettingsKeys.LastProfileName, profile.Name);

                    OnSelectedProfileChanged();
                }
            }
        }

        public event EventHandler<SelectedProfileChangedEventArgs> SelectedProfileChanged;
        public event EventHandler ProfilesChanged;

        private void Load()
        {
            Tracer.Info("Loading profiles");

            try
            {
                var profiles = SettingsProfileStorageAdapter.GetAll();
                _profiles.AddRange(profiles);
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
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

        public object GetAdvancedOptionDefaultValue(string category, string optionId)
        {
            if (_advancedOptionCache == null)
            {
                _advancedOptionCache = GetAdvancedOptions();
                _defaultAdvancedOptionCache = GetAdvancedOptions();
            }

            return _defaultAdvancedOptionCache[category].FirstOrDefault(o => o.Id == optionId)?.Value;
        }

        public ModuleViewportTemplate[] GetViewportTemplates()
        {
            return GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]);
        }

        public ModuleViewportTemplate GetViewportTemplateByModule(string moduleId)
        {
            return GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]).FirstOrDefault(mv => mv.ModuleId == moduleId);
        }

        public void RemoveViewport(string moduleId, Viewport viewport)
        {
            var moduleViewports = GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]);
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);

            viewport = mv?.Viewports.FirstOrDefault(v => v.ViewportName == viewport.ViewportName);

            mv?.Viewports.Remove(viewport);

            SetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void ClearViewports(string name, string moduleId)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId && m.TemplateName == name);

            mv?.Viewports.Clear();

            SetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void RemoveProfile(string profileName)
        {
            var profile = _profiles.FirstOrDefault(p => p.Name == profileName);

            if (profile == null)
            {
                return;
            }

            _profiles.Remove(profile);

            File.Delete(profile.Path);

            OnProfilesChanged();

            if (SelectedProfileName == profile.Name)
            {
                SelectedProfileName = _profiles.First().Name;
            }
        }

        public void AddProfile(SettingsProfile profile)
        {
            _profiles.Add(profile);
            SettingsProfileStorageAdapter.PersistAsync(profile).Wait();
            OnProfilesChanged();
        }

        public void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId && m.TemplateName == name);

            if (mv == null)
            {
                mv = new ModuleViewportTemplate
                {
                    TemplateName = name,
                    ModuleId = moduleId
                };

                moduleViewports.Add(mv);
            }
            else
            {
                mv.TemplateName = name;
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

            monitor.DisplayWidth = (int)screen.Bounds.Width;
            monitor.DisplayHeight = (int)screen.Bounds.Height;

            var vp = mv.Viewports.FirstOrDefault(v => v.ViewportName == viewport.ViewportName);

            viewport.MonitorId = screen.DeviceName;

            mv.Viewports.Remove(vp);
            mv.Viewports.Add(viewport);

            SetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void RemoveViewportTemplate(string moduleId)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(GetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);

            moduleViewports.Remove(mv);

            SetValue(ProfileSettingsCategories.Viewports, SettingsKeys.ModuleViewportTemplates, moduleViewports.ToArray());
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

        public DcsOptionsCategory[] GetDcsOptions()
        {
            if (_advancedOptionCache == null)
            {
                var options = new List<DcsOptionsCategory>();
                var path = "Data/Options/GameOptions.json";
                var contents = File.ReadAllText(path);
                var allOptions = JsonConvert.DeserializeObject<DcsOptionsCategory[]>(contents);

                options.AddRange(allOptions);

                path = Path.Combine(ApplicationPaths.OptionsPath, "GameOptions.json");
                contents = File.ReadAllText(path);

                var customOptions = JsonConvert.DeserializeObject<DcsOptionsCategory[]>(contents);

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


                _dcsOptions = options.ToArray();
            }

            return _dcsOptions;
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

        public ModuleViewportTemplate[] GetDefaultViewportTemplates()
        {
            var path = "Data/Viewports/ViewportTemplates.json";
            var contents = File.ReadAllText(path);
            var templates = JsonConvert.DeserializeObject<ModuleViewportTemplate[]>(contents).ToList();

            path = Path.Combine(ApplicationPaths.ViewportPath, "ViewportTemplates.json");
            contents = File.ReadAllText(path);

            var customTemplates = JsonConvert.DeserializeObject<ModuleViewportTemplate[]>(contents);

            foreach (var customTemplate in customTemplates)
            {
                var existingTemplate = templates.FirstOrDefault(t => t.TemplateName == customTemplate.TemplateName);

                if (existingTemplate != null)
                {
                    var index = templates.IndexOf(existingTemplate);
                    templates[index] = customTemplate;
                }
                else
                {
                    templates.Add(customTemplate);
                }
            }

            return templates.ToArray();
        }

        public Dictionary<string, ViewportOption[]> GetAllViewportOptions()
        {
            var path = "Data/Viewports/ViewportOptions.json";
            var contents = File.ReadAllText(path);
            var optionsLookup = JsonConvert.DeserializeObject<Dictionary<string, ViewportOption[]>>(contents);

            path = Path.Combine(ApplicationPaths.ViewportPath, "ViewportOptions.json");
            contents = File.ReadAllText(path);

            var customOptionsLookup = JsonConvert.DeserializeObject<Dictionary<string, ViewportOption[]>>(contents);

            foreach (var kvp in customOptionsLookup)
            {
                var options = new List<ViewportOption>();

                if (optionsLookup.ContainsKey(kvp.Key))
                {
                    options.AddRange(optionsLookup[kvp.Key]);
                }

                foreach (var option in kvp.Value)
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

                optionsLookup[kvp.Key] = options.ToArray();
            }

            return optionsLookup;
        }

        public ViewportDevice[] GetViewportDevices(string moduleId)
        {
            var path = "Data/Viewports/ViewportDevices.json";
            var contents = File.ReadAllText(path);
            var devices = JsonConvert.DeserializeObject<Dictionary<string, List<ViewportDevice>>>(contents);
            var customDevices = GetValue<Dictionary<string, List<ViewportDevice>>>(ProfileSettingsCategories.Viewports, SettingsKeys.ViewportDevices);
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

        public ViewportOption[] GetViewportOptionsByModuleId(string moduleId)
        {
            var optionsLookup = GetAllViewportOptions();

            if (optionsLookup.TryGetValue(moduleId, out var options))
            {
                return options;
            }

            return new ViewportOption[0];
        }

        private static string GetCategory(string id)
        {
            return OptionCategory.All.First(id.Contains);
        }

        private void OnProfilesChanged()
        {
            var handler = ProfilesChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectedProfileChanged()
        {
            var handler = SelectedProfileChanged;

            handler?.Invoke(this, new SelectedProfileChangedEventArgs(SelectedProfileName));
        }
    }
}