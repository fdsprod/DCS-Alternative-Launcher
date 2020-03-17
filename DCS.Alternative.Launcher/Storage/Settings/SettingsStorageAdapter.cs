using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Storage.Settings
{
    public static class SettingsStorageAdapter
    {
        public static Dictionary<string, Dictionary<string, object>> GetAll()
        {
            return OfflineSettingsStorageProxy.GetAll();
        }

        public static Task PersistAsync(Dictionary<string, Dictionary<string, object>> settings)
        {
            return OfflineSettingsStorageProxy.PersistAsync(settings);
        }
    }
}
