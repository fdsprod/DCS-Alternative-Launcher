using System;

namespace DCS.Alternative.Launcher.Services.Navigation
{
    public class NavigatingEventArgs : EventArgs
    {
        public NavigatingEventArgs(Type to, Type from)
        {
            NavigatingTo = to;
            NavigatingFrom = from;
        }

        public Type NavigatingTo { get; }

        public Type NavigatingFrom { get; }

        public bool Cancel { get; set; }
    }
}