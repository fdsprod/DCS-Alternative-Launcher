using DCS.Alternative.Launcher.Modules;

namespace DCS.Alternative.Launcher.Services
{
    public interface ISettingsService
    {
        InstallLocation SelectedInstall { get; set; }

        void RemoveInstalls(params string[] directory);

        void AddInstalls(params string[] directory);

        ModuleViewport[] GetModuleViewports();

        void RemoveModuleViewports(Module module);

        void RemoveViewport(Module module, Viewport viewport);

        void UpsertViewport(Module module, Viewport viewport);

        InstallLocation[] GetInstallations();

        T GetValue<T>(string category, string key, T defaultValue = default(T));

        void SetValue(string category, string key, object value);
    }
}