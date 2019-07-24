using System.Diagnostics;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Diagnostics
{
    public static class ProcessHelper
    {
        public static Task WaitForDcsUpdaterToFinishAsync()
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcessesByName("DCS_updater.exe");

                while (processes.Length > 0)
                {
                    Task.Delay(200);
                    processes = Process.GetProcessesByName("DCS_updater.exe");
                }
            });
        }

        public static Task WaitForDcsToFinishAsync()
        {
            return Task.Run(() =>
            {
                var processes = Process.GetProcessesByName("DCS.exe");

                while (processes.Length > 0)
                {
                    Task.Delay(200);
                    processes = Process.GetProcessesByName("DCS_updater.exe");
                }
            });
        }
    }
}