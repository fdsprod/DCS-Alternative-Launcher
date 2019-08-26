using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace AutoUpdate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            var directory = Directory.GetCurrentDirectory();

            try
            {
                var exePath = Path.Combine(directory, "DCS Alternative Launcher.exe");
                var assembly = Assembly.LoadFrom(exePath);
                var version = assembly.GetName().Version;

                if (version.ToString() != "0.14.0.0" && (args == null || args.Length == 0))
                {
                    StartLauncher();
                    return;
                }
            }
            catch
            {

            }

            var updateFolder = Path.Combine(directory, "_update");

            if (!Directory.Exists(updateFolder))
            {
                StartLauncher();
                return;
            }

            App app = new App();
            app.Run();
        }

        public App()
        {
            InitializeComponent();
        }

        public static void StartLauncher()
        {
            var directory = Directory.GetCurrentDirectory();
            var exePath = System.IO.Path.Combine(directory, "DCS Alternative Launcher.exe");

            Process.Start(exePath);
        }
    }
}
