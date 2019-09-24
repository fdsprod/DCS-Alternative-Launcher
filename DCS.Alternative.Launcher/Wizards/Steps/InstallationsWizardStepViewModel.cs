using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DCS.Alternative.Launcher.Controls;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Views.General;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Wizards.Steps
{
    public class InstallationsWizardStepViewModel : WizardStepBase
    {
        private readonly ISettingsService _settingsService;

        public InstallationsWizardStepViewModel(IContainer container)
            : base(container)
        {
            _settingsService = container.Resolve<ISettingsService>();

            AddInstallationCommand.Subscribe(OnAddInstallation);
            DetectInstallationsCommand.Subscribe(OnDetectInstallations);

            RemoveInstallationCommand = new ReactiveCommand(SelectedInstall.Select(i => i != null), false);
            RemoveInstallationCommand.Subscribe(OnRemoveInstallation);
        }

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

        public ReactiveCommand RemoveInstallationCommand
        {
            get;
        }

        public ReactiveCommand AddInstallationCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveProperty<bool> IsLoading
        {
            get;
        } = new ReactiveProperty<bool>();

        public override Task ActivateAsync()
        {
            try
            {
                Installations.Clear();

                foreach (var install in _settingsService.GetInstallations())
                {
                    Installations.Add(new InstallLocationModel(install));
                }

                if (Installations.Any())
                {
                    Installations[0].IsDefault.Value = true;
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }
            finally
            {
                IsLoading.Value = false;
            }

            return base.ActivateAsync();
        }

        public override bool Validate()
        {
            if (Installations.Any() && Installations.Count(i=>i.IsDefault.Value) == 0)
            {
                var window = WindowAssist.GetWindow(Controller);
                MessageBoxEx.Show("You must set a default installation to continue.", "Default Installations", MessageBoxButton.OK, parent: window);
                return false;
            }

            if (Installations.Count(i => i.ConcreteInstall.IsValidInstall) == 0)
            {
                var window = WindowAssist.GetWindow(Controller);
                return MessageBoxEx.Show("No valid DCS World installations were found.   Are you sure you want to continue?", "Installations", MessageBoxButton.YesNo, parent: window) == MessageBoxResult.Yes;
            }

            return base.Validate();
        }

        public override bool Commit()
        {
            var defaultInstallation = Installations.First(i => i.IsDefault.Value);

            _settingsService.AddInstalls(Installations.Select(i => i.ConcreteInstall.Directory).ToArray());
            _settingsService.SelectedInstall = defaultInstallation.ConcreteInstall;

            return base.Commit();
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

                    _settingsService.AddInstalls(installation.Directory);
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
                _settingsService.RemoveInstalls(installation.ConcreteInstall.Directory);
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
                    _settingsService.AddInstalls(directory);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }
    }
}