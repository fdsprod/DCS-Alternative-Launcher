using DCS.Alternative.Launcher.DomainObjects;

namespace DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects
{
    public class ViewportOption : Option
    {
        public string FilePath
        {
            get;
            set;
        }

        public string Regex
        {
            get;
            set;
        }
    }
}