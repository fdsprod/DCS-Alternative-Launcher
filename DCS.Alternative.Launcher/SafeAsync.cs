using System;
using System.Threading.Tasks;
using DCS.Alternative.Launcher.Diagnostics;

namespace DCS.Alternative.Launcher
{
    public static class SafeAsync
    {
        public static async void Run(Func<Task> task, Action<Exception> onError = null)
        {
            try
            {
                await task();
            }
            catch (Exception e)
            {
                if (onError != null)
                {
                    onError(e);
                }
                else
                {
                    GeneralExceptionHandler.Instance.OnError(e);
                }
            }
        }

        public static async Task RunAsync(Func<Task> task, Action<Exception> onError = null)
        {
            try
            {
                await task();
            }
            catch (Exception e)
            {
                if (onError != null)
                {
                    onError(e);
                }
                else
                {
                    GeneralExceptionHandler.Instance.OnError(e);
                }
            }
        }
    }
}