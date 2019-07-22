namespace DCS.Alternative.Launcher.Modules
{
    public class M2000CModule : DcsModuleBase
    {
        public override ExportFile[] DefaultExports
        {
            get;
        } =
        {
            new ExportFile("Mods/aircraft/M-2000C/Cockpit/VTB/VTB_init.lua", "RADAR"),
            new ExportFile("Mods/aircraft/M-2000C/Cockpit/RWR/RWR_init.lua", "RWR")
        };

        public override string ExportPrefix
        {
            get { return "M_2000C"; }
        }
    }
}