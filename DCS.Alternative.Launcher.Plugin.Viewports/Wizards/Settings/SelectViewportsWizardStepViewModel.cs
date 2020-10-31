using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects;
using DCS.Alternative.Launcher.Plugin.Viewports.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Wizards.Settings
{
    public class SelectViewportsWizardStepViewModel : WizardStepBase<SelectViewportWizardController>
    {
        private readonly IDcsWorldManager _dcsWorldManager;
        private readonly ModuleViewportTemplate[] _templates;

        public SelectViewportsWizardStepViewModel(IContainer container, ModuleViewportTemplate[] templates)
            : base(container)
        {
            _templates = templates;
            _dcsWorldManager = container.Resolve<IDcsWorldManager>();
        }

        public ReactiveCollection<ModuleViewportModel> ModuleViewports
        {
            get;
        } = new ReactiveCollection<ModuleViewportModel>();

        public override async Task ActivateAsync()
        {
            if (_templates.Length == 0)
            {
                await Controller.GoNextAsync(false);
                return;
            }

            var installedModules = await _dcsWorldManager.GetInstalledAircraftModulesAsync();

            foreach (var template in _templates)
            {
                var module = installedModules.First(m => m.ModuleId == template.ModuleId);
                var model = new ModuleViewportModel(template.TemplateName, template.ExampleImageUrl, module, template.Viewports, template);

                ModuleViewports.Add(model);
            }

            await base.ActivateAsync();
        }

        public override bool Commit()
        {
            var moduleViewport = ModuleViewports.FirstOrDefault(m => m.IsSelected.Value);

            Controller.SelectedTemplate = moduleViewport?.Template;
           
            return base.Commit();
        }
    }
}