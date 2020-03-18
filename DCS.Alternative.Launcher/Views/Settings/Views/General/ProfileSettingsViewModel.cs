using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.Plugins.Settings.Views;
using DCS.Alternative.Launcher.Plugins.Settings.Views.General;
using DCS.Alternative.Launcher.Storage.Profiles;
using DCS.Alternative.Launcher.Windows.FirstUse;
using DCS.Alternative.Launcher.Wizards;
using DCS.Alternative.Launcher.Wizards.Steps.FirstUse;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Views.Settings.Views.General
{
    public class ProfileSettingsViewModel : SettingsCategoryViewModelBase
    {
        public ProfileSettingsViewModel(SettingsController controller)
            : base("PROFILES", controller)
        {
            RemoveProfileCommand = new ReactiveCommand(SelectedProfile.Select(i => i != null), false);
            RemoveProfileCommand.Subscribe(OnRemoveProfile);
            AddProfileCommand.Subscribe(OnAddProfile);
        }

        public ReactiveCommand EditProfileCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand RemoveProfileCommand
        {
            get;
        }

        public ReactiveCommand AddProfileCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCollection<SettingsProfileModel> Profiles
        {
            get;
        } = new ReactiveCollection<SettingsProfileModel>();

        public ReactiveProperty<SettingsProfileModel> SelectedProfile
        {
            get;
        } = new ReactiveProperty<SettingsProfileModel>();

        public override Task ActivateAsync()
        {
            UpdateProfiles();

            return base.ActivateAsync();
        }

        private void UpdateProfiles()
        {
            try
            {
                Profiles.Clear();

                var profiles = SettingsProfileStorageAdapter.GetAll();

                foreach (var profile in profiles)
                {
                    var model = new SettingsProfileModel(profile.Name);
                    Profiles.Add(model);
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
        }
        
        private void OnAddProfile()
        {
            try
            {
                using (var container = Controller.GetChildContainer())
                {
                    container.Register<WizardController>().AsSingleton();

                    var wizard = new Wizard();

                    var steps = new WizardStepBase[]
                    {
                        new CreateProfileWizardStepViewModel(container),
                    };

                    var viewModel = new WizardViewModel(container, steps);

                    wizard.DataContext = viewModel;
                    wizard.ShowDialog();
                }

                UpdateProfiles();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private void OnRemoveProfile()
        {
            try
            {
                var profiles = SettingsProfileStorageAdapter.GetAll();

                if (profiles.Count() == 1)
                {
                    MessageBoxEx.Show("Cannot delete the only profile that exists.  Create a new profile first, then delete this one if you wish to discard it.", "CANNOT DELETE");
                    return;
                }

                var profile = SelectedProfile.Value;

                if (profile == null)
                {
                    return;
                }

                if (MessageBoxEx.Show($"Are you sure you want to remove the {profile.Name.Value} profile?", "Remove Profile", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                {
                    Profiles.Remove(profile);
                    Controller.RemoveProfile(profile.Name.Value);

                    UpdateProfiles();
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }
    }
}