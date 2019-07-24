using System;
using System.Threading.Tasks;
using System.Windows;
using DCS.Alternative.Launcher.Diagnostics.Trace;

namespace DCS.Alternative.Launcher.Diagnostics
{
    public class GeneralExceptionHandler
    {
        private static GeneralExceptionHandler _instance;

        public static GeneralExceptionHandler Instance
        {
            get => _instance ?? (_instance = new GeneralExceptionHandler());
            set => _instance = value;
        }

        public async void OnError(Exception e)
        {
            Tracer.Error(e);
            await OnErrorOverrideAsync(e);
        }

        public Task OnErrorAsync(Exception e)
        {
            Tracer.Error(e);
            return OnErrorOverrideAsync(e);
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        protected virtual async Task OnErrorOverrideAsync(Exception e)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            var dispatcher = Application.Current.Dispatcher;
            var contents = e.ToString();

            if (dispatcher.CheckAccess())
            {
            }
        }
    }
}