using System;
using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced;
using DCS.Alternative.Launcher.Plugins.Settings.Views.General;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Viewports;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly SettingsController _controller;
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<SettingsController>();
            _settingsService = container.Resolve<ISettingsService>();
        }

        protected override async Task InitializeAsync()
        {
            Categories.Add(new CategoryHeaderSettingsViewModel("GENERAL"));
            Categories.Add(new InstallationSettingsViewModel(_controller));

            var optionsCategories = _controller.GetDcsOptionCategories();

            if (optionsCategories.Length > 0)
            {
                Categories.Add(new CategoryHeaderSettingsViewModel("DCS WORLD"));
            }

            foreach (var category in optionsCategories)
            {
                Categories.Add(new DcsOptionCategoryViewModel(category.DisplayName.ToUpper(), category.Id, _controller));
            }

            Categories.Add(new CategoryHeaderSettingsViewModel("VIEWPORTS"));
            Categories.Add(new ViewportSettingsViewModel(_controller));

            var modules = await _controller.GetInstalledAircraftModulesAsync();

            foreach (var module in modules)
            {
                var options = _controller.GetViewportOptions(module.ModuleId);

                if (options.Length > 0)
                {
                    Categories.Add(new ViewportOptionsViewModel(module, options, _controller));
                }
            }

            Categories.Add(new CategoryHeaderSettingsViewModel("ADVANCED OPTIONS"));
            Categories.Add(new GraphicsSettingsViewModel(_controller));
            //Categories.Add(new CameraSettingsViewModel(_controller));
            //Categories.Add(new CameraMirrorsSettingsViewModel(_controller));
            //Categories.Add(new TerrainSettingsViewModel(_controller));

            SelectedCategory.Value = Categories.First(c => !(c is CategoryHeaderSettingsViewModel));
            SelectedCategory.Subscribe(OnSelectedCategoryChanged);

            await base.InitializeAsync();
        }

        private async void OnSelectedCategoryChanged(SettingsCategoryViewModelBase value)
        {
            if (value != null)
            {
                await value.ActivateAsync();
            }
        }

        public ReactiveCollection<SettingsCategoryViewModelBase> Categories
        {
            get;
        } = new ReactiveCollection<SettingsCategoryViewModelBase>();

        public ReactiveProperty<SettingsCategoryViewModelBase> SelectedCategory
        {
            get;
        } = new ReactiveProperty<SettingsCategoryViewModelBase>();

        public override async Task ActivateAsync()
        {
            try
            {
                if (SelectedCategory.Value != null)
                {
                    await SelectedCategory.Value.ActivateAsync();
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }

            await base.ActivateAsync();
        }
    }
}