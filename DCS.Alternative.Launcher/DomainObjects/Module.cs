using System.IO;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public class Module
    {
        public string ViewportPrefix
        {
            get;
            set;
        }

        public string BaseFolderPath
        {
            get;
            set;
        }

        public string DocumentationPath
        {
            get
            {
                var path = Path.Combine(BaseFolderPath, "Doc");

                if (!Directory.Exists(path))
                {
                    path = Path.Combine(BaseFolderPath, "Docs");
                }

                return path;
            }
        }
        public string CockpitScriptsFolderPath
        {
            get { return Path.Combine(BaseFolderPath, "Cockpit\\Scripts"); }
        }

        public string IconPath
        {
            get;
            set;
        }

        public string ModuleId
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string LoadingImagePath
        {
            get;
            set;
        }

        public string MainMenuLogoPath
        {
            get;
            set;
        }

        public bool IsFC3
        {
            get;
            set;
        }

        public string FC3ModuleId
        {
            get;
            set;
        }
    }
}