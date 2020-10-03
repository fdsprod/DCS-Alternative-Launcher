using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.DomainObjects.Dcs;
using NLua;

namespace DCS.Alternative.Launcher.Lua
{
    public class AircraftModuleLuaContext : LuaContextBase
    {
        private readonly List<string> _autoUpdateModules = new List<string>()
        {
            "Su-25T",
            "TF-51D"
        };

        private readonly string _aircraftFolder;
        private readonly DcsVersion _version;

        public AircraftModuleLuaContext(InstallLocation install)
        {
            _autoUpdateModules.AddRange(install.AutoUpdateConfigModules);
            _aircraftFolder =Path.Combine(install.Directory, "Mods//aircraft");
            _version = install.Version;
        }

        public ModuleBase[] GetModules()
        {
            var aircraftFolders = Directory.GetDirectories(_aircraftFolder);
            var modules = new Dictionary<string, AircraftModule>();

            foreach (var folder in aircraftFolders)
            {
                var entryPath = Path.Combine(folder, "entry.lua");
                var entryDefinitions = Path.Combine("Data", "Lua", "entry_definitions.lua");

                DoFile(Path.Combine(ApplicationPaths.ApplicationPath, entryDefinitions));
                DoString($"__DCS_VERSION__ = \"{_version}\"");
                DoString($"current_mod_path = \"{folder.Replace("\\", "\\\\")}\"");

                var plugin = default(DcsPlugin);

                this["declare_plugin"] = new Action<string, LuaTable>((id, description) =>
                {
                    try
                    {
                        plugin = LuaConverter.ConvertTo<DcsPlugin>(description);
                    }
                    catch (Exception e)
                    {
                        Tracer.Warn(e.Message);
                    }
                });

                var makeFlyableAction = new Action<string, string, LuaTable, string>((name, __, ___, ____) =>
                {
                    if (plugin == null)
                    {
                        return;
                    }

                    if (!plugin.Installed)
                    {
                        return;
                    }

                    if ((plugin.DisplayName ?? string.Empty).Contains("_hornet"))
                    {
                        plugin.DisplayName = plugin.DisplayName.Split('_')[0];
                    }

                    var moduleId =
                        string.IsNullOrWhiteSpace(plugin.UpdateId)
                            ? plugin.FileMenuName
                            : plugin.UpdateId;

                    if (modules.ContainsKey($"{moduleId}_{name}"))
                    {
                        return;
                    }

                    var skinPath =
                        plugin.Skins.Any()
                            ? plugin.Skins[1]
                                .Dir
                            : string.Empty;


                    if (!string.IsNullOrEmpty(moduleId) && _autoUpdateModules.Contains(moduleId) && moduleId != "FC3")
                    {
                        var module = new AircraftModule(folder)
                        {
                            ModuleId = moduleId,
                            DisplayName = plugin.DisplayName,
                            LoadingImagePath = Path.Combine(folder, skinPath, "ME", "loading-window.png"),
                            MainMenuLogoPath = Path.Combine(folder, skinPath, "ME", "MainMenulogo.png"),
                            IconPath = Path.Combine(folder, skinPath, "icon.png"),
                        };

                        modules.Add($"{moduleId}_{name}", module);

                        Tracer.Debug($"Found module {plugin.DisplayName}.");
                    }
                    else if (moduleId == "FC3")
                    {
                        var module = new AircraftModule(folder)
                        {
                            ModuleId = moduleId,
                            DisplayName = plugin.DisplayName,
                            IsFC3 = true,
                            FC3ModuleId = name,
                            LoadingImagePath = Path.Combine(folder, skinPath, "ME", "loading-window.png"),
                            MainMenuLogoPath = Path.Combine(folder, skinPath, "ME", "MainMenulogo.png"),
                            IconPath = Path.Combine(folder, skinPath, "icon.png"),
                        };

                        modules.Add($"{moduleId}_{name}", module);

                        Tracer.Debug($"Found module {plugin.DisplayName} {name}.");
                    }
                    else
                    {
                        Tracer.Debug($"Not loading module '{moduleId} - {plugin.DisplayName}'");
                    }
                });

                this["make_flyable"] = makeFlyableAction;
                this["MAC_flyable"] = makeFlyableAction;

                try
                {
                    DoFile(entryPath);
                }
                catch (Exception e)
                {
                    Tracer.Error(e.Message);
                }
            }

            return modules.Values.ToArray();
        }
    }
}