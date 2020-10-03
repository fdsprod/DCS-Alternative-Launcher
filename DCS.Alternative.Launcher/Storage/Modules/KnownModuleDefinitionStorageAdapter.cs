using DCS.Alternative.Launcher.DomainObjects;
using System;
using System.Collections.Generic;
using DCS.Alternative.Launcher.Storage.Modules.Strategies;

namespace DCS.Alternative.Launcher.Storage.Modules
{
    public static class KnownModuleDefinitionStorageAdapter
    {
        public static IEnumerable<KnownModuleDefinition> GetAll()
        {
            return KnownModuleDefinitionStorageStrategy.GetAll();
        }

    }
}
