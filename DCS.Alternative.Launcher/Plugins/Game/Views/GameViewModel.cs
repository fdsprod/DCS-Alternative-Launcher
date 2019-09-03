using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly ISettingsService _settingsService;

        public GameViewModel(IContainer container)
        {
            _container = container;
            _controller = container.Resolve<GameController>();

            PlayButtonText.Value = "PLAY";

            _settingsService = _container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();

            IsUpdateAvailable =
                IsDcsOutOfDate.AsObservable().Merge(
                        IsCheckingLatestVersion.AsObservable().Merge(
                            IsLoading.AsObservable().Merge(
                                FailedVersionCheck.AsObservable()))).Select(_ => IsDcsOutOfDate.Value && !IsLoading.Value && !IsCheckingLatestVersion.Value && !FailedVersionCheck.Value)
                    .ToReactiveProperty();

            SelectInstallCommand.Subscribe(OnSelectInstall);
            UpdateDcsCommand.Subscribe(OnUpdateDcs);
            RepairDcsCommand.Subscribe(OnRepairDcs);
            LaunchDcsCommand.Subscribe(OnLaunchDcs);
            CheckForUpdatesCommand.Subscribe(OnCheckForUpdates);
            ShowNewsArticleCommand.Subscribe(OnShowNewsArticle);

            IsVREnabled.Subscribe(OnIsVREnabledChanged);
        }

        public ReactiveProperty<bool> IsVREnabled
        {
            get;
        } = new ReactiveProperty<bool>();

        public ReactiveCollection<InstallLocation> Installations
        {
            get;
        } = new ReactiveCollection<InstallLocation>();

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
        } = new ReactiveCommand();

        public ReactiveCommand CheckForUpdatesCommand
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
            Process.Start(model.Url.Value);
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
            var install = _settingsService.SelectedInstall;

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
                    PlayButtonText.Value = IsDcsOutOfDate.Value ? "UPDATE / PLAY" : "PLAY";
                    DcsVersion.Value = IsDcsOutOfDate.Value ? "DCS WORLD IS OUT OF DATE" : "DCS WORLD IS UP TO DATE";
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

        public override Task ActivateAsync()
        {
            Installations.Clear();

            foreach (var install in _settingsService.GetInstallations())
            {
                Installations.Add(install);
            }

            SelectedInstall.Value = _settingsService.SelectedInstall;

            return base.ActivateAsync();
        }

        protected override async Task InitializeAsync()
        {
            IsVREnabled.Value = _settingsService.GetValue(SettingsCategories.LaunchOptions, SettingsKeys.IsVREnabled, false);

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
                await _controller.LaunchDcsAsync(IsVREnabled.Value);
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

        private void OnIsVREnabledChanged(bool value)
        {
            _settingsService.SetValue(SettingsCategories.LaunchOptions, SettingsKeys.IsVREnabled, value);
        }
    }
}