using System;
using System.Collections.Generic;
using System.IO;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;

namespace DCS.Alternative.Launcher.Modules
{
    public class Module
    {
        public string ViewportPrefix { get; set; }

        public string BaseFolderPath { get; set; }

        public string IconPath { get; set; }

        public string ModuleId { get; set; }

        public string DisplayName { get; set; }
    }

    public class ModuleViewport
    {
        public Module Module
        {
            get;
            set;
        }

        public List<Viewport> Viewports
        {
            get; set;
        } = new List<Viewport>();

        public virtual void PatchViewports(InstallLocation install)
        {
            foreach (var viewport in Viewports)
            {
                if (!install.FileExists(viewport.InitFileName))
                {
                    Tracer.Warn(
                        $"Module {Module.ModuleId}: Unable to patch viewport(s) [{viewport.Name} in file {viewport.InitFileName}.");
                    continue;
                }

                var contents = install.ReadAllText(viewport.InitFileName);
                var isChanged = false;

                if (!contents.Contains("dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")"))
                {
                    Tracer.Info($"Adding ViewportHandling code to {viewport.InitFileName}");
                    contents += Environment.NewLine +
                                "dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")" +
                                Environment.NewLine;
                    isChanged = true;
                }

                var originalCode = $"try_find_assigned_viewport(\"{viewport.Name}\")";
                
                var code =
                    viewport.Location == LocationIndicator.None
                        ? $"try_find_assigned_viewport(\"{Module.ViewportPrefix}_{viewport.Name}\")"
                        : viewport.Location == LocationIndicator.Left
                            ? $"if disposition == \"L\" then try_find_assigned_viewport(\"{Module.ViewportPrefix}_{viewport.Name}\") end"
                            : $"if disposition == \"R\" then try_find_assigned_viewport(\"{Module.ViewportPrefix}_{viewport.Name}\") end";

                if (!contents.Contains(code))
                {
                    Tracer.Info($"Adding viewport name assignment code to {viewport.InitFileName}");

                    if (contents.Contains(originalCode))
                    {
                        contents = contents.Replace(originalCode, code);
                    }
                    else
                    {
                        contents += Environment.NewLine + code + Environment.NewLine;
                    }

                    isChanged = true;
                }

                if (isChanged)
                {
                    Tracer.Info($"Saving {viewport.InitFileName}");
                    install.WriteAllText(viewport.InitFileName, contents);
                }
            }
        }
    }
}