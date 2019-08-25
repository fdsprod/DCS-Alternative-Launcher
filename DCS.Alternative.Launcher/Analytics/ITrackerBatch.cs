using System;

namespace DCS.Alternative.Launcher.Analytics
{
    public interface ITrackerBatch : IDisposable
    {
        bool IsCancelled
        {
            get;
            set;
        }

        ITrackerBatch AddScreenView(string screenName);

        ITrackerBatch AddEvent(string category, string action, string label, int? value);

        ITrackerBatch AddTiming(string category, string action, string label, long milliseconds);

        ITrackerBatch AddException(string description, bool fatal);

        void Flush();
    }
}