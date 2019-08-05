using System;
using System.Collections.Generic;
using System.IO;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;

namespace DCS.Alternative.Launcher.Modules
{
    public class Module
    {
        public string ViewportPrefix { get; set; }

        public string BaseFolderPath { get; set; }

        public string CockpitScriptsFolderPath { get { return Path.Combine(BaseFolderPath, "Cockpit\\Scripts"); } }

        public string IconPath { get; set; }

        public string ModuleId { get; set; }

        public string DisplayName { get; set; }

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
    }
}