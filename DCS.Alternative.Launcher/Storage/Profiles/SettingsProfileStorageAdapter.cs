using System.Collections.Generic;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Storage.Profiles.Strategies;

namespace DCS.Alternative.Launcher.Storage.Profiles
{
    public static class ProfileStorageAdapter
    {
        public static IEnumerable<Profile> GetAll()
        {
            return OfflineProfileStorageStrategy.GetAll();
        }

        public static Task PersistAsync(Profile profile)
        {
            return OfflineProfileStorageStrategy.PersistAsync(profile);
        }
    }
}
