using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Newtonsoft.Json;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Services
{
    internal class ViewportService : IViewportService
    {
        private readonly IDcsWorldService _dcsWorldService;
        private readonly IProfileService _profileService;

        public ViewportService(IContainer container)
        {
            _dcsWorldService = container.Resolve<IDcsWorldService>();
            _profileService = container.Resolve<IProfileService>();
        }

        public ModuleViewportTemplate[] GetViewportTemplates()
        {
            return _profileService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]);
        }

        public ModuleViewportTemplate GetViewportTemplateByModule(string moduleId)
        {
            return _profileService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]).FirstOrDefault(mv => mv.ModuleId == moduleId);
        }

        public void RemoveViewport(string moduleId, Viewport viewport)
        {
            var moduleViewports = _profileService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]);
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);

            viewport = mv?.Viewports.FirstOrDefault(v => v.ViewportName == viewport.ViewportName);

            mv?.Viewports.Remove(viewport);

            _profileService.SetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void ClearViewports(string name, string moduleId)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(_profileService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId && m.TemplateName == name);

            mv?.Viewports.Clear();

            _profileService.SetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }
        public void UpsertViewport(string name, string moduleId, Screen screen, Viewport viewport)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(_profileService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId && m.TemplateName == name);

            if (mv == null)
            {
                mv = new ModuleViewportTemplate
                {
                    TemplateName = name,
                    ModuleId = moduleId
                };

                moduleViewports.Add(mv);
            }
            else
            {
                mv.TemplateName = name;
            }

            var monitor = mv.Monitors.FirstOrDefault(m => m.MonitorId == screen.DeviceName);

            if (monitor == null)
            {
                monitor = new MonitorDefinition
                {
                    MonitorId = screen.DeviceName
                };

                mv.Monitors.Add(monitor);
            }

            monitor.DisplayWidth = (int)screen.Bounds.Width;
            monitor.DisplayHeight = (int)screen.Bounds.Height;

            var vp = mv.Viewports.FirstOrDefault(v => v.ViewportName == viewport.ViewportName);

            viewport.MonitorId = screen.DeviceName;

            mv.Viewports.Remove(vp);
            mv.Viewports.Add(viewport);

            _profileService.SetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }

        public void RemoveViewportTemplate(string moduleId)
        {
            var moduleViewports = new List<ModuleViewportTemplate>(_profileService.GetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, new ModuleViewportTemplate[0]));
            var mv = moduleViewports.FirstOrDefault(m => m.ModuleId == moduleId);

            moduleViewports.Remove(mv);

            _profileService.SetValue(ViewportProfileCategories.Viewports, ViewportSettingKeys.ModuleViewportTemplates, moduleViewports.ToArray());
        }


        public async Task WriteViewportOptionsAsync()
        {
            var install = _profileService.GetSelectedInstall();
            var modules = await _dcsWorldService.GetInstalledAircraftModulesAsync();

            foreach (var module in modules)
            {
                var options = GetViewportOptionsByModuleId(module.ModuleId);

                foreach (var option in options)
                {
                    if (!_profileService.TryGetValue<object>(string.Format(ViewportProfileCategories.ViewportOptionsFormat, module.ModuleId), option.Id, out var value))
                    {
                        continue;
                    }

                    var filePath = Path.Combine(install.Directory, option.FilePath);

                    if (!File.Exists(filePath))
                    {
                        Tracer.Warn($"Unable to find file \"{filePath}\", skipping output of option {option.Id} for module {module.ModuleId}.");
                        continue;
                    }

                    var fileContents = File.ReadAllText(filePath);
                    var regex = new Regex(option.Regex);
                    var match = regex.Match(fileContents);

                    if (!match.Success)
                    {
                        Tracer.Warn($"Unable to make a match on regex {option.Regex} for file \"{filePath}\", skipping output of option {option.Id} for module {module.ModuleId}.");
                        continue;
                    }

                    fileContents = fileContents.Remove(match.Index, match.Length);
                    fileContents = fileContents.Insert(match.Index, value is bool ? value.ToString().ToLower() : value.ToString());

                    File.WriteAllText(filePath, fileContents);
                }
            }
        }



        public ModuleViewportTemplate[] GetDefaultViewportTemplates()
        {
            var path = "Data/Viewports/ViewportTemplates.json";
            var contents = File.ReadAllText(path);
            var templates = JsonConvert.DeserializeObject<ModuleViewportTemplate[]>(contents).ToList();

            path = Path.Combine(ApplicationPaths.ViewportPath, "ViewportTemplates.json");
            contents = File.ReadAllText(path);

            var customTemplates = JsonConvert.DeserializeObject<ModuleViewportTemplate[]>(contents);

            foreach (var customTemplate in customTemplates)
            {
                var existingTemplate = templates.FirstOrDefault(t => t.TemplateName == customTemplate.TemplateName);

                if (existingTemplate != null)
                {
                    var index = templates.IndexOf(existingTemplate);
                    templates[index] = customTemplate;
                }
                else
                {
                    templates.Add(customTemplate);
                }
            }

            return templates.ToArray();
        }

        public Dictionary<string, ViewportOption[]> GetAllViewportOptions()
        {
            var path = "Data/Viewports/ViewportOptions.json";
            var contents = File.ReadAllText(path);
            var optionsLookup = JsonConvert.DeserializeObject<Dictionary<string, ViewportOption[]>>(contents);

            path = Path.Combine(ApplicationPaths.ViewportPath, "ViewportOptions.json");
            contents = File.ReadAllText(path);

            var customOptionsLookup = JsonConvert.DeserializeObject<Dictionary<string, ViewportOption[]>>(contents);

            foreach (var kvp in customOptionsLookup)
            {
                var options = new List<ViewportOption>();

                if (optionsLookup.ContainsKey(kvp.Key))
                {
                    options.AddRange(optionsLookup[kvp.Key]);
                }

                foreach (var option in kvp.Value)
                {
                    var existingOption = options.FirstOrDefault(o => o.Id == option.Id);

                    if (existingOption == null)
                    {
                        options.Add(option);
                    }
                    else
                    {
                        options[options.IndexOf(existingOption)] = option;
                    }
                }

                optionsLookup[kvp.Key] = options.ToArray();
            }

            return optionsLookup;
        }

        public ViewportDevice[] GetViewportDevices(string moduleId)
        {
            var path = "Data/Viewports/ViewportDevices.json";
            var contents = File.ReadAllText(path);
            var devices = JsonConvert.DeserializeObject<Dictionary<string, List<ViewportDevice>>>(contents);
            var customDevices = _profileService.GetValue<Dictionary<string, List<ViewportDevice>>>(ViewportProfileCategories.Viewports, ViewportSettingKeys.ViewportDevices);
            var results = new List<ViewportDevice>();

            if (devices.ContainsKey(moduleId))
            {
                results.AddRange(devices[moduleId]);
            }

            if (customDevices != null && customDevices.ContainsKey(moduleId))
            {
                results.AddRange(customDevices[moduleId]);
            }

            return results.ToArray();
        }

        public ViewportOption[] GetViewportOptionsByModuleId(string moduleId)
        {
            var optionsLookup = GetAllViewportOptions();

            if (optionsLookup.TryGetValue(moduleId, out var options))
            {
                return options;
            }

            return new ViewportOption[0];
        }

        public Task PatchViewportsAsync()
        {
            return Task.Run(async () =>
            {
                var install = _profileService.GetSelectedInstall();
                var viewportTemplates = GetViewportTemplates();
                var modules = await _dcsWorldService.GetInstalledAircraftModulesAsync();

                foreach (var template in viewportTemplates)
                {
                    var module = modules.FirstOrDefault(m => m.ModuleId == template.ModuleId);

                    if (module == null)
                    {
                        Tracer.Warn($"Could not patch viewport for module {template.ModuleId} because the module is not installed.");
                        return;
                    }

                    foreach (var viewport in template.Viewports)
                    {
                        if (string.IsNullOrEmpty(viewport.RelativeInitFilePath))
                        {
                            continue;
                        }

                        if (!install.FileExists(viewport.RelativeInitFilePath))
                        {
                            Tracer.Warn($"Module {template.ModuleId}: Unable to patch viewport(s) [{viewport.ViewportName} in file {viewport.RelativeInitFilePath}.");
                            continue;
                        }

                        var contents = install.ReadAllText(viewport.RelativeInitFilePath);
                        var isChanged = false;

                        if (!contents.Contains("dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")"))
                        {
                            Tracer.Info($"Adding ViewportHandling code to {viewport.RelativeInitFilePath}");
                            contents += Environment.NewLine + "dofile(LockOn_Options.common_script_path..\"ViewportHandling.lua\")" + Environment.NewLine;
                            isChanged = true;
                        }

                        var originalCode = $"try_find_assigned_viewport(\"{viewport.ViewportName}\")";
                        var code = $"try_find_assigned_viewport(\"{template.ViewportPrefix}_{viewport.ViewportName}\", \"{viewport.ViewportName}\")";

                        if (!contents.Contains(code))
                        {
                            Tracer.Info($"Adding viewport name assignment code \"{code}\" to {viewport.RelativeInitFilePath}");

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
                        else
                        {
                            Tracer.Warn($"Unable to write to {viewport.RelativeInitFilePath}, Original code \"{originalCode}\" not found.  Viewport {viewport.ViewportName} may not work.");
                        }

                        if (isChanged)
                        {
                            Tracer.Info($"Saving {viewport.RelativeInitFilePath}");
                            install.WriteAllText(viewport.RelativeInitFilePath, contents);
                        }
                    }
                }
            });
        }

    }
}