using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;

namespace DCS.Alternative.Launcher.Diagnostics.Trace.Listeners
{
    public class StreamOuputEventListener : EventListener
    {
        private readonly bool m_closeStreamOnDispose;
        private readonly object m_syncRoot = new object();
        private readonly StreamWriter m_writer;
        private Stream m_stream;

        public StreamOuputEventListener(Stream stream, bool closeStreamOnDispose)
        {
            m_stream = stream;
            m_writer = new StreamWriter(m_stream, Encoding.Unicode);
            m_closeStreamOnDispose = closeStreamOnDispose;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (!m_closeStreamOnDispose || m_stream == null)
            {
                return;
            }

            m_stream.Dispose();
            m_stream = null;
        }

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            var output = $"{DateTime.Now:hh:mm tt} [{e.Level,13}] {e.Payload[0]}";

            lock (m_syncRoot)
            {
                m_writer.WriteLine(output);
                m_writer.Flush();
            }
        }
    }
}