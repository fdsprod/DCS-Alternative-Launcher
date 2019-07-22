namespace DCS.Alternative.Launcher.Services
{
    public interface ISettingsService
    {
        InstallLocation SelectedInstall
        {
            get;
            set;
        }

        InstallLocation[] Installations
        {
            get;
        }

        T GetValue<T>(string category, string key, T defaultValue = default(T));

        void SetValue(string category, string key, object value);
    }
}