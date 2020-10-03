using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.Diagnostics;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using DCS.Alternative.Launcher.Models;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Navigation;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public class GameViewModel : NavigationAwareBase
    {
        private readonly IContainer _container;
        private readonly GameController _controller;
        private readonly IDcsWorldService _dcsWorldService;
        private readonly IProfileService _profileService;

        private DispatcherTimer _checkPlayingTimer;

        public GameViewModel(IContainer container)
        {
            DcsProcessMonitor.Instance.DcsProcessExited += OnDcsProcessExited;

            _container = container;
            _controller = container.Resolve<GameController>();

            _profileService = _container.Resolve<IProfileService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();

            IsUpdateAvailable =
                IsDcsOutOfDate.AsObservable().Merge(
                        IsCheckingLatestVersion.AsObservable().Merge(
                            IsLoading.AsObservable().Merge(
                                FailedVersionCheck.AsObservable()))).Select(_ => IsDcsOutOfDate.Value && !IsLoading.Value && !IsCheckingLatestVersion.Value && !FailedVersionCheck.Value)
                    .ToReactiveProperty();

            var canPlayObservable =
                SelectedInstall
                    .Select(_ => Unit.Default)
                    .Merge(IsPlayingDcs.Select(_ => Unit.Default))
                    .Merge(IsUpdateAvailable.Select(_ => Unit.Default)).Select(_ => CanLaunchDcs());

            LaunchDcsCommand = canPlayObservable.ToReactiveCommand();

            SelectInstallCommand.Subscribe(OnSelectInstall);
            UpdateDcsCommand.Subscribe(OnUpdateDcs);
            RepairDcsCommand.Subscribe(OnRepairDcs);
            LaunchDcsCommand.Subscribe(OnLaunchDcs);
            CheckForUpdatesCommand.Subscribe(OnCheckForUpdates);
            ShowNewsArticleCommand.Subscribe(OnShowNewsArticle);
            CleanShadersCommand.Subscribe(OnCleanShaders);

            _checkPlayingTimer = new DispatcherTimer();
            _checkPlayingTimer.Interval = TimeSpan.FromSeconds(1);
            _checkPlayingTimer.Tick += OnCheckPlayingTimerTick;
            _checkPlayingTimer.Start();

            IsUpdatingDcs.Subscribe(OnIsUpdatingDcsChanged);
        }

        private async void OnIsUpdatingDcsChanged(bool value)
        {
            if (value)
            {
                return;
            }

            try
            {
                await CheckForUpdatesAsync();
            }
            catch(Exception e)
            {
                Tracer.Error(e);
            }
        }

        private void OnCleanShaders()
        {
            try
            {
                var install = SelectedInstall.Value;

                if(install == null)
                {
                    MessageBoxEx.Show("Could not find a valid DCS World installation.");
                    return;
                }

                var path = Path.Combine(install.SavedGamesPath, "fxo");

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                path = Path.Combine(install.SavedGamesPath, "metashaders");

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                
                path = Path.Combine(install.SavedGamesPath, "metashaders2");

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }

            MessageBoxEx.Show("Shader cache has been cleaned.");
        }

        private void OnDcsProcessExited(object sender, EventArgs e)
        {
            Application.Current.MainWindow?.Show();
            Application.Current.MainWindow?.BringIntoView();
        }

        private void OnCheckPlayingTimerTick(object sender, EventArgs e)
        {
            CheckDcsStatus();
        }

        private void CheckDcsStatus()
        {
            IsPlayingDcs.Value = SelectedInstall.Value != null && DcsProcessMonitor.Instance.IsDcsInstallRunning(SelectedInstall.Value);
            IsUpdatingDcs.Value = SelectedInstall.Value != null && DcsProcessMonitor.Instance.IsDcsInstallUpdating(SelectedInstall.Value);

            UpdatePlayButtonText();
        }

        private bool CanLaunchDcs()
        {
            return SelectedInstall.Value != null && !IsUpdatingDcs.Value && !IsPlayingDcs.Value;
        }

        public ReactiveProperty<bool> IsPlayingDcs
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsUpdatingDcs
        {
            get;
        } = new ReactiveProperty<bool>();
        
        public ReactiveProperty<InstallLocation> SelectedInstall
        {
            get;
        } = new ReactiveProperty<InstallLocation>();

        public ReactiveCommand<InstallLocation> SelectInstallCommand
        {
            get;
        } = new ReactiveCommand<InstallLocation>();

        public ReactiveProperty<string> PlayButtonText
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<bool> IsLoading
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsUpdateAvailable
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsCheckingLatestVersion
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsDcsOutOfDate
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsDcsUpToDate
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> FailedVersionCheck
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> DcsVersion
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveCommand RepairDcsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand UpdateDcsCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand LaunchDcsCommand
        {
            get;
        }

        public ReactiveCommand CheckForUpdatesCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveCommand CleanShadersCommand
        {
            get;
        } = new ReactiveCommand();

        public ReactiveProperty<string> LatestYouTubeUrl
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<NewsArticleModel> LatestNewsArticle
        {
            get;
        } = new ReactiveProperty<NewsArticleModel>();

        public ReactiveProperty<NewsArticleModel> PreviousNewsArticle
        {
            get;
        } = new ReactiveProperty<NewsArticleModel>();

        public ReactiveCommand<NewsArticleModel> ShowNewsArticleCommand
        {
            get;
        } = new ReactiveCommand<NewsArticleModel>();

        private void OnSelectInstall(InstallLocation install)
        {
            SelectedInstall.Value = install;
        }

        private void OnShowNewsArticle(NewsArticleModel model)
        {
            var ps = new ProcessStartInfo(model.Url.Value)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }

        private async void OnCheckForUpdates()
        {
            try
            {
                await CheckForUpdatesAsync();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            var install = _profileService.GetSelectedInstall();

            if (install?.IsValidInstall ?? false)
            {
                IsCheckingLatestVersion.Value = true;
                IsDcsOutOfDate.Value = false;
                IsDcsUpToDate.Value = false;
                FailedVersionCheck.Value = false;

                try
                {
                    var latestVersions = await _dcsWorldService.GetLatestVersionsAsync();
                    var variant = install.Variant;

                    install.RefreshInfo();

                    IsDcsUpToDate.Value = !(IsDcsOutOfDate.Value = install.Version < latestVersions[variant]);
                    DcsVersion.Value = IsDcsOutOfDate.Value ? "DCS WORLD IS OUT OF DATE" : "DCS WORLD IS UP TO DATE";

                    UpdatePlayButtonText();
                }
                catch (Exception e)
                {
                    FailedVersionCheck.Value = true;
                    GeneralExceptionHandler.Instance.OnError(e);
                }
                finally
                {
                    IsCheckingLatestVersion.Value = false;
                }
            }
        }

        private void UpdatePlayButtonText()
        {
            if (SelectedInstall.Value == null)
            {
                PlayButtonText.Value = "NOT INSTALLED";
            }
            else if (IsUpdatingDcs.Value)
            {
                PlayButtonText.Value = "UPDATING...";
            }
            else if (IsPlayingDcs.Value)
            {
                PlayButtonText.Value = "RUNNING...";
            }
            else
            {
                PlayButtonText.Value = IsDcsOutOfDate.Value ? "UPDATE / PLAY" : "PLAY";
            }
        }

        public override Task ActivateAsync()
        {
            var install = _profileService.GetSelectedInstall();

            SelectedInstall.Value = install;

            CheckDcsStatus();

            return base.ActivateAsync();
        }

        protected override async Task InitializeAsync()
        {
            SafeAsync.Run(() => Task.Run(async () =>
            {
                IsLoading.Value = true;

                await Task.WhenAll(
                    SafeAsync.RunAsync(FetchNewsAsync),
                    SafeAsync.RunAsync(FetchLatestYouTubeAsync));

                IsLoading.Value = false;

                await CheckForUpdatesAsync();
            }), Tracer.Error);

            await base.InitializeAsync();
        }

        private async Task FetchLatestYouTubeAsync()
        {
            LatestYouTubeUrl.Value = await _dcsWorldService.GetLatestYoutubeVideoUrlAsync();
        }

        private async Task FetchNewsAsync()
        {
            var articles = await _dcsWorldService.GetLatestNewsArticlesAsync(2);

            LatestNewsArticle.Value = articles.FirstOrDefault();
            PreviousNewsArticle.Value = articles.Skip(1).FirstOrDefault();
        }

        public override Task OnNavigatingAsync(NavigatingEventArgs args)
        {
            args.Cancel = IsLoading.Value;

            return base.OnNavigatingAsync(args);
        }

        private async void OnLaunchDcs()
        {
            var window = Application.Current.MainWindow;

            try
            {
                if (IsDcsOutOfDate.Value && MessageBoxEx.Show($"DCS World is not currently up to date.{Environment.NewLine}Would you like to update now?", "Update", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    window.WindowState = WindowState.Minimized;
                    await _controller.UpdateAsync();
                }

                window.WindowState = WindowState.Minimized;
                await _controller.LaunchDcsAsync();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
        }

        private async void OnUpdateDcs()
        {
            var window = Application.Current.MainWindow;

            try
            {
                window.WindowState = WindowState.Minimized;
                await _controller.UpdateAsync();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
            finally
            {
                window.WindowState = WindowState.Normal;
            }
        }

        private async void OnRepairDcs()
        {
            var window = Application.Current.MainWindow;

            try
            {
                window.WindowState = WindowState.Minimized;
                await _controller.RepairAsync();
            }
            catch (Exception e)
            {
                GeneralExceptionHandler.Instance.OnError(e);
            }
            finally
            {
                window.WindowState = WindowState.Normal;
            }
        }
    }
}