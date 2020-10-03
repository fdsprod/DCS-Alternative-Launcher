using System;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Views;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Views.Settings
{
    public class ViewportOptionsViewModel : SettingsCategoryViewModelBase
    {
        private readonly ModuleBase _module;
        private readonly ViewportController _viewportController;

        public ViewportOptionsViewModel(ModuleBase module, ViewportOption[] options, ViewportController viewportController, SettingsController settingsController)
            : base($"    {module.DisplayName} VIEWPORT OPTIONS", settingsController)
        {
            _module = module;
            _viewportController = viewportController;

            var models = OptionModelFactory.CreateAll(options);

            foreach (var model in models)
            {
                model.ValueChangeObservable.Subscribe(value => OnValueChanged(model, value));
                Options.Add(model);
            }
        }

        public ReactiveCollection<OptionModelBase> Options
        {
            get;
        } = new ReactiveCollection<OptionModelBase>();

        private void OnValueChanged(OptionModelBase model, object value)
        {
            _viewportController.UpsertViewportOption(_module.ModuleId, model.Id, value);
        }
    }
}