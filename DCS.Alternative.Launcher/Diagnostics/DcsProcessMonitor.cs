using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace DCS.Alternative.Launcher.Diagnostics
{
    public class DcsProcessMonitor
    {
        private const uint PathBufferSize = 512;
        private readonly StringBuilder _pathBuffer = new StringBuilder((int) PathBufferSize);
        private int _dcsProcessCountLastCheck;
        private Timer _processMonitorTimer;

        private DcsProcessMonitor()
        {
             _processMonitorTimer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        public static DcsProcessMonitor Instance
        {
            get;
        } = new DcsProcessMonitor();

        public event EventHandler DcsProcessExited;

        public event EventHandler DcsProcessStarted;

        private string GetExecutablePath(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return string.Empty;
            } // not a valid window handle

            // Get the process id
            NativeMethods.GetWindowThreadProcessId(hwnd, out var processId);

            // Try the GetModuleFileName method first since it's the fastest. 
            // May return ACCESS_DENIED (due to VM_READ flag) if the process is not owned by the current user.
            // Will fail if we are compiled as x86 and we're trying to open a 64 bit process...not allowed.
            var hProcess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.Read, false, processId);

            if (hProcess != IntPtr.Zero)
            {
                try
                {
                    if (NativeMethods.GetModuleFileNameEx(hProcess, IntPtr.Zero, _pathBuffer, PathBufferSize) > 0)
                    {
                        return _pathBuffer.ToString();
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hProcess);
                }
            }

            hProcess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation, false, processId);

            if (hProcess == IntPtr.Zero)
            {
                return string.Empty;
            }

            try
            {
                // Try this method for Vista or higher operating systems
                var size = PathBufferSize;
                if (Environment.OSVersion.Version.Major >= 6 && NativeMethods.QueryFullProcessImageName(hProcess, 0, _pathBuffer, ref size) && size > 0)
                {
                    return _pathBuffer.ToString();
                }

                // Try the GetProcessImageFileName method
                if (NativeMethods.GetProcessImageFileName(hProcess, _pathBuffer, PathBufferSize) > 0)
                {
                    var path = _pathBuffer.ToString();

                    foreach (var drive in Environment.GetLogicalDrives())
                    {
                        if (NativeMethods.QueryDosDevice(drive.TrimEnd('\\'), _pathBuffer, PathBufferSize) <= 0)
                        {
                            continue;
                        }

                        if (path.StartsWith(_pathBuffer.ToString()))
                        {
                            return drive + path.Remove(0, _pathBuffer.Length);
                        }
                    }
                }
            }
            finally
            {
                NativeMethods.CloseHandle(hProcess);
            }

            return string.Empty;
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

        private string[] GetPathsByProcessName(string processName)
        {
            var results = new List<string>();
            var processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                var path = GetExecutablePath(process.MainWindowHandle);

                if (path != string.Empty)
                {
                    results.Add(path);
                }
            }

            return results.ToArray();
        }

        public bool IsDcsInstallUpdating(InstallLocation install)
        {
            var paths = GetPathsByProcessName("DCS_updater");

            return paths.Any(path => path == install.ExePath);
        }

        public bool IsDcsInstallRunning(InstallLocation install)
        {
            var paths = GetPathsByProcessName("DCS");

            return paths.Any(path => string.Compare(path, install.ExePath, StringComparison.CurrentCultureIgnoreCase) == 0);
        }
    }
}