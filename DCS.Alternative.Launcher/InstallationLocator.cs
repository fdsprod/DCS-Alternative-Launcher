using System;
using System.Collections.Generic;
using System.IO;
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

        public static List<string> KnownInstallationRegistryKeys
        {
            get;
        }

        public static IEnumerable<InstallLocation> Locate()
        {
            var installations = new List<InstallLocation>();

            for (var i = 0; i < KnownInstallationRegistryKeys.Count; i++)
            {
                var exePath = GetExePath(KnownInstallationRegistryKeys[i]);

                if (!string.IsNullOrEmpty(exePath) && !installations.Contains(exePath)) installations.Add(exePath);
            }

            return installations;
        }

        private static string GetExePath(string subName)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\{0}", subName));

                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\{0}", subName));

                    if (key == null) return null;
                }

                var path = key.GetValue("Path") as string;

                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return null;

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