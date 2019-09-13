using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;
using WpfScreenHelper;

namespace DCS.Alternative.Launcher.Wizards.Steps
{
    public class SelectInitialViewportsWizardStepViewModel : WizardStepBase
    {
        private readonly IDcsWorldService _dcsWorldService;
        private readonly ISettingsService _settingsService;
        private readonly IProfileSettingsService _profileSettingsService;

        public SelectInitialViewportsWizardStepViewModel(IContainer container)
            : base(container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
            _profileSettingsService = container.Resolve<IProfileSettingsService>();
        }

        public ReactiveCollection<ModuleViewportModel> ModuleViewports
        {
            get;
        } = new ReactiveCollection<ModuleViewportModel>();

        public override async Task ActivateAsync()
        {
            var defaultViewportTemplates = _profileSettingsService.GetDefaultViewportTemplates();
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
            var deviceScreenId = _profileSettingsService.GetValue<string[]>(ProfileSettingsCategories.Viewports, SettingsKeys.DeviceViewportsDisplays).First();
            var screen = Screen.AllScreens.First(s => s.DeviceName == deviceScreenId);

            foreach (var selected in ModuleViewports.Where(mv => mv.IsSelected.Value))
            {
                foreach (var viewport in selected.Viewports)
                {
                    _profileSettingsService.UpsertViewport(selected.Name.Value, selected.Module.Value.ModuleId, screen, viewport);
                }
            }

            return base.Commit();
        }
    }
}