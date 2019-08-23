using System;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public abstract class AdvancedOptionSettingsViewModelBase : SettingsCategoryViewModelBase
    {
        private readonly string _advancedOptionsCategory;

        protected AdvancedOptionSettingsViewModelBase(string name, string optionsCategory, SettingsController controller)
            : base(name, controller)
        {
            _advancedOptionsCategory = optionsCategory;

            ResetCommand.Subscribe(OnReset);
            ResetAllCommand.Subscribe(OnResetAll);
        }

        public ReactiveCommand<OptionModelBase> ResetCommand
        {
            get;
        } = new ReactiveCommand<OptionModelBase>();

        public ReactiveCommand ResetAllCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCollection<OptionModelBase> Options
        {
            get;
        } = new ReactiveCollection<OptionModelBase>();

        protected override Task InitializeAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    var options = Controller.GetAdvancedOptions(_advancedOptionsCategory);
                    var models = OptionModelFactory.CreateAll(options);

                    foreach (var model in models)
                    {
                        model.ValueChangeObservable.Subscribe(value => OnValueChanged(model, value));
                        Options.AddOnScheduler(model);
                    }
                }
                catch (Exception e)
                {
                    GeneralExceptionHandler.Instance.OnError(e);
                }
            });

            return base.InitializeAsync();
        }

        private void OnValueChanged(OptionModelBase model, object value)
        {
            Controller.UpsertAdvancedOption(model.Id, value);
        }

        private void OnReset(OptionModelBase model)
        {
            var value = Controller.ResetAdvancedOptionValue(_advancedOptionsCategory, model.Id);
            model.ResetValue(value);
        }

        private void OnResetAll()
        {
            foreach (var model in Options)
            {
                var value = Controller.ResetAdvancedOptionValue(_advancedOptionsCategory, model.Id);
                model.ResetValue(value);
            }
        }
    }
}