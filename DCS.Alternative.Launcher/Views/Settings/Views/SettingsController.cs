using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Lua;
using DCS.Alternative.Launcher.Services;
using IContainer = DCS.Alternative.Launcher.ServiceModel.IContainer;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsController
    {
        private readonly IContainer _container;
        private readonly IDcsWorldService _dcsWorldService;
        private readonly IProfileService _profileService;

        public SettingsController(IContainer container)
        {
            _container = container;
            _dcsWorldService = container.Resolve<IDcsWorldService>();
            _profileService = container.Resolve<IProfileService>();
        }

        public IEnumerable<InstallLocation> GetInstallations()
        {
            return _profileService.GetInstallations();
        }

        public void AddInstalls(string directory)
        {
            _profileService.AddInstalls(directory);
        }

        public void RemoveInstalls(string installationDirectory)
        {
            _profileService.RemoveInstalls(installationDirectory);
        }

        public IContainer GetChildContainer()
        {
            return _container.GetChildContainer();
        }
        public Task<ModuleBase[]> GetInstalledAircraftModulesAsync()
        {
            return _dcsWorldService.GetInstalledAircraftModulesAsync();
        }
        public Option[] GetAdvancedOptions(string optionsCategory)
        {
            var options = _profileService.GetAdvancedOptions(optionsCategory);

            foreach (var option in options)
            {
                if (_profileService.TryGetValue<object>(ProfileCategories.AdvancedOptions, option.Id, out var value))
                {
                    option.Value = value;
                }
            }

            return options;
        }

        public GameOptionsCategory[] GetDcsOptionCategories()
        {
            return _profileService.GetGameOptions();
        }

        public GameOptionsCategory[] GetDcsCategoryOptionForInstall(InstallLocation install, bool isVr)
        {
            var categories = _profileService.GetGameOptions();

            using (var context = new OptionLuaContext(install))
            {
                foreach (var category in categories)
                {
                    foreach (var option in category.Options)
                    {
                        if (!_profileService.TryGetValue<object>(ProfileCategories.GameOptions, option.Id, out var value))
                        {
                            value = context.GetValue(category.Id, option.Id);
                        }

                        if (value != null)
                        {
                            var valueStr = value.ToString();
                            var valueType = option.Value.GetType();
                            var converter = TypeDescriptor.GetConverter(valueType);

                            try
                            {
                                option.Value = converter.ConvertFromString(valueStr);
                            }
                            catch (Exception e)
                            {
                                Tracer.Error(e, $"An error occured while trying to convert the value {valueStr} to type {valueType} for option id {option.Id}.");
                            }
                        }
                        else
                        {
                            Tracer.Warn($"Unable to find option value for {category.DisplayName} {option.Id}.  Using default value {option.Value}");
                        }
                    }
                }
            }

            return categories;
        }

        public void UpsertAdvancedOption(string id, object value)
        {
            _profileService.SetValue(ProfileCategories.AdvancedOptions, id, value);
        }

        public InstallLocation GetCurrentInstall()
        {
            return _profileService.GetSelectedInstall();
        }

        public void UpsertDcsOption(string categoryId, string id, object value, bool isVr)
        {
            _profileService.SetValue(ProfileCategories.GameOptions, id, value);
        }

        public object ResetAdvancedOptionValue(string categoryId, string optionId)
        {
            var defaultValue = _profileService.GetAdvancedOptionDefaultValue(categoryId, optionId);

            _profileService.DeleteValue(ProfileCategories.AdvancedOptions, optionId);

            return defaultValue;
        }

        public object ResetDcsOption(string categoryId, string optionId)
        {
            var categories = _profileService.GetGameOptions();
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            var option = category.Options.FirstOrDefault(o => o.Id == optionId);

            _profileService.DeleteValue(ProfileCategories.GameOptions, optionId);

            return option?.Value;
        }

        public void SaveDcsOptions()
        {
            _dcsWorldService.WriteOptionsAsync();
        }
        
        public void RemoveProfile(string profileName)
        {
            _profileService.RemoveProfile(profileName);
        }
    }
}