using System;
using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced;
using DCS.Alternative.Launcher.Plugins.Settings.Views.General;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Viewports;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Views.Settings.Views.General;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly SettingsController _controller;
        private readonly ISettingsService _settingsService;
        private readonly IProfileSettingsService _profileSettingsService;

        public SettingsViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<SettingsController>();
            _settingsService = container.Resolve<ISettingsService>();
            _profileSettingsService = container.Resolve<IProfileSettingsService>();

            _profileSettingsService.SelectedProfileChanged += OnSelectedProfileChanged;
        }

        public ReactiveCollection<SettingsCategoryViewModelBase> Categories
        {
            get;
        } = new ReactiveCollection<SettingsCategoryViewModelBase>();

        public ReactiveProperty<SettingsCategoryViewModelBase> SelectedCategory
        {
            get;
        } = new ReactiveProperty<SettingsCategoryViewModelBase>();

        private async void OnSelectedProfileChanged(object sender, Services.Settings.SelectedProfileChangedEventArgs e)
        {
            try
            {
                for (var i = Categories.Count - 1; i >= 3; i--)
                {
                    Categories.RemoveAt(i);
                }

                await PopulateProfileSettingsAsync();
            }
            catch (Exception ex)
            {
                GeneralExceptionHandler.Instance.OnError(ex);
            }
        }

        protected override async Task InitializeAsync()
        {
            Categories.Add(new CategoryHeaderSettingsViewModel("APPLICATION SETTINGS"));
            Categories.Add(new InstallationSettingsViewModel(_controller));
            Categories.Add(new ProfileSettingsViewModel(_controller));

            await PopulateProfileSettingsAsync();

            await base.InitializeAsync();
        }

        private async Task PopulateProfileSettingsAsync()
        {
            var profileName = _profileSettingsService.SelectedProfileName;
            var optionsCategories = _controller.GetDcsOptionCategories();

            Categories.Add(new CategoryHeaderSettingsViewModel($"PROFILE SETTINGS ({profileName})"));

            if (optionsCategories.Length > 0)
            {
                Categories.Add(new CategoryHeaderSettingsViewModel($"    DCS"));
            }

            foreach (var category in optionsCategories.OrderBy(c => c.DisplayOrder))
            {
                Categories.Add(new DcsOptionCategoryViewModel("    " + category.DisplayName.ToUpper(), category.Id, _controller));
            }

            Categories.Add(new CategoryHeaderSettingsViewModel($"    VIEWPORTS"));
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

            Categories.Add(new CategoryHeaderSettingsViewModel($"    ADVANCED"));
            Categories.Add(new GraphicsSettingsViewModel(_controller));
            //Categories.Add(new CameraSettingsViewModel(_controller));
            //Categories.Add(new CameraMirrorsSettingsViewModel(_controller));
            //Categories.Add(new TerrainSettingsViewModel(_controller));
            //Categories.Add(new TerrainMirrorSettingsViewModel(_controller));
            //Categories.Add(new TerrainReflectionSettingsViewModel(_controller));
            Categories.Add(new SoundSettingsViewModel(_controller));

            SelectedCategory.Value = Categories.First(c => !(c is CategoryHeaderSettingsViewModel));
            SelectedCategory.Subscribe(OnSelectedCategoryChanged);
        }

        private async void OnSelectedCategoryChanged(SettingsCategoryViewModelBase value)
        {
            if (value != null)
            {
                await value.ActivateAsync();
            }
        }

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