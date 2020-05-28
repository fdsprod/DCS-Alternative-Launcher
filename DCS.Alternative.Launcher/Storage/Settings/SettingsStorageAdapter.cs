using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Storage.Settings.Strategies;

namespace DCS.Alternative.Launcher.Storage.Settings
{
    public static class SettingsStorageAdapter
    {
        public static Dictionary<string, Dictionary<string, object>> GetAll()
        {
            return OfflineSettingsStorageStrategy.GetAll();
        }

        public static Task PersistAsync(Dictionary<string, Dictionary<string, object>> settings)
        {
            return OfflineSettingsStorageStrategy.PersistAsync(settings);
        }
    }
}
