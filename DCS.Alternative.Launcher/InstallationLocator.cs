using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using Microsoft.Win32;

namespace DCS.Alternative.Launcher
{
    internal static class InstallationLocator
    {
        static InstallationLocator()
        {
            KnownInstallationRegistryKeys =
                new List<string>
                {
                    @"Eagle Dynamics\DCS World OpenBeta",
                    @"Eagle Dynamics\DCS World"
                };
        }

        public static List<string> KnownInstallationRegistryKeys { get; }

        public static IEnumerable<InstallLocation> Locate()
        {
            var installations = new List<InstallLocation>();

            for (var i = 0; i < KnownInstallationRegistryKeys.Count; i++)
            {
                var path = GetPath(KnownInstallationRegistryKeys[i]);

                if (!string.IsNullOrEmpty(path) && installations.All(ins => ins.Directory != path))
                {
                    Tracer.Info($"Found DCS path ({path}) from registry key {KnownInstallationRegistryKeys[i]}");
                    installations.Add(new InstallLocation(path));
                }
            }

            return installations;
        }

        private static string GetPath(string subName)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\{subName}");

                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\{subName}");

                    if (key == null)
                    {
                        return null;
                    }
                }

                var path = key.GetValue("Path") as string;

                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                {
                    return null;
                }

                return path;
            }
            catch (Exception e)
            {
                Tracer.Error(e);
                return null;
            }
        }
    }
}