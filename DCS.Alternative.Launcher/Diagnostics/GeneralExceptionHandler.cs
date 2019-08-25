using System;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Analytics;
using DCS.Alternative.Launcher.Diagnostics.Trace;

namespace DCS.Alternative.Launcher.Diagnostics
{
    public class GeneralExceptionHandler
    {
        private static GeneralExceptionHandler _instance;

        public static GeneralExceptionHandler Instance
        {
            get { return _instance ?? (_instance = new GeneralExceptionHandler()); }
            set { _instance = value; }
        }

        public async void OnError(Exception e)
        {
            Tracker.Instance.SendException(e.ToString(), false);
            Tracer.Error(e);

            await OnErrorOverrideAsync(e);
        }

        public Task OnErrorAsync(Exception e)
        {
            Tracker.Instance.SendEvent(AnalyticsCategories.Exceptions, e.GetType().Name, e.Message);
            Tracker.Instance.SendException(e.ToString(), false);
            Tracer.Error(e);

            return OnErrorOverrideAsync(e);
        }

        protected virtual Task OnErrorOverrideAsync(Exception e)
        {
            return Task.FromResult(true);
        }
    }
}