using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using DCS.Alternative.Launcher.ComponentModel;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher
{
    [TypeConverter(typeof(InstallLocationTypeConverter))]
    public class InstallLocation
    {
        private string _variant;
        private string _variantDisplay;
        private Version _version;

        public InstallLocation(string directory)
        {
            Directory = directory;
        }

        public string Variant
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_variant)) return _variant;

                var autoupdaterConfigFile = GetPath("autoupdate.cfg");

                if (!File.Exists(autoupdaterConfigFile)) return _variant = "Unknown";

                var config = JsonConvert.DeserializeObject<AutoUpdaterConfig>(File.ReadAllText(autoupdaterConfigFile));
                _variant = config.Branch;
                return _variant;
            }
        }

        public string DisplayVariant
        {
            get
            {
                _variantDisplay = Variant?.ToUpper();

                return _variantDisplay;
            }
        }

        public Version Version
        {
            get
            {
                if (_version == null)
                {
                    var clientExe = GetExePath();

                    if (File.Exists(clientExe))
                    {
                        var fileVersionInfo = GetClientVersion();
                        _version = new Version(
                            fileVersionInfo.FileMajorPart,
                            fileVersionInfo.FileMinorPart,
                            fileVersionInfo.FileBuildPart,
                            fileVersionInfo.FilePrivatePart);
                    }
                    else
                    {
                        _version = new Version("0.0.0.0");
                    }
                }

                return _version;
            }
        }

        public string Directory
        {
            get;
        }

        public FileStream Create(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename)) throw new FileNotFoundException(filename);

            return File.Create(filename);
        }

        public FileStream OpenWrite(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename)) throw new FileNotFoundException(filename);

            return File.OpenWrite(filename);
        }

        public FileStream OpenRead(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename)) throw new FileNotFoundException(filename);

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

            if (!File.Exists(filename)) throw new FileNotFoundException(filename);

            return File.ReadAllText(filename);
        }

        public void WriteAllText(string filename, string contents)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename)) throw new FileNotFoundException(filename);

            File.WriteAllText(filename, contents);
        }

        private FileVersionInfo GetClientVersion()
        {
            var clientExe = GetExePath();

            if (!File.Exists(clientExe)) throw new FileNotFoundException(clientExe);

            return FileVersionInfo.GetVersionInfo(clientExe);
        }

        public string GetExePath()
        {
            return GetPath("bin\\dcs.exe");
        }

        public string GetUpdaterPath()
        {
            return GetPath("bin\\dcs_updater.exe");
        }

        public string GetPath(string filename, params object[] args)
        {
            var path = Path.Combine(Directory, string.Format(filename, args));

            if (!File.Exists(path)) Tracer.Warn("{0} does not exists.", path);

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
            return $"{DisplayVariant} ({Directory})";
        }

        public class AutoUpdaterConfig
        {
            public string Branch
            {
                get;
                set;
            }
        }
    }
}