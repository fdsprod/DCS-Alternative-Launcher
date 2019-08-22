using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public class DcsOptionsCategory
    {
        public string Id
        {
            get;set;
        }

        public string DisplayName
        {
            get;set;
        }

        public Option[] Options
        {
            get;set;
        }
    }
}
