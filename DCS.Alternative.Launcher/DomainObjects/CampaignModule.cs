namespace DCS.Alternative.Launcher.DomainObjects
{
    public sealed class CampaignModule : ModuleBase
    {
        public CampaignModule(string baseFolderPath)
            : base(baseFolderPath, ModuleClassification.Campaign)
        {
        }
    }
}