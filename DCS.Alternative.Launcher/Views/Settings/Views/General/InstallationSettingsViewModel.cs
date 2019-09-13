using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.General
{
    public class InstallationSettingsViewModel : SettingsCategoryViewModelBase
    {
        public InstallationSettingsViewModel(SettingsController controller)
            : base("INSTALLATION", controller)
        {
            RemoveInstallationCommand = new ReactiveCommand(SelectedInstall.Select(i => i != null), false);

            DetectInstallationsCommand.Subscribe(OnDetectInstallations);
            RemoveInstallationCommand.Subscribe(OnRemoveInstallation);
            AddInstallationCommand.Subscribe(OnAddInstallation);
            VerifyInstallationsCommand.Subscribe(OnVerifyInstallations);
        }

        public ReactiveCommand VerifyInstallationsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand RemoveInstallationCommand
        {
            get;
        }

        public ReactiveCommand AddInstallationCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand DetectInstallationsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCollection<InstallLocationModel> Installations
        {
            get;
        } = new ReactiveCollection<InstallLocationModel>();

        public ReactiveProperty<InstallLocationModel> SelectedInstall
        {
            get;
        } = new ReactiveProperty<InstallLocationModel>();

        public override Task ActivateAsync()
        {
            try
            {
                Installations.Clear();

                foreach (var install in Controller.GetInstallations())
                {
                    Installations.Add(new InstallLocationModel(install));
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }

            return base.ActivateAsync();
        }

        private void OnAddInstallation()
        {
            try
            {
                var folderBrowser = new FolderBrowserDialog
                {
                    Description = "Please select the DCS folder you wish to add...",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = false
                };

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    var selectedFolder = folderBrowser.SelectedPath;
                    var installation = new InstallLocation(selectedFolder);

                    Installations.Add(new InstallLocationModel(installation));

                    Controller.AddInstalls(installation.Directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private void OnVerifyInstallations()
        {
            foreach (var install in Installations)
            {
                install.Verify();
            }

            MessageBoxEx.Show("Verification complete.");
        }

        private void OnRemoveInstallation()
        {
            try
            {
                var installation = SelectedInstall.Value;

                if (installation == null)
                {
                    return;
                }

                if (MessageBoxEx.Show($"Are you sure you want to remove the {installation.ConcreteInstall.Name} install", "Remove Install", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                {
                    Installations.Remove(installation);
                    Controller.RemoveInstalls(installation.ConcreteInstall.Directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private void OnDetectInstallations()
        {
            try
            {
                var installations = InstallationLocator.Locate();
                var addedInstallations = new List<string>();

                foreach (var installation in installations)
                {
                    if (Installations.All(i => i.ToString() != installation.ToString()))
                    {
                        Installations.Add(new InstallLocationModel(installation));
                        addedInstallations.Add(installation.Directory);
                    }
                }

                foreach (var directory in addedInstallations)
                {
                    Controller.AddInstalls(directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }
    }

    public class InstallLocationModel
    {
        public InstallLocationModel(InstallLocation install)
        {
            ConcreteInstall = install;

            Verify();
        }

        public InstallLocation ConcreteInstall
        {
            get;
        }

        public ReactiveProperty<bool> IsValidInstall
        {
            get;
        } = new ReactiveProperty<bool>();

        public void Verify()
        {
            IsValidInstall.Value = ConcreteInstall.IsValidInstall;
        }
    }
}