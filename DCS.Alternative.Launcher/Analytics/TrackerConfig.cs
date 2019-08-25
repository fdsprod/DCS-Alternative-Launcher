using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Analytics
{
    public class TrackerConfig
    {
        public string TrackerUrl
        {
            get;
            set;
        }

        public string TrackerVersion
        {
            get;
            set;
        }

        public string TrackingId
        {
            get;
            set;
        }

        public string ClientId
        {
            get;
            set;
        }

        public string UId
        {
            get;
            set;
        }

        public string AppName
        {
            get;
            set;
        }

        public string AppVersion
        {
            get;
            set;
        }

        public string AppId
        {
            get;
            set;
        }

        public string AppInstallerId
        {
            get;
            set;
        }

        public bool Debug
        {
            get;
            set;
        }
    }
}
