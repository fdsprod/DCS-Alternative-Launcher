using System.IO;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public abstract class ModuleBase
    {
        protected ModuleBase(string baseFolderPath, ModuleClassification classification)
        {
            BaseFolderPath = baseFolderPath;
            Classification = classification;
        }

        public ModuleClassification Classification
        {
            get;
        }

        public string BaseFolderPath
        {
            get;
        }

        public bool IsActive
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