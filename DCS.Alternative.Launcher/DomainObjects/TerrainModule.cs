namespace DCS.Alternative.Launcher.DomainObjects
{
    public sealed class TerrainModule : ModuleBase
    {
        public TerrainModule(string baseFolderPath)
            : base(baseFolderPath, ModuleClassification.Terrain)
        {
        }
    }
}