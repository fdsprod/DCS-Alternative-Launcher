namespace DCS.Alternative.Launcher.Modules
{
    public class KA50Module : DcsModuleBase
    {
        public override ExportFile[] DefaultExports
        {
            get;
        } =
        {
            new ExportFile("Mods/aircraft/Ka-50/Cockpit/Scripts/SHKVAL/SHKVAL_init.lua", "SHKVAL"),
            new ExportFile("Mods/aircraft/Ka-50/Cockpit/Scripts/PVI/PVI_init.lua", "PVI"),
            new ExportFile("Mods/aircraft/Ka-50/Cockpit/Scripts/ABRIS/ABRIS_init.lua", "ABRIS"),
            new ExportFile("Mods/aircraft/Ka-50/Cockpit/Scripts/EKRAN/Indicator/Ekran_init.lua", "EKRAN"),
            new ExportFile("Mods/aircraft/Ka-50/Cockpit/Scripts/UV_26/UV_26_init.lua", "UV26")
        };

        public override string ExportPrefix
        {
            get { return "KA_50"; }
        }
    }
}