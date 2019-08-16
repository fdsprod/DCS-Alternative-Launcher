using System;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Viewports
{
    public class ViewportOptionsViewModel : SettingsCategoryViewModelBase
    {
        private readonly Module _module;

        public ViewportOptionsViewModel(Module module, ViewportOption[] options, SettingsController controller)
            : base($"{module.DisplayName} VIEWPORT OPTIONS", controller)
        {
            _module = module;

            var models = OptionModelFactory.CreateAll(options);

            foreach (var model in models)
            {
                model.ValueChangeObservable.Subscribe(value => OnValueChanged(model, value));
                Options.Add(model);
            }
        }
        
        private void OnValueChanged(OptionModel model, object value)
        {
            Controller.UpsertViewportOption(_module.ModuleId, model.Id, value);
        }

        public ReactiveCollection<OptionModel> Options
        {
            get;
        } = new ReactiveCollection<OptionModel>();
    }
}