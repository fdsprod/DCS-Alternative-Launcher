using System;
using System.Collections.Generic;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Services.Settings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Services
{
    public interface IProfileSettingsService
    {
        string SelectedProfileName { get; set; }

        event EventHandler<SelectedProfileChangedEventArgs> SelectedProfileChanged;

        event EventHandler ProfilesChanged;

        object GetAdvancedOptionDefaultValue(string category, string optionId);

        Option[] GetAdvancedOptions(string category);

        DcsOptionsCategory[] GetDcsOptions();

        T GetValue<T>(string category, string key, T defaultValue = default(T));

        bool TryGetValue<T>(string category, string key, out T value);

        void SetValue(string category, string key, object value);

        void DeleteValue(string category, string key);

        ModuleViewportTemplate[] GetViewportTemplates();

        ModuleViewportTemplate GetViewportTemplateByModule(string moduleId);

        void RemoveViewportTemplate(string moduleId);

        void RemoveViewport(string moduleId, Viewport viewport);

        void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport);

        ModuleViewportTemplate[] GetDefaultViewportTemplates();

        ViewportDevice[] GetViewportDevices(string moduleId);

        ViewportOption[] GetViewportOptionsByModuleId(string moduleId);

        Dictionary<string, ViewportOption[]> GetAllViewportOptions();

        void ClearViewports(string name, string moduleId);

        void RemoveProfile(string profileName);

        void AddProfile(SettingsProfile profile);
    }
}