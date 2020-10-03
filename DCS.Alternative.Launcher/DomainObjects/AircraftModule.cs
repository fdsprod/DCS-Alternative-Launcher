using System.IO;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public sealed class AircraftModule : ModuleBase
    {
        public string CockpitScriptsFolderPath
        {
            get { return Path.Combine(BaseFolderPath, "Cockpit\\Scripts"); }
        }

        public AircraftModule(string baseFolderPath)
            : base(baseFolderPath, ModuleClassification.Aircraft)
        {
        }
    }
}