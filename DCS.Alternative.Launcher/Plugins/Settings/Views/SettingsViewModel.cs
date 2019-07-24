using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Modules;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Settings;
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
            RemoveViewportCommand = new ReactiveCommand(SelectedModuleViewport.Select(i => i != null), false);

            DetectInstallationsCommand.Subscribe(OnDetectInstallations);
            RemoveInstallationCommand.Subscribe(OnRemoveInstallation);
            AddInstallationCommand.Subscribe(OnAddInstallation);
            RemoveViewportCommand.Subscribe(OnRemoveViewport);
            AddViewportCommand.Subscribe(OnAddViewport);
            EditViewportCommand.Subscribe(OnEditViewport);
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

        public ReactiveCommand RemoveInstallationCommand
        {
            get;
        }

        public ReactiveCommand AddInstallationCommand
        {
            get;
        }
            = new ReactiveCommand();

        public ReactiveCommand RemoveViewportCommand
        {
            get;
        }

        public ReactiveCommand AddViewportCommand
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
                    foreach (var install in _settingsService.GetInstallations())
                    {
                        Installations.Add(install);
                    }

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

        private async void OnAddViewport()
        {
            var modules = await _dscWorldService.GetInstalledAircraftModulesAsync();
            var viewModel = new EditModuleViewportWindowViewModel(_settingsService.SelectedInstall, modules);

            var view = new EditModuleViewportWindow();

            view.DataContext = viewModel;
            view.Owner = App.Current.MainWindow;

            if (view.ShowDialog() == true)
            {
                var moduleViewport = new ModuleViewport
                {
                    Module = viewModel.SelectedModule.Value,
                };

                moduleViewport.Viewports.Add(new Viewport
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
                });

                _settingsService.UpsertInstalls(moduleViewport);

                ModuleViewports.Add(new ModuleViewportModel(moduleViewport.Module, moduleViewport.Viewports));
            }
        }

        private async void OnEditViewport(Viewport viewport)
        {
            var model = ModuleViewports.FirstOrDefault(m => m.Viewports.Contains(viewport));
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
                var moduleViewport = new ModuleViewport
                {
                    Module = viewModel.SelectedModule.Value,
                };

                moduleViewport.Viewports.Add(new Viewport
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
                });

                _settingsService.UpsertInstalls(moduleViewport);

                ModuleViewports.Add(new ModuleViewportModel(moduleViewport.Module, moduleViewport.Viewports));
            }
        }

        private void OnRemoveViewport()
        {
            var moduleViewport = SelectedModuleViewport.Value;

            if (moduleViewport == null)
            {
                return;
            }

            ModuleViewports.Remove(moduleViewport);
            _settingsService.RemoveViewport(moduleViewport.Module.Value.ModuleId);
        }
    }
}