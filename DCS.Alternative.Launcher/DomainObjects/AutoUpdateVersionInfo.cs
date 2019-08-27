using System;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public class AutoUpdateVersionInfo
    {
        public string Version
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        internal Version ConcreteVersion
        {
            get { return new Version(Version); }
        }
    }
}