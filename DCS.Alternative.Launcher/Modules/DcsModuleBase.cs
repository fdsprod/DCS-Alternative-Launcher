using System;
using DCS.Alternative.Launcher.Diagnostics.Trace;

namespace DCS.Alternative.Launcher.Modules
{
    public abstract class DcsModuleBase
    {
        public abstract string ExportPrefix
        {
            get;
        }

        public abstract ExportFile[] DefaultExports
        {
            get;
        }

        public virtual void PatchViewports(InstallLocation install)
        {
            foreach (var exportFile in DefaultExports)
            {
                if (!install.FileExists(exportFile.FileName))
                {
                    Tracer.Warn($"Module {ExportPrefix}: Unable to patch viewport(s) [{string.Join(",", exportFile.ExportNames)} in file {exportFile.FileName}.");
                    continue;
                }

                var contents = install.ReadAllText(exportFile.FileName);
                var isChanged = false;

                if (!contents.Contains("dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")"))
                {
                    Tracer.Info($"Adding ViewportHandling code to {exportFile.FileName}");
                    contents += Environment.NewLine + "dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")" + Environment.NewLine;
                    isChanged = true;
                }

                foreach (var exportName in exportFile.ExportNames)
                {
                    var code = $"try_find_assigned_viewport(\"{ExportPrefix}_{exportName}\")";

                    if (!contents.Contains(code))
                    {
                        Tracer.Info($"Adding viewport name assignment code to {exportFile.FileName}");
                        contents += Environment.NewLine + code + Environment.NewLine;
                        isChanged = true;
                    }
                }

                if (isChanged)
                {
                    Tracer.Info($"Saving {exportFile.FileName}");
                    install.WriteAllText(exportFile.FileName, contents);
                }
            }
        }
    }
}