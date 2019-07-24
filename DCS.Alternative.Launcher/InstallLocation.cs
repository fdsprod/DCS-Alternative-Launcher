using System;
using System.ComponentModel;
using System.IO;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher
{
    [TypeConverter(typeof(InstallLocationTypeConverter))]
    public class InstallLocation
    {
        private const string _exePath = "bin\\dcs.exe";
        private const string _updaterPath = "bin\\dcs_updater.exe";
        private const string _updaterConfigPath = "autoupdate.cfg";

        private AutoUpdaterConfig _config;

        public InstallLocation(string directory)
        {
            Directory = directory;

            RefreshInfo();
        }

        public string[] Modules => _config.Modules;

        public bool IsValidInstall =>
            File.Exists(ExePath) && File.Exists(UpdaterPath) && File.Exists(UpdaterConfigPath);

        public string ExePath => GetPath(_exePath);

        public string UpdaterPath => GetPath(_updaterPath);

        public string UpdaterConfigPath => GetPath(_updaterConfigPath);

        public string Variant => _config?.Branch ?? "Unknown";

        public Version Version => Version.TryParse(_config.Version, out var result) ? result : new Version(0, 0, 0, 0);

        public string Directory { get; }

        public void RefreshInfo()
        {
            if (File.Exists(UpdaterConfigPath))
            {
                _config = JsonConvert.DeserializeObject<AutoUpdaterConfig>(File.ReadAllText(UpdaterConfigPath));
            }
        }

        public FileStream OpenWrite(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            return File.OpenWrite(filename);
        }

        public FileStream OpenRead(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            return File.OpenRead(filename);
        }

        public bool FileExists(string filename)
        {
            filename = GetPath(filename);

            return File.Exists(filename);
        }

        public string ReadAllText(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            return File.ReadAllText(filename);
        }

        public void WriteAllText(string filename, string contents)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            File.WriteAllText(filename, contents);
        }

        public string GetPath(string filename, params object[] args)
        {
            var path = Path.Combine(Directory, string.Format(filename, args));

            if (!File.Exists(path))
            {
                Tracer.Warn("{0} does not exists.", path);
            }

            return path;
        }

        public static implicit operator string(InstallLocation a)
        {
            return a.Directory;
        }

        public static implicit operator InstallLocation(string a)
        {
            return new InstallLocation(a);
        }

        public override string ToString()
        {
            return $"{Version}-{Variant} ({Directory})";
        }
    }

    public class AutoUpdaterConfig
    {
        public string Branch { get; set; }

        public string Version { get; set; }

        public string Timestamp { get; set; }

        public string Arch { get; set; }

        public string Lang { get; set; }

        public string Launch { get; set; }

        public string[] Modules { get; set; }
    }
}