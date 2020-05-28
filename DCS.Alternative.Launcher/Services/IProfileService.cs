using System.Collections.Generic;
using DCS.Alternative.Launcher.DomainObjects;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Services
{
    public interface IProfileService
    {
        string SelectedProfileName { get; set; }

        InstallLocation GetSelectedInstall();

        void SetSelectedInstall(InstallLocation install);
        
        object GetAdvancedOptionDefaultValue(string category, string optionId);

        Option[] GetAdvancedOptions(string category);

        DcsOptionsCategory[] GetDcsOptions();

        T GetValue<T>(string category, string key, T defaultValue = default(T));

        bool TryGetValue<T>(string category, string key, out T value);

        void SetValue(string category, string key, object value);

        void DeleteValue(string category, string key);

        void RemoveProfile(string profileName);

        void AddProfile(Profile profile);

        void RemoveInstalls(params string[] directories);

        void AddInstalls(params string[] directories);

        IEnumerable<InstallLocation> GetInstallations();
    }
}