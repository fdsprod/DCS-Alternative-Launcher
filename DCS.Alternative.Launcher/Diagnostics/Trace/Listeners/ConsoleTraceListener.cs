using System;
using System.Diagnostics.Tracing;

namespace DCS.Alternative.Launcher.Diagnostics.Trace.Listeners
{
    public class ConsoleOutputEventListener : EventListener
    {
        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var color = ConsoleColor.Gray;

            switch (e.Level)
            {
                case EventLevel.Informational:
                    color = ConsoleColor.White;
                    break;
                case EventLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case EventLevel.Error:
                case EventLevel.Critical:
                    color = ConsoleColor.Red;
                    break;
            }

            ConsoleManager.PushColor(color);
            Console.WriteLine(e.Payload[0]);
            ConsoleManager.PopColor();
        }
    }
}