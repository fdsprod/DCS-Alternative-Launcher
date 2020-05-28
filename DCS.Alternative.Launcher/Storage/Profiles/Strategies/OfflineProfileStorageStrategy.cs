using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Storage.Profiles.Strategies
{
    public static class OfflineProfileStorageStrategy
    {
        public static IEnumerable<Profile> GetAll()
        {
            var profiles = new List<Profile>();

            foreach (var profileJson in Directory.GetFiles(ApplicationPaths.ProfilesPath, "*.json"))
            {
                var contents = File.ReadAllText(profileJson);
                var profile = JsonConvert.DeserializeObject<Profile>(contents);

                profiles.Add(profile);
            }

            return profiles;
        }

        public static async Task PersistAsync(Profile profile)
        {
            var path = Path.Combine(ApplicationPaths.ProfilesPath, $"{profile.Name}.json");
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(profile, Formatting.Indented)).ConfigureAwait(false);
        }
    }
}