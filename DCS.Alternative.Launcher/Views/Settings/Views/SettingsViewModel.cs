using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced;
using DCS.Alternative.Launcher.Plugins.Settings.Views.General;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Settings;
using DCS.Alternative.Launcher.Views.Settings.Views.General;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views
{
    public class SettingsViewModel : NavigationAwareBase
    {
        private readonly ApplicationEventRegistry _eventRegistry;
        private readonly SettingsController _controller;
        private readonly ILauncherSettingsService _settingsService;
        private readonly IProfileService _profileSettingsService;

        public SettingsViewModel(IContainer container)
        {
            _eventRegistry = container.Resolve<ApplicationEventRegistry>();
            _controller = container.Resolve<SettingsController>();
            _settingsService = container.Resolve<ILauncherSettingsService>();
            _profileSettingsService = container.Resolve<IProfileService>();

            var eventRegistry = container.Resolve<ApplicationEventRegistry>();

            eventRegistry.CurrentProfileChanged += OnSelectedProfileChanged;
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
                using (e.GetDeferral())
                {
                    for (var i = Categories.Count - 1; i >= 2; i--)
                    {
                        Categories.RemoveAt(i);
                    }

                    await PopulateProfileSettingsAsync();
                }
            }
            catch (Exception ex)
            {
                GeneralExceptionHandler.Instance.OnError(ex);
            }
        }

        protected override async Task InitializeAsync()
        {
            Categories.Add(new CategoryHeaderSettingsViewModel("APPLICATION SETTINGS"));
            Categories.Add(new ProfileSettingsViewModel(_controller));

            await PopulateProfileSettingsAsync();

            await base.InitializeAsync();
        }

        private async Task PopulateProfileSettingsAsync()
        {
            var profileName = _profileSettingsService.SelectedProfileName;
            var optionsCategories = _controller.GetDcsOptionCategories();

            Categories.Add(new CategoryHeaderSettingsViewModel($"PROFILE SETTINGS ({profileName})"));
            Categories.Add(new InstallationSettingsViewModel(_controller));

            if (optionsCategories.Length > 0)
            {
                Categories.Add(new CategoryHeaderSettingsViewModel($"    DCS"));
            }

            foreach (var category in optionsCategories.OrderBy(c => c.DisplayOrder))
            {
                Categories.Add(new DcsOptionCategoryViewModel("    " + category.DisplayName.ToUpper(), category.Id, _controller));
            }

            var eventArgs = new PopulateSettingsEventArgs(_controller);

            await _eventRegistry.InvokePopulateSettingsAsync(this, eventArgs);

            foreach (var (category, models) in eventArgs.Settings)
            {
                Categories.Add(new CategoryHeaderSettingsViewModel($"    {category}"));

                foreach(var model in models)
                {
                    Categories.Add(model);
                }
            }

            Categories.Add(new CategoryHeaderSettingsViewModel($"    ADVANCED"));
            Categories.Add(new GeneralSettingsViewModel(_controller)); 
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
            await base.ActivateAsync();

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

        }
    }

    public class PopulateSettingsEventArgs : DeferredEventArgs
    {
        internal readonly Dictionary<string, List<SettingsCategoryViewModelBase>> Settings = new Dictionary<string, List<SettingsCategoryViewModelBase>>();

        public PopulateSettingsEventArgs(SettingsController controller)
        {
            Controller = controller;
        }

        public SettingsController Controller
        {
            get;
        }

        public void AddCategory(string parentCategory, SettingsCategoryViewModelBase categoryModel)
        {
            if (!Settings.TryGetValue(parentCategory, out var categories))
            {
                categories = new List<SettingsCategoryViewModelBase>();
                Settings[parentCategory] = categories;
            }

            categories.Add(categoryModel);
        }
    }

}