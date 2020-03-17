using System.Collections.Generic;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Storage.Profiles
{
    public static class SettingsProfileStorageAdapter
    {
        public static IEnumerable<SettingsProfile> GetAll()
        {
            return OfflineSettingsProfileStorageStrategy.GetAll();
        }

        public static Task PersistAsync(SettingsProfile profile)
        {
            return OfflineSettingsProfileStorageStrategy.PersistAsync(profile);
        }
    }
}
