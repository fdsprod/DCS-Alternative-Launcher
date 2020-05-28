namespace DCS.Alternative.Launcher.Services
{
    public interface ILauncherSettingsService
    {
        T GetValue<T>(string category, string key, T defaultValue = default(T));

        bool TryGetValue<T>(string category, string key, out T value);

        void SetValue(string category, string key, object value);

        void DeleteValue(string category, string key);
    }
}