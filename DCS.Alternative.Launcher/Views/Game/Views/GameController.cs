using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;
using DCS.Alternative.Launcher.Services.Settings;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public class GameController
    {
        public readonly IDcsWorldManager _dcsWorldManager;
        public readonly IProfileService _profileService;
        public readonly ILauncherSettingsService _settingsService;
        public readonly ApplicationEventRegistry _eventRegistry;

        public GameController(IContainer container)
        {
            _settingsService = container.Resolve<ILauncherSettingsService>();
            _dcsWorldManager = container.Resolve<IDcsWorldManager>();
            _eventRegistry = container.Resolve<ApplicationEventRegistry>();
            _profileService = container.Resolve<IProfileService>();

        }

        public Task UpdateAsync()
        {
            return Task.Run(() =>
            {
                var install = _profileService.GetSelectedInstall();

                if (!install.IsValidInstall)
                {
                    MessageBoxEx.Show($"The installation path \"{install.Directory}\" appears to be invalid.   Please fix the path and try again.");
                    return;
                }

                var processInfo = new ProcessStartInfo(install.UpdaterPath, "update");
                var process = Process.Start(processInfo);

                process?.WaitForExit();
            });
        }

        public Task RepairAsync()
        {
            return Task.Run(() =>
            {
                var install = _profileService.GetSelectedInstall();

                if (!install.IsValidInstall)
                {
                    MessageBoxEx.Show($"The installation path \"{install.Directory}\" appears to be invalid.   Please fix the path and try again.");
                    return;
                }

                var processInfo = new ProcessStartInfo(install.UpdaterPath, "repair");
                var process = Process.Start(processInfo);

                process?.WaitForExit();
            });
        }

        public async Task LaunchDcsAsync()
        {
            await Task.Run(async () =>
            {
                var install = _profileService.GetSelectedInstall();

                Guard.RequireIsNotNull(install, nameof(install));

                if (!install.IsValidInstall)
                {
                    MessageBoxEx.Show($"The installation path \"{install.Directory}\" appears to be invalid.   Please fix the path and try again.");
                    return;
                }

                await _dcsWorldManager.WriteOptionsAsync();
                await _dcsWorldManager.UpdateAdvancedOptionsAsync();

                await _eventRegistry.InvokeBeforeDcsLaunchedAsync(this, DeferredEventArgs.CreateEmpty());

                //await _dcsWorldManager.PatchViewportsAsync();
                //await _dcsWorldManager.WriteViewportOptionsAsync();

                var processInfo = new ProcessStartInfo(install.ExePath)
                {
                    UseShellExecute = true
                };

                Process.Start(processInfo);

                await _eventRegistry.InvokeAfterDcsLaunchedAsync(this, DeferredEventArgs.CreateEmpty());
            });
        }

        public void CleanupShaders(InstallLocation install)
        {
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

        public void ShowUrl(string url)
        {
            var ps = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
        }
    }
}