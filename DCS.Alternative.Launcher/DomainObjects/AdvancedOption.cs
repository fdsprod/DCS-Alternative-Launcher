using System.Collections.Generic;
using DCS.Alternative.Launcher.Modules;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public class AdvancedOption
    {
        public string Id
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public List<AdvancedOptionMinMax> MinMax
        {
            get;
            set;
        } = new List<AdvancedOptionMinMax>();

        public Dictionary<string, object> Params
        {
            get;
        } = new Dictionary<string, object>();
    }
}