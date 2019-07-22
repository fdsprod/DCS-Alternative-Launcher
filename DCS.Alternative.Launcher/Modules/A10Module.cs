namespace DCS.Alternative.Launcher.Modules
{
    public class A10Module : DcsModuleBase
    {
        public override ExportFile[] DefaultExports
        {
            get;
        } =
        {
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/MFCD/indicator/MFCD_right_init.lua", "RIGHT_MFCD"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/MFCD/indicator/MFCD_left_init.lua", "LEFT_MFCD"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/CMSC/indicator/CMSC_init.lua", "CMSC_SCREEN"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/CMSP/indicator/CMSP_init.lua", "CMSP_SCREEN"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/AN_ALR69V/indicator/AN_ALR69V_init.lua", "RWR_SCREEN"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/DigitalClock/Indicator/DIGIT_CLK_init.lua", "DIGIT_CLOCK"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/UHF_Radio/indicator/freq_status_init.lua", "UHF_FREQUENCY_STATUS"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/UHF_Radio/indicator/preset_channel_init.lua", "UHF_PRESET_CHANNEL"),
            new ExportFile("Mods/aircraft/A-10C/Cockpit/Scripts/UHF_Radio/indicator/repeater_init.lua", "UHF_REPEATER")
        };

        public override string ExportPrefix
        {
            get { return "A_10C"; }
        }
    }
}