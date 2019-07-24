using System;
using System.Threading.Tasks;
using System.Windows;

namespace DCS.Alternative.Launcher.Threading
{
    public static class EventAsync
    {
        public static Task FromEvent(
            Action<EventHandler> addEventHandler,
            Action<EventHandler> removeEventHandler,
            Action beginAction = null)
        {
            return new EventHandlerTaskSource(addEventHandler, removeEventHandler, beginAction).Task;
        }

        public static Task<object> FromEvent<T>(
            Action<EventHandler<T>> addEventHandler,
            Action<EventHandler<T>> removeEventHandler,
            Action beginAction = null)
        {
            return new EventHandlerTaskSource<T>(addEventHandler, removeEventHandler, beginAction).Task;
        }

        public static Task<SizeChangedEventArgs> FromSizeChangedEvent(
            Action<SizeChangedEventHandler> addEventHandler,
            Action<SizeChangedEventHandler> removeEventHandler,
            Action beginAction = null)
        {
            return new SizeChangedEventHandlerTaskSource(addEventHandler, removeEventHandler, beginAction).Task;
        }

        public static Task<RoutedEventArgs> FromRoutedEvent(
            Action<RoutedEventHandler> addEventHandler,
            Action<RoutedEventHandler> removeEventHandler,
            Action beginAction = null)
        {
            return new RoutedEventHandlerTaskSource(addEventHandler, removeEventHandler, beginAction).Task;
        }

        private sealed class EventHandlerTaskSource : HandlerTaskSourceBase<object>
        {
            private readonly Action<EventHandler> _removeEventHandler;
            private readonly TaskCompletionSource<object> _tcs;

            public EventHandlerTaskSource(
                Action<EventHandler> addEventHandler,
                Action<EventHandler> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null)
                {
                    throw new ArgumentNullException("addEventHandler");
                }

                if (removeEventHandler == null)
                {
                    throw new ArgumentNullException("removeEventHandler");
                }

                _tcs = new TaskCompletionSource<object>();
                _removeEventHandler = removeEventHandler;

                addEventHandler.Invoke(EventCompleted);

                if (beginAction != null)
                {
                    beginAction.Invoke();
                }
            }

            public override Task<object> Task => _tcs.Task;

            private void EventCompleted(object sender, EventArgs args)
            {
                _removeEventHandler.Invoke(EventCompleted);
                _tcs.SetResult(args);
            }
        }

        private sealed class EventHandlerTaskSource<TEventArgs> : HandlerTaskSourceBase<object>
        {
            private readonly Action<EventHandler<TEventArgs>> _removeEventHandler;
            private readonly TaskCompletionSource<object> _tcs;

            public EventHandlerTaskSource(
                Action<EventHandler<TEventArgs>> addEventHandler,
                Action<EventHandler<TEventArgs>> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null)
                {
                    throw new ArgumentNullException("addEventHandler");
                }

                if (removeEventHandler == null)
                {
                    throw new ArgumentNullException("removeEventHandler");
                }

                _tcs = new TaskCompletionSource<object>();
                _removeEventHandler = removeEventHandler;

                addEventHandler.Invoke(EventCompleted);

                if (beginAction != null)
                {
                    beginAction.Invoke();
                }
            }

            public override Task<object> Task => _tcs.Task;

            private void EventCompleted(object sender, TEventArgs args)
            {
                _removeEventHandler.Invoke(EventCompleted);
                _tcs.SetResult(args);
            }
        }

        private sealed class RoutedEventHandlerTaskSource : HandlerTaskSourceBase<RoutedEventArgs>
        {
            private readonly Action<RoutedEventHandler> _removeEventHandler;
            private readonly TaskCompletionSource<RoutedEventArgs> _tcs;

            public RoutedEventHandlerTaskSource(
                Action<RoutedEventHandler> addEventHandler,
                Action<RoutedEventHandler> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null)
                {
                    throw new ArgumentNullException("addEventHandler");
                }

                if (removeEventHandler == null)
                {
                    throw new ArgumentNullException("removeEventHandler");
                }

                _tcs = new TaskCompletionSource<RoutedEventArgs>();
                _removeEventHandler = removeEventHandler;

                addEventHandler.Invoke(EventCompleted);

                if (beginAction != null)
                {
                    beginAction.Invoke();
                }
            }

            public override Task<RoutedEventArgs> Task => _tcs.Task;

            private void EventCompleted(object sender, RoutedEventArgs args)
            {
                _removeEventHandler.Invoke(EventCompleted);
                _tcs.SetResult(args);
            }
        }

        private sealed class SizeChangedEventHandlerTaskSource : HandlerTaskSourceBase<SizeChangedEventArgs>
        {
            private readonly Action<SizeChangedEventHandler> _removeEventHandler;
            private readonly TaskCompletionSource<SizeChangedEventArgs> _tcs;

            public SizeChangedEventHandlerTaskSource(
                Action<SizeChangedEventHandler> addEventHandler,
                Action<SizeChangedEventHandler> removeEventHandler,
                Action beginAction = null)
            {
                if (addEventHandler == null)
                {
                    throw new ArgumentNullException("addEventHandler");
                }

                if (removeEventHandler == null)
                {
                    throw new ArgumentNullException("removeEventHandler");
                }

                _tcs = new TaskCompletionSource<SizeChangedEventArgs>();
                _removeEventHandler = removeEventHandler;

                addEventHandler.Invoke(EventCompleted);

                if (beginAction != null)
                {
                    beginAction.Invoke();
                }
            }

            public override Task<SizeChangedEventArgs> Task => _tcs.Task;

            private void EventCompleted(object sender, SizeChangedEventArgs args)
            {
                _removeEventHandler.Invoke(EventCompleted);
                _tcs.SetResult(args);
            }
        }
    }
}