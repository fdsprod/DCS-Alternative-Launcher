using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DCS.Alternative.Launcher.Diagnostics
{
    public class DcsProcessMonitor
    {
        public static DcsProcessMonitor Instance
        {
            get;
        } = new DcsProcessMonitor();

        private Timer _processMonitorTimer;
        private int _dcsProcessCountLastCheck;

        public EventHandler DcsProcessExited;
        public EventHandler DcsProcessStarted;

        private DcsProcessMonitor()
        {
            _processMonitorTimer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void OnTick(object state)
        {
            var processes = Process.GetProcessesByName("DCS");

            if (_dcsProcessCountLastCheck > processes.Length)
            {
                var handler = DcsProcessExited;
                handler?.Invoke(this, EventArgs.Empty);
            }

            if (_dcsProcessCountLastCheck < processes.Length)
            {
                var handler = DcsProcessStarted;
                handler?.Invoke(this, EventArgs.Empty);
            }

            _dcsProcessCountLastCheck = processes.Length;
        }

        public bool IsUpdating()
        {
            var processes = Process.GetProcessesByName("DCS_updater");

            return processes.Length > 0;
        }

        public bool IsDcsRunning()
        {
            var processes = Process.GetProcessesByName("DCS");

            return processes.Length > 0;
        }

        public bool IsDcsInstallUpdating(InstallLocation install)
        {
            var processes = Process.GetProcessesByName("DCS_updater");

            return processes.Any(p => p.MainModule.FileName == install.ExePath);
        }

        public bool IsDcsInstallRunning(InstallLocation install)
        {
            var processes  = Process.GetProcessesByName("DCS");

            return processes.Any(p => p.MainModule.FileName == install.ExePath);
        }
    }
}