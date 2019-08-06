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
    }
}