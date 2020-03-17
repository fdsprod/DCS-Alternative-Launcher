using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Storage.Profiles
{
    public static class OfflineSettingsProfileStorageStrategy
    {
        public static IEnumerable<SettingsProfile> GetAll()
        {
            var profiles = new List<SettingsProfile>();

            foreach (var profileJson in Directory.GetFiles(ApplicationPaths.ProfilesPath, "*.json"))
            {
                var contents = File.ReadAllText(profileJson);
                var profile = JsonConvert.DeserializeObject<SettingsProfile>(contents);

                profile.Path = profileJson;

                profiles.Add(profile);
            }

            return profiles;
        }

        public static async Task PersistAsync(SettingsProfile profile)
        {
            await File.WriteAllTextAsync(profile.Path, JsonConvert.SerializeObject(profile, Formatting.Indented)).ConfigureAwait(false);
        }
    }
}