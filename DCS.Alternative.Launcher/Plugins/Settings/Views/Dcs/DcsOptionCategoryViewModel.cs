using System;
using System.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
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
        }

        public ReactiveCollection<OptionModel> Options
        {
            get;
        } = new ReactiveCollection<OptionModel>();

        public ReactiveCollection<InstallLocation> Installations
        {
            get;
        } = new ReactiveCollection<InstallLocation>();

        public ReactiveProperty<InstallLocation> SelectedInstall
        {
            get;
        } = new ReactiveProperty<InstallLocation>();

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
        
        private void OnValueChanged(OptionModel model, object value)
        {
            Controller.UpsertDcsOption(_categoryId, model.Id, value, false);
        }
    }
}