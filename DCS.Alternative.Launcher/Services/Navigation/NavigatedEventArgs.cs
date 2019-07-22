using System;

namespace DCS.Alternative.Launcher.Services.Navigation
{
    public class NavigatedEventArgs : EventArgs
    {
        public NavigatedEventArgs(Type to, Type from)
        {
            NavigatedTo = to;
            NavigatedFrom = from;
        }

        public Type NavigatedTo
        {
            get;
        }

        public Type NavigatedFrom
        {
            get;
        }
    }
}