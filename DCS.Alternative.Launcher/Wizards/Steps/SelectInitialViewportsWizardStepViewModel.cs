using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Settings;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Wizards.Steps
{
    public class SelectInitialViewportsWizardStepViewModel : WizardStepBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IDcsWorldService _dcsWorldService;

        public SelectInitialViewportsWizardStepViewModel(IContainer container)
            : base(container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }

        public override async Task ActivateAsync()
        {
            var defaultViewportTemplates = _settingsService.GetDefaultViewportTemplates();
            var installedModules = await _dcsWorldService.GetInstalledAircraftModulesAsync();
            var templates = defaultViewportTemplates.Where(vp => installedModules.Any(m => m.ModuleId == vp.ModuleId)).ToArray();

            if (templates.Length == 0)
            {
                await Controller.GoNextAsync(false);
                return;
            }

            foreach (var template in templates)
            {
                var module = installedModules.First(m => m.ModuleId == template.ModuleId);
                var model = new ModuleViewportModel(template.TemplateName, template.ExampleImageUrl, module, template.Viewports);

                ModuleViewports.Add(model);
            }

            await base.ActivateAsync();
        }

        public override bool Commit()
        {
            var deviceScreenId = _settingsService.GetValue<string[]>(SettingsCategories.Viewports, SettingsKeys.DeviceViewportsDisplays).First();
            var screen = Screen.AllScreens.First(s => s.DeviceName == deviceScreenId);

            foreach (var selected in ModuleViewports.Where(mv=>mv.IsSelected.Value))
            {
                foreach (var viewport in selected.Viewports)
                {
                    _settingsService.UpsertViewport(selected.Name.Value, selected.Module.Value.ModuleId, screen, viewport);
                }
            }

            return base.Commit();
        }

        public ReactiveCollection<ModuleViewportModel> ModuleViewports
        {
            get;
        } = new ReactiveCollection<ModuleViewportModel>();
    }
}
