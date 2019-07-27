using System.Diagnostics;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.ServiceModel;
using DCS.Alternative.Launcher.Services;

namespace DCS.Alternative.Launcher.Plugins.Game.Views
{
    public class GameController
    {
        public readonly ISettingsService _settingsService;

        public GameController(IContainer container)
        {
            _settingsService = container.Resolve<ISettingsService>();
        }

        public Task UpdateAsync()
        {
            return Task.Run(() =>
            {
                var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.UpdaterPath, "update");
                var process = Process.Start(processInfo);

                process?.WaitForExit();
            });
        }

        public Task RepairAsync()
        {
            return Task.Run(() =>
            {
                var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.UpdaterPath, "repair");
                var process = Process.Start(processInfo);

                process?.WaitForExit();
            });

        }

        public Task LaunchDcsAsync(bool isVREnabled)
        {
            return Task.Run(() =>
            {
                var moduleViewports = _settingsService.GetModuleViewports();

                foreach (var mv in moduleViewports)
                {
                    mv.PatchViewports(_settingsService.SelectedInstall);
                }

                var processInfo = new ProcessStartInfo(_settingsService.SelectedInstall.ExePath);
                processInfo.Arguments = isVREnabled ? "--force_enable_VR" : "--force_disable_VR";
                Process.Start(processInfo);
            });
        }
    }
}