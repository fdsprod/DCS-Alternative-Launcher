using System.IO;

namespace DCS.Alternative.Launcher.Diagnostics.Trace.Listeners
{
    public sealed class FileLogEventListener : StreamOuputEventListener
    {
        public FileLogEventListener(string filename)
            : base(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read), true)
        {
        }
    }
}