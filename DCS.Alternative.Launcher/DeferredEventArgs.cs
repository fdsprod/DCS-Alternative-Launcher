using System;
using System.Threading;
using System.Threading.Tasks;

namespace DCS.Alternative.Launcher.Services.Settings
{
    public class DeferredEventArgs : EventArgs
    {
        private readonly object _syncRoot = new object();

        private EventDeferral _deferral;

        public static DeferredEventArgs CreateEmpty()
        {
            return new DeferredEventArgs();
        }

        public IDisposable GetDeferral()
        {
            lock (_syncRoot)
            {
                return _deferral ??= new EventDeferral();
            }
        }

        internal EventDeferral GetCurrentDeferralAndReset()
        {
            lock (_syncRoot)
            {
                var eventDeferral = _deferral;

                _deferral = null;

                return eventDeferral;
            }
        }

        internal class EventDeferral : IDisposable
        {
            private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

            private void Complete()
            {
                _tcs.TrySetResult(null);
            }

            internal async Task WaitForCompletion(CancellationToken cancellationToken)
            {
                using (cancellationToken.Register(() => _tcs.TrySetCanceled()))
                {
                    await _tcs.Task;
                }
            }

            public void Dispose()
            {
                Complete();
            }
        }
    }
}