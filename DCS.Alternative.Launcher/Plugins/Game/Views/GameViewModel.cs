using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
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
        private readonly IDcsWorldService _dcsWorldService;
        private readonly ISettingsService _settingsService;

        public GameViewModel(IContainer container)
        {
            _container = container;
            _settingsService = _container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();

            IsUpdateAvailable =
                IsDcsOutOfDate.AsObservable().Merge(
                        IsCheckingLatestVersion.AsObservable().Merge(
                            IsLoading.AsObservable().Merge(
                                FailedVersionCheck.AsObservable()))).Select(_ =>
                    {
                        return IsDcsOutOfDate.Value && !IsLoading.Value && !IsCheckingLatestVersion.Value &&
                               !FailedVersionCheck.Value;
                    })
                    .ToReactiveProperty();

            RepairDcsCommand.Subscribe(OnRepairDcs);
            LaunchDcsCommand.Subscribe(OnLaunchDcs);

            ShowNewsArticleCommand.Subscribe(OnShowNewsArticle);
        }

        public ReactiveProperty<bool> IsLoading { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsUpdateAvailable { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsCheckingLatestVersion { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsDcsOutOfDate { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsDcsUpToDate { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> FailedVersionCheck { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<string> DcsVersion { get; } = new ReactiveProperty<string>();

        public ReactiveCommand RepairDcsCommand { get; } = new ReactiveCommand();

        public ReactiveCommand LaunchDcsCommand { get; } = new ReactiveCommand();

        public ReactiveProperty<string> LatestEagleDynamicsYouTubeUrl { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<NewsArticleModel> LatestNewsArticle { get; } = new ReactiveProperty<NewsArticleModel>();

        public ReactiveProperty<NewsArticleModel> PreviousNewsArticle { get; } =
            new ReactiveProperty<NewsArticleModel>();

        public ReactiveCommand<NewsArticleModel> ShowNewsArticleCommand { get; } =
            new ReactiveCommand<NewsArticleModel>();

        private void OnShowNewsArticle(NewsArticleModel model)
        {
            Process.Start(model.Url.Value);
        }

        protected override async Task InitializeAsync()
        {
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                try
                {
                    IsLoading.Value = true;
                    LatestEagleDynamicsYouTubeUrl.Value = await _dcsWorldService.GetLatestYoutubeVideoUrlAsync();

                    var articles = await _dcsWorldService.GetLatestNewsArticlesAsync(2);
                    LatestNewsArticle.Value = articles.FirstOrDefault();
                    PreviousNewsArticle.Value = articles.Skip(1).FirstOrDefault();
                }
                catch (Exception e)
                {
                    Tracer.Error(e);
                }
                finally
                {
                    IsLoading.Value = false;
                }

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

                        IsDcsUpToDate.Value = !(IsDcsOutOfDate.Value = install.Version < latestVersions[variant]);

                        if (IsDcsOutOfDate.Value)
                        {
                            DcsVersion.Value = "DCS WORLD IS OUT OF DATE";
                        }
                        else
                        {
                            DcsVersion.Value = $"DCS WORLD IS UP TO DATE ({install.Version})";
                        }
                    }
                    catch (Exception e)
                    {
                        FailedVersionCheck.Value = true;
                        Tracer.Error(e);
                    }
                    finally
                    {
                        IsCheckingLatestVersion.Value = false;
                    }
                }
            });

            await base.InitializeAsync();
        }

        public override Task OnNavigatingAsync(NavigatingEventArgs args)
        {
            args.Cancel = IsLoading.Value;

            return base.OnNavigatingAsync(args);
        }

        private void OnLaunchDcs()
        {
            var moduleViewports = _settingsService.GetModuleViewports();

            foreach (var mv in moduleViewports)
            {
                mv.PatchViewports(_settingsService.SelectedInstall);
            }

            var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.ExePath);
            var process = Process.Start(processInfo);

            App.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        private void UpdateDcs()
        {
            var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.UpdaterPath, "update");
            var process = Process.Start(processInfo);

            process?.WaitForExit();
        }

        private void OnRepairDcs()
        {
            var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.UpdaterPath, "repair");
            var process = Process.Start(processInfo);

            process?.WaitForExit();
        }
    }
}