using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace DCS.Alternative.Launcher
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(App.Start)
                .WithNotParsed(_ => App.Start(new CommandLineOptions()));
        }

    }
}
