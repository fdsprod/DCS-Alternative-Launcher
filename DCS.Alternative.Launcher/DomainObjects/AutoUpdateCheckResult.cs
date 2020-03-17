using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.DomainObjects
{
    public class AutoUpdateCheckResult
    {
        public Version UpdateVersion
        {
            get;
            set;
        }

        public bool IsUpdateAvailable
        {
            get;
            set;
        }

        public Task UpdatingTask
        {
            get;
            set;
        } = Task.FromResult(true);
    }
}
