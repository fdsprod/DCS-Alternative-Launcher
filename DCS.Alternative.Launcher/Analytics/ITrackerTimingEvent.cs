using System;

namespace DCS.Alternative.Launcher.Analytics
{
    public interface ITrackerTimingEvent : IDisposable
    {
        bool IsCancelled
        {
            get;
            set;
        }

        void Resume();

        IDisposable Suspend();
    }
}