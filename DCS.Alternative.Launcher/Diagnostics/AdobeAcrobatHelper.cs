using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using Microsoft.Win32;

namespace DCS.Alternative.Launcher.Diagnostics
{
    public static class AdobeAcrobatHelper
    {
        public static bool IsDCVersionInstalled()
        {
            var software = Registry.LocalMachine.OpenSubKey("Software");

            if (software == null)
            {
                return false;
            }

            RegistryKey adobe = null;

            if (Environment.Is64BitOperatingSystem)
            {
                var software64 = software.OpenSubKey("Wow6432Node");

                if (software64 != null)
                { 
                    adobe = software64.OpenSubKey("Adobe");
                }
            }

            if (adobe == null)
            {
                adobe = software.OpenSubKey("Adobe");
            }

            var subKey = adobe?.OpenSubKey("Acrobat Reader");

            if (subKey == null)
            {
                return false;
            }

            var versions = subKey.GetSubKeyNames();

            return versions.Any(v => v == "DC");

        }

        public static void ApplyProtectedModeFix()
        {
            var regAdobe = 
                Registry.CurrentUser.OpenSubKey(@"Software\Adobe\Acrobat Reader\DC\Privileged", true) ??
                Registry.CurrentUser.CreateSubKey(@"Software\Adobe\Acrobat Reader\DC\Privileged", true);

            regAdobe?.SetValue("bProtectedMode", 0);
        }

        public static bool IsProtectedModeDisabled()
        {
            var software = Registry.LocalMachine.OpenSubKey("Software");

            if (software == null)
            {
                return false;
            }

            RegistryKey adobe = null;

            if (Environment.Is64BitOperatingSystem)
            {
                var software64 = software.OpenSubKey("Wow6432Node");

                if (software64 != null)
                {
                    adobe = software64.OpenSubKey("Adobe");
                }
            }

            if (adobe == null)
            {
                Tracer.Warn("Unable to find adobe registry keys.");
                return false;
            }

            var subKey = adobe.OpenSubKey(@"Acrobat Reader\DC\Privileged", false);

            return (int)(subKey?.GetValue("bProtectedMode", 1) ?? 1) == 0;
        }
    }
}
