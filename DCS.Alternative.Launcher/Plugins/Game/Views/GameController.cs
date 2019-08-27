using System.Diagnostics;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Controls.MessageBoxEx;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public class GameController
    {
        public readonly IDcsWorldService _dcsWorldService;
        public readonly ISettingsService _settingsService;

        public GameController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
            _dcsWorldService = container.Resolve<IDcsWorldService>();
        }

        public Task UpdateAsync()
        {
            return Task.Run(() =>
            {
                var install = _settingsService.SelectedInstall;

                if (!install.IsValidInstall)
                {
                    MessageBoxEx.Show($"The installation path \"{install.Directory}\" appears to be invalid.   Please fix the path and try again.");
                    return;
                }

                var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.UpdaterPath, "update");
                var process = Process.Start(processInfo);

                process?.WaitForExit();
            });
        }

        public Task RepairAsync()
        {
            return Task.Run(() =>
            {
                var install = _settingsService.SelectedInstall;

                if (!install.IsValidInstall)
                {
                    MessageBoxEx.Show($"The installation path \"{install.Directory}\" appears to be invalid.   Please fix the path and try again.");
                    return;
                }

                var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.UpdaterPath, "repair");
                var process = Process.Start(processInfo);

                process?.WaitForExit();
            });
        }

        public async Task LaunchDcsAsync(bool isVREnabled)
        {
            await Task.Run(async () =>
            {
                var install = _settingsService.SelectedInstall;

                Guard.RequireIsNotNull(install, nameof(install));

                if (!install.IsValidInstall)
                {
                    MessageBoxEx.Show($"The installation path \"{install.Directory}\" appears to be invalid.   Please fix the path and try again.");
                    return;
                }

                await _dcsWorldService.WriteOptionsAsync(isVREnabled);
                await _dcsWorldService.PatchViewportsAsync();
                await _dcsWorldService.UpdateAdvancedOptionsAsync();

                var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.ExePath);
                processInfo.Arguments = isVREnabled ? "--force_enable_VR" : "--force_disable_VR";
                Process.Start(processInfo);
            });
        }
    }
}