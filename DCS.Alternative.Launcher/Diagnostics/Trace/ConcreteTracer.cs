using System;
using System.Text;

namespace DCS.Alternative.Launcher.Diagnostics.Trace
{
    public sealed class ConcreteTracer : ITracer
    {
        public static readonly ConcreteTracer Instance = new ConcreteTracer();

        public void Critical(Exception ex)
        {
            Guard.Require<ArgumentNullException>(ex != null, "ex");
            TracerEventSource.Instance.Critical(getFormattedString(ex, string.Empty, new object[] { }));
        }

        public void Critical(Exception ex, string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Critical(getFormattedString(ex, message, args));
        }

        public void Error(Exception ex)
        {
            Guard.Require<ArgumentNullException>(ex != null, "ex");
            TracerEventSource.Instance.Error(getFormattedString(ex, string.Empty, new object[] { }));
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Error(getFormattedString(ex, message, args));
        }

        public void Warn(Exception ex)
        {
            Guard.Require<ArgumentNullException>(ex != null, "ex");
            TracerEventSource.Instance.Warn(getFormattedString(ex, string.Empty, new object[] { }));
        }

        public void Warn(Exception ex, string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Warn(getFormattedString(ex, message, args));
        }

        public void Warn(string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Warn(getFormattedString(null, message, args));
        }

        public void Verbose(string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Verbose(getFormattedString(null, message, args));
        }

        public void Info(string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Info(getFormattedString(null, message, args));
        }

        public void Critical(string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Warn(getFormattedString(null, message, args));
        }

        public void Error(string message, params object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Error(getFormattedString(null, message, args));
        }

        public void Debug(string message, object[] args)
        {
            Guard.Require<ArgumentNullException>(message != null, "message");
            TracerEventSource.Instance.Verbose(getFormattedString(null, message, args));
            System.Diagnostics.Debug.WriteLine(getFormattedString(null, message, args));
        }

        private string getFormattedString(Exception ex, string message, object[] args)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(message))
            {
                if (args.Length > 0)
                {
                    sb.AppendFormat(message, args);
                }
                else
                {
                    sb.Append(message);
                }
            }

            if (ex != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Environment.NewLine).Append(Environment.NewLine);
                }

                sb.Append(ex);
            }

            return sb.ToString();
        }
    }
}