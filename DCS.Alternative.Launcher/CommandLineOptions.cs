using CommandLine;

namespace DCS.Alternative.Launcher
{
    public class CommandLineOptions
    {
        [Option("no-analytics", Required = false, HelpText = "Turns off analytics tracking.")]
        public bool NoAnalytics { get; set; }
    }
}