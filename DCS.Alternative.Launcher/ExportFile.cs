using DCS.Alternative.Launcher.Plugins.Settings.Dialogs;

namespace DCS.Alternative.Launcher
{
    public class Bounds
    {
        public int X
        {
            get;
            set;
        }
        public int Y
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
    }

    public class Viewport
    {
        public Viewport()
        {
        }

        public string MonitorId
        {
            get;
            set;
        }

        public Bounds Bounds
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string InitFileName
        {
            get;
            set;
        }

        public LocationIndicator Location
        {
            get;
            set; 
        }
    }
}