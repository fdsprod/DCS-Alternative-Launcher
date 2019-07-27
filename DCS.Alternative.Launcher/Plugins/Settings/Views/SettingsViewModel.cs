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
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Settings;
using KeraLua;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsViewModel : NavigationAwareBase
    {
        private readonly IDcsWorldService _dscWorldService;
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dscWorldService = container.Resolve<IDcsWorldService>();

            RemoveInstallationCommand = new ReactiveCommand(SelectedInstall.Select(i => i != null), false);
            RemoveModuleViewportCommand = new ReactiveCommand(SelectedModuleViewport.Select(i => i != null), false);

            DetectInstallationsCommand.Subscribe(OnDetectInstallations);
            RemoveInstallationCommand.Subscribe(OnRemoveInstallation);
            AddInstallationCommand.Subscribe(OnAddInstallation);
            RemoveModuleViewportCommand.Subscribe(OnRemoveModuleViewport);
            AddModuleViewportCommand.Subscribe(OnAddModuleViewport);
            EditViewportCommand.Subscribe(OnEditViewport);
            AddViewportCommand.Subscribe(OnAddViewport);
            RemoveViewportCommand.Subscribe(OnRemoveViewport);
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

        public ReactiveCommand<ModuleViewportModel> AddViewportCommand
        {
            get;
        } = new ReactiveCommand<ModuleViewportModel>();

        public ReactiveCommand<Viewport> RemoveViewportCommand
        {
            get;
        } = new ReactiveCommand<Viewport>();

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
        }
            = new ReactiveCommand();

        public ReactiveCommand RemoveModuleViewportCommand
        {
            get;
        }

        public ReactiveCommand AddModuleViewportCommand
        {
            get;
        }
            = new ReactiveCommand();

        public ReactiveCommand DetectInstallationsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand<Viewport> EditViewportCommand
        {
            get;
        } = new ReactiveCommand<Viewport>();

        public ReactiveCollection<InstallLocation> Installations
        {
            get;
        } = new ReactiveCollection<InstallLocation>();

        public ReactiveProperty<InstallLocation> SelectedInstall
        {
            get;
        } = new ReactiveProperty<InstallLocation>();

        protected override Task InitializeAsync()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            Task.Run(async () =>
            {
                IsLoading.Value = true;

                try
                {
                    var moduleViewports = _settingsService.GetModuleViewports();

                    await dispatcher.InvokeAsync(() =>
                    {
                        foreach (var mv in moduleViewports)
                        {
                            ModuleViewports.Add(new ModuleViewportModel(mv.Module, mv.Viewports.ToArray()));
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

            return base.InitializeAsync();
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

        private void OnRemoveInstallation()
        {
            var installation = SelectedInstall.Value;

            if (installation == null)
            {
                return;
            }

            Installations.Remove(installation);
            _settingsService.RemoveInstalls(installation.Directory);
        }

        private void OnDetectInstallations()
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

        private async void OnAddModuleViewport()
        {
            var modules = await _dscWorldService.GetInstalledAircraftModulesAsync();
            var viewModel = new EditModuleViewportWindowViewModel(_settingsService.SelectedInstall, modules);

            var view = new EditModuleViewportWindow();

            view.DataContext = viewModel;
            view.Owner = App.Current.MainWindow;

            if (view.ShowDialog() == true)
            {
                var viewport = new Viewport
                {
                    Bounds = new Bounds
                    {
                        X = viewModel.Bounds.Value.X.Value,
                        Y = viewModel.Bounds.Value.Y.Value,
                        Width = viewModel.Bounds.Value.Width.Value,
                        Height = viewModel.Bounds.Value.Height.Value,
                    },
                    InitFileName = viewModel.InitFilePath.Value,
                    Location = viewModel.IsNoLocationIndicator.Value
                        ? LocationIndicator.None
                        : viewModel.IsLeftLocationIndicator.Value
                            ? LocationIndicator.Left
                            : LocationIndicator.Right,
                    MonitorId = viewModel.SelectedMonitor.Value.Name,
                    Name = viewModel.ViewportName.Value
                };

                _settingsService.UpsertViewport(viewModel.SelectedModule.Value, viewport);

                ModuleViewports.Add(new ModuleViewportModel(viewModel.SelectedModule.Value, new [] { viewport }));
            }
        }

        private async void OnEditViewport(Viewport viewport)
        {
            var model = ModuleViewports.FirstOrDefault(m => m.Viewports.Contains(viewport));
            await EditViewportAsync(viewport, model);
        }

        private async Task<bool> EditViewportAsync(Viewport viewport, ModuleViewportModel model)
        {
            var modules = await _dscWorldService.GetInstalledAircraftModulesAsync();
            var viewModel = new EditModuleViewportWindowViewModel(
                _settingsService.SelectedInstall,
                modules,
                model.Module.Value.ModuleId,
                viewport.MonitorId,
                viewport.InitFileName,
                viewport.Name,
                viewport.Location,
                viewport.Bounds);

            var view = new EditModuleViewportWindow();

            view.DataContext = viewModel;
            view.Owner = App.Current.MainWindow;

            //TODO: Revert if cancelled
            if (view.ShowDialog() == true)
            {
                viewport.Bounds = new Bounds
                {
                    X = viewModel.Bounds.Value.X.Value,
                    Y = viewModel.Bounds.Value.Y.Value,
                    Width = viewModel.Bounds.Value.Width.Value,
                    Height = viewModel.Bounds.Value.Height.Value,
                };
                viewport.InitFileName = viewModel.InitFilePath.Value;
                viewport.Location =
                    viewModel.IsNoLocationIndicator.Value
                        ? LocationIndicator.None
                        : viewModel.IsLeftLocationIndicator.Value
                            ? LocationIndicator.Left
                            : LocationIndicator.Right;
                viewport.MonitorId = viewModel.SelectedMonitor.Value.Name;
                viewport.Name = viewModel.ViewportName.Value;
            
                _settingsService.UpsertViewport(viewModel.SelectedModule.Value, viewport);

                return true;
            }

            return false;
        }

        private void OnRemoveModuleViewport()
        {
            var moduleViewport = SelectedModuleViewport.Value;

            if (moduleViewport == null)
            {
                return;
            }

            ModuleViewports.Remove(moduleViewport);
            _settingsService.RemoveModuleViewports(moduleViewport.Module.Value);
        }

        private async void OnAddViewport(ModuleViewportModel model)
        {
            var viewport = new Viewport();

            if (await EditViewportAsync(viewport, model))
            {
                model.Viewports.Add(viewport);
            }
        }

        private void OnRemoveViewport(Viewport viewport)
        {
            var mvm = ModuleViewports.FirstOrDefault(m => m.Viewports.Contains(viewport));

            if (mvm != null)
            {
                mvm.Viewports.Remove(viewport);
                _settingsService.RemoveViewport(mvm.Module.Value, viewport);
            }
        }

        private void OnGenerateMonitorConfig()
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
                sb.AppendLine($"    {{");
                sb.AppendLine($"        -- {screen.DeviceName},");
                sb.AppendLine($"        x = {screen.Bounds.X},");
                sb.AppendLine($"        y = {screen.Bounds.Y},");
                sb.AppendLine($"        width = {screen.Bounds.Width},");
                sb.AppendLine($"        height = {screen.Bounds.Height}");
                sb.AppendLine($"    }},");
            }

            sb.AppendLine("}");
            sb.AppendLine();

            sb.AppendLine($"Viewports =");
            sb.AppendLine($"{{");
            sb.AppendLine($"    Center =");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        x = {Screen.PrimaryScreen.Bounds.X},");
            sb.AppendLine($"        y = {Screen.PrimaryScreen.Bounds.Y},");
            sb.AppendLine($"        width = {Screen.PrimaryScreen.Bounds.Width},");
            sb.AppendLine($"        height = {Screen.PrimaryScreen.Bounds.Height},");
            sb.AppendLine($"        viewDx = 0,");
            sb.AppendLine($"        viewDy = 0,");
            sb.AppendLine($"        aspect = {Screen.PrimaryScreen.Bounds.Width} / {Screen.PrimaryScreen.Bounds.Height},");
            sb.AppendLine($"    }},");
            sb.AppendLine($"}}");
            sb.AppendLine();
            sb.AppendLine("UIMainView = Viewports.Center");
            sb.AppendLine();

            usedScreens.Add(Screen.PrimaryScreen);

            var moduleViewports = _settingsService.GetModuleViewports();
            var resolutionWidth = Screen.PrimaryScreen.Bounds.Width;
            var resolutionHeight = Screen.PrimaryScreen.Bounds.Height;

            foreach (var module in moduleViewports)
            {
                sb.AppendLine($"-- *************** {module.Module.DisplayName} ***************");
                sb.AppendLine();
                sb.AppendLine();

                foreach (var viewport in module.Viewports)
                {
                    var screen = screens.Single(s => s.DeviceName == viewport.MonitorId);
                    var displayIndex = screensIndexByName[screen.DeviceName];

                    resolutionWidth = Math.Max(resolutionWidth, screen.Bounds.X + viewport.Bounds.X + viewport.Bounds.Width);
                    resolutionHeight = Math.Max(resolutionHeight, screen.Bounds.Y + viewport.Bounds.Y + viewport.Bounds.Height);

                    sb.AppendLine($"--{viewport.InitFileName}");
                    sb.AppendLine($"{module.Module.ViewportPrefix}_{viewport.Name} =");
                    sb.AppendLine($"{{");
                    sb.AppendLine($"    x = displays[{displayIndex}].x + {viewport.Bounds.X},");
                    sb.AppendLine($"    y = displays[{displayIndex}].y + {viewport.Bounds.Y},");
                    sb.AppendLine($"    width = {viewport.Bounds.Width},");
                    sb.AppendLine($"    height = {viewport.Bounds.Height},");
                    sb.AppendLine($"}}");
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
            var monitorConfigPath = Path.Combine(install.SavedGamesPath, "Config\\MonitorSetup\\monitor_config_DAL.lua");

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
    }
}