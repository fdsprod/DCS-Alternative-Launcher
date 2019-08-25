using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Reactive.Disposables;
using System.Threading;

namespace DCS.Alternative.Launcher.Analytics
{
    public static class AnalyticsCategories
    {
        public const string Exceptions = "exceptions";
        public const string AppLifecycle = "app_lifecycle";
        public const string Navigation = "navigation";
    }

    public static class AnalyticsTimingActions
    {
    }

    public static class AnalyticsEvents
    {
        public const string StartupComplete = "startup_complete";
        public const string Ping = "ping";
    }

    public class Tracker : ITracker
    {
        public static ITracker Instance
        {
            get;
            internal set;
        }

        private readonly object _syncRoot = new object();
        private readonly TrackerConfig _config;

        private NameValueCollection _basePostDataValues;

        public Tracker(TrackerConfig config)
        {
            _config = config;
        }

        protected NameValueCollection BasePostDataValues
        {
            get
            {
                if (_basePostDataValues == null)
                {
                    lock (_syncRoot)
                    {
                        if (_basePostDataValues == null)
                        {
                            _basePostDataValues = new NameValueCollection();
                            _basePostDataValues.Add("v", _config.TrackerVersion); // GA Version
                            _basePostDataValues.Add("tid", _config.TrackingId); // Tracking ID / Web property / Property ID
                            _basePostDataValues.Add("an", _config.AppName);
                            _basePostDataValues.Add("av", _config.AppVersion);
                            _basePostDataValues.Add("cid", _config.ClientId); // Anonymous Client ID
                            _basePostDataValues.Add("uid", _config.UId); // User ID
                        }
                    }
                }

                return _basePostDataValues;
            }
        }

        public void SetCustomDimension(int key, string value)
        {
            try
            {
                BasePostDataValues[$"cd{key}"] = value;
            }
            catch
            {
            }
        }

        public ITrackerBatch CreateBatch()
        {
            return new TrackerBatch(this);
        }

        public void SendScreenView(string screenName)
        {
            try
            {
                var nvc = HitInfoFactory.CreateScreenHit(BasePostDataValues, screenName);

                sendImmediately(nvc);
            }
            catch
            {
            }
        }

        public void SendEvent(string category, string action, string label, int? value)
        {
            try
            {
                var nvc = HitInfoFactory.CreateEventHit(BasePostDataValues, category, action, label, value);

                sendImmediately(nvc);
            }
            catch
            {
            }
        }

        public ITrackerTimingEvent CreateTimingEvent(string categoryName, string action, string label)
        {
            return TimingEvent.Create(elapsed => SendTiming(categoryName, action, label, (long) elapsed.TotalMilliseconds));
        }

        public void SendTiming(string category, string action, string label, long milliseconds)
        {
            try
            {
                var nvc = HitInfoFactory.CreateTimingHit(BasePostDataValues, category, action, label, milliseconds);

                sendImmediately(nvc);
            }
            catch
            {
            }
        }

        public void SendException(string description, bool fatal)
        {
            try
            {
                var nvc = HitInfoFactory.CreateExceptionHit(BasePostDataValues, description, fatal);

                sendImmediately(nvc);
            }
            catch
            {
            }
        }

        public void SendSocial(string action, string network, string target)
        {
            try
            {
                var nvc = HitInfoFactory.CreateSocialHit(BasePostDataValues, action, network, target);

                sendImmediately(nvc);
            }
            catch
            {
            }
        }

        private void sendBatch(NameValueCollection[] values)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                using (var client = new WebClient())
                {
                    foreach (var value in values)
                    {
                        try
                        {
                            client.UploadValues(new Uri(_config.TrackerUrl), value);
                        }
                        catch
                        {
#if DEBUG
                            Debug.WriteLine("Exception occurred while POSTing Analytics Data in Tracker.sendImmediately method");
#endif
                        }
                    }
                }
            });
        }

        private void sendImmediately(NameValueCollection values)
        {
            if (values == null)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        client.UploadValuesAsync(new Uri(_config.TrackerUrl), values);
                    }
                    catch
                    {
#if DEBUG
                        Debug.WriteLine("Exception occurred while POSTing Analytics Data in Tracker.sendImmediately method");
#endif
                    }
                }
            });
        }

        private class TrackerBatch : ITrackerBatch
        {
            private readonly Tracker _tracker;
            private readonly object _syncRoot = new object();
            private readonly List<NameValueCollection> _cache = new List<NameValueCollection>();

            public TrackerBatch(Tracker tracker)
            {
                _tracker = tracker;
            }

            public bool IsCancelled
            {
                get;
                set;
            }

            public void Dispose()
            {
                Flush();
            }

            public ITrackerBatch AddScreenView(string screenName)
            {
                var hit = HitInfoFactory.CreateScreenHit(_tracker.BasePostDataValues, screenName);

                checkFlush();

                lock (_syncRoot)
                {
                    _cache.Add(hit);
                }

                return this;
            }

            public ITrackerBatch AddEvent(string category, string action, string label, int? value)
            {
                var hit = HitInfoFactory.CreateEventHit(_tracker.BasePostDataValues, category, action, label, value);

                checkFlush();

                lock (_syncRoot)
                {
                    _cache.Add(hit);
                }

                return this;
            }

            public ITrackerBatch AddTiming(string category, string action, string label, long milliseconds)
            {
                var hit = HitInfoFactory.CreateTimingHit(_tracker.BasePostDataValues, category, action, label, milliseconds);

                checkFlush();

                lock (_syncRoot)
                {
                    _cache.Add(hit);
                }

                return this;
            }

            public ITrackerBatch AddException(string description, bool fatal)
            {
                var hit = HitInfoFactory.CreateExceptionHit(_tracker.BasePostDataValues, description, fatal);

                checkFlush();

                lock (_syncRoot)
                {
                    _cache.Add(hit);
                }

                return this;
            }

            public void Flush()
            {
                NameValueCollection[] values;

                lock (_syncRoot)
                {
                    values = _cache.ToArray();
                    _cache.Clear();
                }

                _tracker.sendBatch(values);
            }

            private void checkFlush()
            {
                lock (_syncRoot)
                {
                    if (_cache.Count > 500)
                    {
                        Flush();
                    }
                }
            }
        }

        private class TimingEvent : ITrackerTimingEvent
        {
            public static ITrackerTimingEvent Create(Action<TimeSpan> action)
            {
                return new TimingEvent(action);
            }

            private readonly Action<TimeSpan> _action;
            private readonly Stopwatch _stopwatch;

            private TimeSpan _totalSuspendTime;

            private Stopwatch _suspendStopwatch;

            private TimingEvent(Action<TimeSpan> action)
            {
                _totalSuspendTime = TimeSpan.Zero;
                _action = action;
                _stopwatch = Stopwatch.StartNew();
            }

            public bool IsCancelled
            {
                get;
                set;
            }

            public IDisposable Suspend()
            {
                if (!(_suspendStopwatch?.IsRunning ?? false))
                {
                    _suspendStopwatch = Stopwatch.StartNew();
                }

                return Disposable.Create(Resume);
            }

            public void Resume()
            {
                if (_suspendStopwatch?.IsRunning ?? false)
                {
                    _suspendStopwatch.Stop();
                    _totalSuspendTime += _suspendStopwatch.Elapsed;
                }
            }

            public void Dispose()
            {
                if (IsCancelled)
                {
                    return;
                }

                _stopwatch.Stop();

                var elapsed = _stopwatch.Elapsed - _totalSuspendTime;

                _action(elapsed);
            }
        }
    }
}