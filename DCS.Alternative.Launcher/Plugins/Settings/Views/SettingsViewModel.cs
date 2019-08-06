using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;
using Application = System.Windows.Application;
using Screen = WpfScreenHelper.Screen;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly SettingsController _controller;
        private readonly IDcsWorldService _dscWorldService;
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<SettingsController>();

            _settingsService = container.Resolve<ISettingsService>();
            _dscWorldService = container.Resolve<IDcsWorldService>();

            RemoveInstallationCommand = new ReactiveCommand(SelectedInstall.Select(i => i != null), false);
            RemoveModuleViewportCommand = new ReactiveCommand(SelectedModuleViewport.Select(i => i != null), false);

            DetectInstallationsCommand.Subscribe(OnDetectInstallations);
            RemoveInstallationCommand.Subscribe(OnRemoveInstallation);
            AddInstallationCommand.Subscribe(OnAddInstallation);

            RemoveModuleViewportCommand.Subscribe(OnRemoveModuleViewport);
            AddModuleViewportCommand.Subscribe(OnAddModuleViewport);

            EditViewportsCommand.Subscribe(OnEditViewports);

            GenerateMonitorConfigCommand.Subscribe(OnGenerateMonitorConfig);
        }

        public ReactiveProperty<bool> IsLoading
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveCollection<ModuleViewportModel> ModuleViewports
        {
            get;
        } = new ReactiveCollection<ModuleViewportModel>();

        public ReactiveProperty<ModuleViewportModel> SelectedModuleViewport
        {
            get;
        } = new ReactiveProperty<ModuleViewportModel>();

        public ReactiveCommand GenerateMonitorConfigCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand RemoveInstallationCommand
        {
            get;
        }

        public ReactiveCommand AddInstallationCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand RemoveModuleViewportCommand
        {
            get;
        }

        public ReactiveCommand AddModuleViewportCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand DetectInstallationsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand<ModuleViewportModel> EditViewportsCommand
        {
            get;
        } = new ReactiveCommand<ModuleViewportModel>();

        public ReactiveCollection<InstallLocation> Installations
        {
            get;
        } = new ReactiveCollection<InstallLocation>();

        public ReactiveProperty<InstallLocation> SelectedInstall
        {
            get;
        } = new ReactiveProperty<InstallLocation>();

        private async void OnAddModuleViewport()
        {
            try
            {
                var deviceViewportMonitorIds = _settingsService.GetValue(SettingsCategories.Viewports, SettingsKeys.DeviceViewportsDisplays, new string[0]);

                if (deviceViewportMonitorIds.Length == 0)
                {
                    if (MessageBoxEx.Show("You have not defined a screen for device viewports.  Do you want to do that now?", "Device Viewport Screen") == MessageBoxResult.Yes)
                    {
                        //TODO Wizard...
                    }
                    else
                    {
                        return;
                    }
                }

                var selectModuleDialog = new SelectModuleDialog();
                var viewportTemplates = _settingsService.GetViewportTemplates();

                foreach (var i in await _dscWorldService.GetInstalledAircraftModulesAsync())
                {
                    if (viewportTemplates.Any(vt => vt.ModuleId == i.ModuleId))
                    {
                        continue;
                    }

                    selectModuleDialog.Modules.Add(i);
                }

                selectModuleDialog.SelectedModule = selectModuleDialog.Modules.First();
                selectModuleDialog.Owner = Application.Current.MainWindow;

                if (!selectModuleDialog.ShowDialog() ?? false)
                {
                    return;
                }

                var module = selectModuleDialog.SelectedModule;
                var templates = _settingsService.GetDefaultViewportTemplates().Where(t => t.ModuleId == module.ModuleId).ToArray();
                var screens = Screen.AllScreens.Where(s => deviceViewportMonitorIds.Contains(s.DeviceName)).ToArray();
                var monitorDefinitions = screens.Select(s => new MonitorDefinition {MonitorId = s.DeviceName, DisplayWidth = (int) s.Bounds.Width, DisplayHeight = (int) s.Bounds.Height});

                if (templates.Length == 0)
                {
                    MessageBoxEx.Show($"There are no default templates for the {module.DisplayName}.  An empty template will be created.", "No Template Defined");

                    var model =
                        new ModuleViewportModel(
                            module.DisplayName,
                            null,
                            module,
                            monitorDefinitions,
                            new Viewport[0]);

                    var viewports = await _controller.EditViewportsAsync(model);

                    await SaveViewportsAsync(module.DisplayName, module.ModuleId, viewports);
                }
                else if (templates.Length > 1)
                {
                    //TODO: Show wizard for selection
                }
                else
                {
                    if (MessageBoxEx.Show($"Would you like to start with the default template {templates[0].TemplateName}?", "Default Template", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var viewports = templates[0].Viewports.ToArray();

                        Array.ForEach(viewports,v => v.MonitorId = screens[0].DeviceName);

                        var model =
                            new ModuleViewportModel(
                                templates[0].TemplateName,
                                templates[0].ExampleImageUrl,
                                module,
                                monitorDefinitions,
                                viewports);

                        viewports = await _controller.EditViewportsAsync(model);

                        await SaveViewportsAsync(templates[0].TemplateName, module.ModuleId, viewports);
                    }
                    else
                    {
                        var model =
                            new ModuleViewportModel(
                                module.DisplayName,
                                null,
                                module,
                                monitorDefinitions,
                                new Viewport[0]);

                        var viewports = await _controller.EditViewportsAsync(model);

                        await SaveViewportsAsync(module.DisplayName, module.ModuleId, viewports);
                    }
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private async Task SaveViewportsAsync(string name, string moduleId, Viewport[] viewports)
        {
            try
            {
                IsLoading.Value = true;

                foreach (var viewport in viewports)
                {
                    var screen = Screen.AllScreens.First(s => s.DeviceName == viewport.MonitorId);
                    _settingsService.UpsertViewport(name, moduleId, screen, viewport);
                }

                await PopulateViewportsAsync();
            }
            finally
            {
                IsLoading.Value = false;
            }
        }

        private async void OnEditViewports(ModuleViewportModel value)
        {
            try
            {
                var viewports = await _controller.EditViewportsAsync(value);

                await SaveViewportsAsync(value.Name.Value, value.Module.Value.ModuleId, viewports);
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        protected override async Task InitializeAsync()
        {
            await PopulateViewportsAsync();
            await base.InitializeAsync();
        }

        private async Task PopulateViewportsAsync()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            await Task.Run(async () =>
            {
                IsLoading.Value = true;

                try
                {
                    dispatcher.Invoke(() => ModuleViewports.Clear());

                    var viewportTemplates = _settingsService.GetViewportTemplates();
                    var installedModules = await _dscWorldService.GetInstalledAircraftModulesAsync();

                    await dispatcher.InvokeAsync(() =>
                    {
                        foreach (var template in viewportTemplates)
                        {
                            var module = installedModules.First(m => m.ModuleId == template.ModuleId);
                            ModuleViewports.Add(new ModuleViewportModel(template.TemplateName, template.ExampleImageUrl, module, template.Monitors, template.Viewports.ToArray()));
                        }
                    });
                }
                catch (Exception e)
                {
                    Tracer.Error(e);
                }
                finally
                {
                    IsLoading.Value = false;
                }
            });
        }

        public override Task ActivateAsync()
        {
            try
            {
                Installations.Clear();

                foreach (var install in _settingsService.GetInstallations())
                {
                    Installations.Add(install);
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
            finally
            {
                IsLoading.Value = false;
            }

            return base.ActivateAsync();
        }

        private void OnAddInstallation()
        {
            try
            {
                var folderBrowser = new FolderBrowserDialog
                {
                    Description = "Please select the DCS folder you wish to add...",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = false
                };

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    var selectedFolder = folderBrowser.SelectedPath;
                    var installation = new InstallLocation(selectedFolder);

                    Installations.Add(installation);

                    _settingsService.AddInstalls(installation.Directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private void OnRemoveInstallation()
        {
            try
            {
                var installation = SelectedInstall.Value;

                if (installation == null)
                {
                    return;
                }

                Installations.Remove(installation);
                _settingsService.RemoveInstalls(installation.Directory);
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private void OnDetectInstallations()
        {
            try
            {
                var installations = InstallationLocator.Locate();
                var addedInstallations = new List<string>();

                foreach (var installation in installations)
                {
                    if (Installations.All(i => i.ToString() != installation.ToString()))
                    {
                        Installations.Add(installation);
                        addedInstallations.Add(installation.Directory);
                    }
                }

                foreach (var directory in addedInstallations)
                {
                    _settingsService.AddInstalls(directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }


        private void OnRemoveModuleViewport()
        {
            try
            {
                var moduleViewport = SelectedModuleViewport.Value;

                if (moduleViewport == null)
                {
                    return;
                }

                ModuleViewports.Remove(moduleViewport);
                _settingsService.RemoveViewportTemplate(moduleViewport.Module.Value.ModuleId);
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private async void OnGenerateMonitorConfig()
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("_  = function(p) return p; end;");
                sb.AppendLine("name = _(\"monitor_config_DAL\");");
                sb.AppendLine("Description = _(\"Monitor-Config created by DCS Alternate Launcher\")");
                sb.AppendLine();
                sb.AppendLine("-- *************** Displays ***************");

                var screens = Screen.AllScreens.OrderBy(s => s.DeviceName).ToArray();
                var screensIndexByName = new Dictionary<string, int>();
                var usedScreens = new List<Screen>();

                sb.AppendLine("displays =");
                sb.AppendLine("{");

                for (var i = 0; i < screens.Length; i++)
                {
                    var screen = screens[i];

                    screensIndexByName.Add(screen.DeviceName, i + 1);

                    sb.AppendLine($"    [{i + 1}] = ");
                    sb.AppendLine("    {");
                    sb.AppendLine($"        -- {screen.DeviceName},");
                    sb.AppendLine($"        x = {screen.Bounds.X},");
                    sb.AppendLine($"        y = {screen.Bounds.Y},");
                    sb.AppendLine($"        width = {screen.Bounds.Width},");
                    sb.AppendLine($"        height = {screen.Bounds.Height}");
                    sb.AppendLine("    },");
                }

                sb.AppendLine("}");
                sb.AppendLine();

                sb.AppendLine("Viewports =");
                sb.AppendLine("{");
                sb.AppendLine("    Center =");
                sb.AppendLine("    {");
                sb.AppendLine($"        x = {Screen.PrimaryScreen.Bounds.X},");
                sb.AppendLine($"        y = {Screen.PrimaryScreen.Bounds.Y},");
                sb.AppendLine($"        width = {Screen.PrimaryScreen.Bounds.Width},");
                sb.AppendLine($"        height = {Screen.PrimaryScreen.Bounds.Height},");
                sb.AppendLine("        viewDx = 0,");
                sb.AppendLine("        viewDy = 0,");
                sb.AppendLine($"        aspect = {Screen.PrimaryScreen.Bounds.Width} / {Screen.PrimaryScreen.Bounds.Height},");
                sb.AppendLine("    },");
                sb.AppendLine("}");
                sb.AppendLine();
                sb.AppendLine("UIMainView = Viewports.Center");
                sb.AppendLine();

                usedScreens.Add(Screen.PrimaryScreen);

                var viewportTemplates = _settingsService.GetViewportTemplates();
                var resolutionWidth = Screen.PrimaryScreen.Bounds.Width;
                var resolutionHeight = Screen.PrimaryScreen.Bounds.Height;

                var installedModules = await _dscWorldService.GetInstalledAircraftModulesAsync();

                foreach (var template in viewportTemplates)
                {
                    var module = installedModules.FirstOrDefault(m => m.ModuleId == template.ModuleId);

                    if (module == null)
                    {
                        Tracer.Warn($"Could not patch viewport for module {template.ModuleId} because the module is not installed.");
                        continue;
                    }

                    sb.AppendLine($"-- *************** {module.DisplayName} ***************");
                    sb.AppendLine();
                    sb.AppendLine();

                    foreach (var viewport in template.Viewports)
                    {
                        var screen = screens.Single(s => s.DeviceName == viewport.MonitorId);
                        var displayIndex = screensIndexByName[screen.DeviceName];
                        var originalWidth = (double) viewport.OriginalDisplayWidth;
                        var originalHeight = (double) viewport.OriginalDisplayHeight;
                        var ratioX = viewport.X / originalWidth;
                        var ratioY = viewport.Y / originalHeight;
                        var ratioW = viewport.Width / originalWidth;
                        var ratioH = viewport.Height / originalHeight;

                        var x = (int) Math.Round(screen.Bounds.Width * ratioX, 0, MidpointRounding.AwayFromZero);
                        var y = (int) Math.Round(screen.Bounds.Height * ratioY, 0, MidpointRounding.AwayFromZero);
                        var w = (int) Math.Round(screen.Bounds.Width * ratioW, 0, MidpointRounding.AwayFromZero);
                        var h = (int) Math.Round(screen.Bounds.Height * ratioH, 0, MidpointRounding.AwayFromZero);

                        resolutionWidth = Math.Max(resolutionWidth, screen.Bounds.X + x + w);
                        resolutionHeight = Math.Max(resolutionHeight, screen.Bounds.Y + y + h);

                        sb.AppendLine($"--{viewport.RelativeInitFilePath}");
                        sb.AppendLine($"{module.ViewportPrefix}_{viewport.ViewportName} =");
                        sb.AppendLine("{");
                        sb.AppendLine($"    x = displays[{displayIndex}].x + {x},");
                        sb.AppendLine($"    y = displays[{displayIndex}].y + {y},");
                        sb.AppendLine($"    width = {w},");
                        sb.AppendLine($"    height = {h},");
                        sb.AppendLine("}");
                        sb.AppendLine();

                        if (!usedScreens.Contains(screen))
                        {
                            usedScreens.Add(screen);
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine();
                }

                var contents = sb.ToString();
                var install = _settingsService.SelectedInstall;
                var monitorConfigDirectory = Path.Combine(install.SavedGamesPath, "Config\\MonitorSetup");
                var monitorConfigPath = Path.Combine(monitorConfigDirectory, "monitor_config_DAL.lua");

                if (!Directory.Exists(monitorConfigDirectory))
                {
                    Directory.CreateDirectory(monitorConfigDirectory);
                }

                if (File.Exists(monitorConfigPath))
                {
                    if (MessageBoxEx.Show("Monitor config already exists.  Are you sure you want to override it?", "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        File.WriteAllText(monitorConfigPath, contents);
                    }
                }
                else
                {
                    File.WriteAllText(monitorConfigPath, contents);
                }

                MessageBoxEx.Show(@"Make sure you setup the proper Monitor config in DCS once it is started.

To do this go to Options -> System -> Monitors and change the drop down to ""monitor_config_DAL""
Then make sure you change your monitor resolution to " + resolutionWidth + "x" + resolutionHeight + ".");
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }
    }
}