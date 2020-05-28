using System.Collections.Generic;

namespace DCS.Alternative.Launcher.Plugin.Viewports.DomainObjects
{
    public class ModuleViewportTemplate
    {
        public int? Id
        {
            get;
            set;
        }

        public string TemplateName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ViewportPrefix
        {
            get;
            set;
        }

        public bool IsHeliosTemplate
        {
            get;
            set;
        }

        public int SeatIndex
        {
            get;
            set;
        }

        public string ModuleId
        {
            get;
            set;
        }

        public string ExampleImageUrl
        {
            get;
            set;
        }

        public double? CloudRating
        {
            get;
            set;
        }

        public double? UserRating
        {
            get;
            set;
        }

        public List<MonitorDefinition> Monitors
        {
            get;
            set;
        } = new List<MonitorDefinition>();

        public List<Viewport> Viewports
        {
            get;
            set;
        } = new List<Viewport>();
    }
}