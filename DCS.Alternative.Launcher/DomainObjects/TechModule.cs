namespace DCS.Alternative.Launcher.DomainObjects
{
    public sealed class TechModule : ModuleBase
    {
        public TechModule(string baseFolderPath)
            : base(baseFolderPath, ModuleClassification.Tech)
        {
        }
    }
}