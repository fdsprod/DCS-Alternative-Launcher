using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.DomainObjects
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
