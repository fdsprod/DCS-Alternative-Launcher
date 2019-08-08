using System;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public class AdvancedOptionSettingsViewModelBase : SettingsCategoryViewModelBase
    {
        private readonly string _advancedOptionsCategory;

        public AdvancedOptionSettingsViewModelBase(string name, string optionsCategory, SettingsController controller)
            : base(name, controller)
        {
            _advancedOptionsCategory = optionsCategory;
        }

        protected override Task InitializeAsync()
        {
            var options = Controller.GetAdvancedOptions(_advancedOptionsCategory);
            var models = AdvancedOptionModelFactory.CreateAll(options);

            foreach (var model in models)
            {
                model.ValueChangeObservable.Subscribe(value => OnValueChanged(model, value));
                Options.Add(model);
            }

            return base.InitializeAsync();
        }

        private void OnValueChanged(AdvancedOptionModel model, object value)
        {
            Controller.UpsertAdvancedOption(model.Id, value);
        }

        public ReactiveCollection<AdvancedOptionModel> Options
        {
            get;
        } = new ReactiveCollection<AdvancedOptionModel>();
    }
}