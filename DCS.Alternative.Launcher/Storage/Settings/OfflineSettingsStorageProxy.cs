using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Storage.Settings
{
    public static class OfflineSettingsStorageProxy
    { 
        public static Dictionary<string, Dictionary<string, object>> GetAll()
        {
            var path = Path.Combine(ApplicationPaths.StoragePath, "settings.json");

            if (!File.Exists(path))
            {
                return new Dictionary<string, Dictionary<string, object>>();
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);

        }

        public static async Task PersistAsync(Dictionary<string, Dictionary<string, object>> settings)
        {
            var path = Path.Combine(ApplicationPaths.StoragePath, "settings.json");
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(settings, Formatting.Indented)).ConfigureAwait(false);
        }
    }
}