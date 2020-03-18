using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;
using DCS.Alternative.Launcher.Windows.FirstUse;
using DCS.Alternative.Launcher.Wizards;
using DCS.Alternative.Launcher.Wizards.Steps;
using DCS.Alternative.Launcher.Wizards.Steps.FirstUse;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Viewports
{
    public class ViewportSettingsViewModel : SettingsCategoryViewModelBase
    {
        public ViewportSettingsViewModel(SettingsController controller)
            : base("    VIEWPORTS", controller)
        {
            RemoveModuleViewportCommand = new ReactiveCommand(SelectedModuleViewport.Select(i => i != null), false);
            RemoveModuleViewportCommand.Subscribe(OnRemoveModuleViewport);

            AddModuleViewportCommand.Subscribe(OnAddModuleViewport);
            EditViewportsCommand.Subscribe(OnEditViewports);
            GenerateMonitorConfigCommand.Subscribe(OnGenerateMonitorConfig);
            MonitorSetupWizardCommand.Subscribe(OnMonitorSetupWizard);
        }

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

        public ReactiveCommand MonitorSetupWizardCommand
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

        public ReactiveCommand<ModuleViewportModel> EditViewportsCommand
        {
            get;
        } = new ReactiveCommand<ModuleViewportModel>();

        private async void OnAddModuleViewport()
        {
            try
            {
                var deviceViewportMonitorIds = Controller.GetDeviceViewportMonitorIds();

                if (deviceViewportMonitorIds.Length == 0)
                {
                    if (MessageBoxEx.Show("You have not defined a screen for device viewports.  Do you want to do that now?", "Device Viewports", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        using (var container = Controller.GetChildContainer())
                        {
                            var firstUseWizard = new Wizard();
                            var viewModel = new WizardViewModel(container,
                                new SelectGameViewportScreensStepViewModel(container),
                                new SelectUIViewportScreensStepViewModel(container),
                                new SelectDeviceViewportScreensStepViewModel(container));

                            firstUseWizard.DataContext = viewModel;
                            firstUseWizard.ShowDialog();
                        }

                        deviceViewportMonitorIds = Controller.GetDeviceViewportMonitorIds();

                        if (deviceViewportMonitorIds.Length == 0)
                        {
                            MessageBoxEx.Show("You must define a screen for device viewports before adding viewports for a module.", "Device Viewports");
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                var selectModuleDialog = new SelectModuleDialog();
                var viewportTemplates = Controller.GetDefaultViewportTemplates();

                foreach (var i in await Controller.GetInstalledAircraftModulesAsync())
                {
                    if (viewportTemplates.All(vt => vt.ModuleId != i.ModuleId))
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
                var templates = Controller.GetDefaultViewportTemplatesForModule(module.ModuleId);

                if (templates.Length == 0)
                {
                    MessageBoxEx.Show($"There are no default templates for the {module.DisplayName}.  An empty template will be created.", "No Template Defined");

                    var viewports = await Controller.EditViewportsAsync(module.DisplayName, null, module, new Viewport[0]);
                    Controller.SaveViewports(module.DisplayName, module.ModuleId, viewports);
                }
                else if(templates.Length > 1)
                {
                    var template = Controller.ShowTemplateSelection(templates);

                    if (template != null)
                    {
                        var viewports = await Controller.EditViewportsAsync(template.TemplateName, template.ExampleImageUrl, module, template.Viewports.ToArray());
                        Controller.SaveViewports(template.TemplateName, module.ModuleId, viewports);
                    }
                    else
                    {
                        var viewports = await Controller.EditViewportsAsync(module.DisplayName, null, module, new Viewport[0]);
                        Controller.SaveViewports(module.DisplayName, module.ModuleId, viewports);
                    }
                }
                else 
                {
                    if (MessageBoxEx.Show($"Would you like to start with the default template {templates[0].TemplateName}?", "Default Template", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var viewports = await Controller.EditViewportsAsync(templates[0].TemplateName, templates[0].ExampleImageUrl, module, templates[0].Viewports.ToArray());
                        Controller.SaveViewports(templates[0].TemplateName, module.ModuleId, viewports);
                    }
                    else
                    {

                        var viewports = await Controller.EditViewportsAsync(module.DisplayName, null, module, new Viewport[0]);
                        Controller.SaveViewports(module.DisplayName, module.ModuleId, viewports);
                    }
                }

                await PopulateViewportsAsync();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private async void OnMonitorSetupWizard()
        {
            Controller.ShowMonitorSetupWizard();

            await PopulateViewportsAsync();
        }

        private async void OnEditViewports(ModuleViewportModel value)
        {
            try
            {
                var viewports = await Controller.EditViewportsAsync(value);
                Controller.SaveViewports(value.Name.Value, value.Module.Value.ModuleId, viewports);
                await PopulateViewportsAsync();
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
                try
                {
                    dispatcher.Invoke(() => ModuleViewports.Clear());

                    var viewportTemplates = Controller.GetViewportTemplates();
                    var installedModules = await Controller.GetInstalledAircraftModulesAsync();

                    await dispatcher.InvokeAsync(() =>
                    {
                        foreach (var template in viewportTemplates)
                        {
                            var module = installedModules.First(m => m.ModuleId == template.ModuleId);
                            var model = new ModuleViewportModel(template.TemplateName, template.ExampleImageUrl, module, template.Viewports.ToArray());

                            model.IsValidSetup.Value = Controller.IsValidViewports(model.Viewports.ToArray());

                            ModuleViewports.Add(model);
                        }
                    });
                }
                catch (Exception e)
                {
                    Tracer.Error(e);
                }
            });
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
                Controller.RemoveViewportTemplate(moduleViewport.Module.Value.ModuleId);
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
                await Controller.GenerateMonitorConfigAsync();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }
    }
}