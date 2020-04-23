using System;
using System.ComponentModel;
using System.IO;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
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

        public string[] Modules
        {
            get { return _config?.Modules ?? new string[0]; }
        }

        public string Name
        {
            get;
            set;
        }

        public bool IsValidInstall
        {
            get { return File.Exists(ExePath) && File.Exists(UpdaterPath) && File.Exists(UpdaterConfigPath); }
        }

        public string ExePath
        {
            get { return GetPath(_exePath); }
        }
        public string UpdaterPath
        {
            get { return GetPath(_updaterPath); }
        }

        public string AutoexecCfg
        {
            get { return Path.Combine(SavedGamesPath, "Config", "autoexec.cfg"); }
        }

        private static readonly Guid SavedGamesGuid = Guid.Parse("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4");

        public string SavedGamesPath
        {
            get
            {
                var variantFilePath = GetPath("dcs_variant.txt");
                var savedGamesPath = NativeMethods.GetKnownFolderPath(SavedGamesGuid);
                var variant = string.Empty;

                if (File.Exists(variantFilePath))
                {
                    var contents = File.ReadAllText(variantFilePath);
                    variant = $".{contents}";
                }

                return Path.Combine(savedGamesPath, "DCS" + variant);
            }
        }

        public string UpdaterConfigPath
        {
            get { return GetPath(_updaterConfigPath); }
        }
        public string Variant
        {
            get { return _config?.Branch ?? "stable"; }
        }
        public string DirectoryName
        {
            get { return new DirectoryInfo(Directory).Name; }
        }
        public DcsVersion Version
        {
            get
            {
                if (_config == null)
                {
                    return new DcsVersion(0, 0, 0, 0, 0);
                }

                return DcsVersion.TryParse(_config.Version, out var result) ? result : new DcsVersion(0, 0, 0, 0, 0);
            }
        }

        public string Directory
        {
            get;
        }

        public void RefreshInfo()
        {
            if (File.Exists(UpdaterConfigPath))
            {
                Tracer.Info($"Updater config found in {Directory}");
                _config = JsonConvert.DeserializeObject<AutoUpdaterConfig>(File.ReadAllText(UpdaterConfigPath));
                Tracer.Info($"Install Detected as branch {_config.Branch} {_config.Version}");
            }
            else
            {
                Tracer.Info($"Updater config was not found in {Directory}");
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

        public string GetPath(string filename)
        {
            var path = Path.Combine(Directory, filename);

            if (!File.Exists(path))
            {
                Tracer.Warn("{0} does not exists.", path);
            }

            return path;
        }

        public override string ToString()
        {
            return $"{Version}-{Variant} ({Directory})";
        }
    }

    public class AutoUpdaterConfig
    {
        public string Branch
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string Timestamp
        {
            get;
            set;
        }

        public string Arch
        {
            get;
            set;
        }

        public string Lang
        {
            get;
            set;
        }

        public string Launch
        {
            get;
            set;
        }

        public string[] Modules
        {
            get;
            set;
        }
    }
}