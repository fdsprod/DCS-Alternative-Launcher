using System;
using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Plugins.Settings.Models;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced
{
    public class DcsOptionCategoryViewModel : SettingsCategoryViewModelBase
    {
        private readonly string _categoryId;

        public DcsOptionCategoryViewModel(string name, string categoryId, SettingsController controller)
            : base(name, controller)
        {
            _categoryId = categoryId;

            SelectedInstall.Subscribe(OnSelectedInstallChanged);
            ResetCommand.Subscribe(OnReset);
            ResetAllCommand.Subscribe(OnResetAll);
        }

        public ReactiveCollection<OptionModelBase> Options
        {
            get;
        } = new ReactiveCollection<OptionModelBase>();

        public ReactiveCollection<InstallLocation> Installations
        {
            get;
        } = new ReactiveCollection<InstallLocation>();

        public ReactiveProperty<InstallLocation> SelectedInstall
        {
            get;
        } = new ReactiveProperty<InstallLocation>();
        public ReactiveCommand<OptionModelBase> ResetCommand
        {
            get;
        } = new ReactiveCommand<OptionModelBase>();

        public ReactiveCommand ResetAllCommand
        {
            get;
        } = new ReactiveCommand();

        public override Task ActivateAsync()
        {
            UpdateInstallations();

            return base.ActivateAsync();
        }

        private void UpdateInstallations()
        {
            Installations.Clear();

            var selectedInstall = SelectedInstall.Value;

            if (selectedInstall == null)
            {
                selectedInstall = Controller.GetCurrentInstall();
            }

            foreach (var install in Controller.GetInstallations())
            {
                Installations.Add(install);
            }

            SelectedInstall.Value = selectedInstall;
        }

        private void OnSelectedInstallChanged(InstallLocation install)
        {
            Options.Clear();

            if (install == null)
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    var categories = Controller.GetDcsCategoryOptionForInstall(install, false);
                    var category = categories.FirstOrDefault(c => c.Id == _categoryId);

                    if (category == null)
                    {
                        return;
                    }

                    var models = OptionModelFactory.CreateAll(category.Options);

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
        }
        
        private void OnValueChanged(OptionModelBase model, object value)
        {
            Controller.UpsertDcsOption(_categoryId, model.Id, value, false);
        }

        private void OnReset(OptionModelBase model)
        {
            var value = Controller.ResetDcsOption(_categoryId, model.Id);
            model.ResetValue(value);
        }

        private void OnResetAll()
        {
            if (MessageBoxEx.Show("Are you sure you want to reset all options to their default values?", "Reset Options", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }

            foreach (var model in Options)
            {
                var value = Controller.ResetDcsOption(_categoryId, model.Id);
                model.ResetValue(value);
            }
        }
    }
}