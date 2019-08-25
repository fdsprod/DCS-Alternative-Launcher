using System;
using System.Reactive.Disposables;

namespace DCS.Alternative.Launcher.Analytics
{
    public class NullTracker : ITracker
    {
        public void SetCustomDimension(int key, string value)
        {
        }

        public ITrackerBatch CreateBatch()
        {
            return new NullTrackerBatch();
        }

        public void SendScreenView(string screenName)
        {
            throw new NotImplementedException();
        }

        public void SendEvent(string category, string action, string label, int? value)
        {
        }

        public ITrackerTimingEvent CreateTimingEvent(string categoryName, string action, string label)
        {
            return new NullTrackerTimingEvent();
        }

        public void SendTiming(string category, string action, string label, long milliseconds)
        {
        }

        public void SendException(string description, bool fatal)
        {
        }

        public void SendSocial(string action, string network, string target)
        {
        }

        private class NullTrackerTimingEvent : ITrackerTimingEvent
        {
            public void Dispose()
            {
            }

            public bool IsCancelled
            {
                get;
                set;
            }

            public void Resume()
            {
            }

            public IDisposable Suspend()
            {
                return Disposable.Create(() => { });
            }
        }

        private class NullTrackerBatch : ITrackerBatch
        {
            public void Dispose()
            {
            }

            public bool IsCancelled
            {
                get;
                set;
            }

            public ITrackerBatch AddScreenView(string screenName)
            {
                return this;
            }

            public ITrackerBatch AddEvent(string category, string action, string label, int? value)
            {
                return this;
            }

            public ITrackerBatch AddTiming(string category, string action, string label, long milliseconds)
            {
                return this;
            }

            public ITrackerBatch AddException(string description, bool fatal)
            {
                return this;
            }

            public void Flush()
            {
            }
        }
    }
}