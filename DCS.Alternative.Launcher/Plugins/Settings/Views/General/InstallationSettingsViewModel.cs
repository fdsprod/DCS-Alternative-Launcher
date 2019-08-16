using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Views.Advanced;
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
        }

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
            try
            {
                Installations.Clear();

                foreach (var install in Controller.GetInstallations())
                {
                    Installations.Add(install);
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

                    Installations.Add(installation);

                    Controller.AddInstalls(installation.Directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
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

                Installations.Remove(installation);
                Controller.RemoveInstalls(installation.Directory);
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
                        Installations.Add(installation);
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
}