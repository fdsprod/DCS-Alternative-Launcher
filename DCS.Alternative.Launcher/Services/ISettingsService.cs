using System.Collections.Generic;
using DCS.Alternative.Launcher.DomainObjects;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Services
{
    public interface ISettingsService
    {
        Option[] GetAdvancedOptions(string category);

        DcsOptionsCategory[] GetDcsOptions();

        InstallLocation SelectedInstall { get; set; }

        void RemoveInstalls(params string[] directory);

        void AddInstalls(params string[] directory);

        ModuleViewportTemplate[] GetViewportTemplates();

        ModuleViewportTemplate GetViewportTemplateByModule(string moduleId);

        void RemoveViewportTemplate(string moduleId);

        void RemoveViewport(string moduleId, Viewport viewport);

        void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport);

        InstallLocation[] GetInstallations();

        ModuleViewportTemplate[] GetDefaultViewportTemplates();

        ViewportDevice[] GetViewportDevices(string moduleId);

        ViewportOption[] GetViewportOptionsByModuleId(string moduleId);

        Dictionary<string, ViewportOption[]> GetAllViewportOptions();

        AdditionalResource[] GetAdditionalResourcesByModule(string moduleId);

        T GetValue<T>(string category, string key, T defaultValue = default(T));

        bool TryGetValue<T>(string category, string key, out T value);

        void SetValue(string category, string key, object value);

    }
}