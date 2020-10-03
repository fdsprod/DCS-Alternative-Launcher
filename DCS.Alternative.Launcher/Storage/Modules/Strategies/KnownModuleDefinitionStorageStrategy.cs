using DCS.Alternative.Launcher.DomainObjects;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.Storage.Modules.Strategies
{
    public static class KnownModuleDefinitionStorageStrategy
    {
        public static IEnumerable<KnownModuleDefinition> GetAll()
        {
            var path = Path.Combine(ApplicationPaths.ApplicationPath, "Data", "KnownModules.json");

            if (!File.Exists(path))
            {
                return new KnownModuleDefinition[0];
            }

            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<KnownModuleDefinition[]>(json);

        }
    }
}
