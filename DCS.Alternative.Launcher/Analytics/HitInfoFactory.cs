using System.Collections.Specialized;

namespace DCS.Alternative.Launcher.Analytics
{
    public static class HitInfoFactory
    {
        public static NameValueCollection CreateSocialHit(NameValueCollection baseValues, string action, string network, string target)
        {
            var nvc = new NameValueCollection(baseValues);

            nvc.Add("t", "social"); // Social hit type.
            nvc.Add("dh", action); // Social Action.         Required.
            nvc.Add("dp", network); // Social Network.        Required.
            nvc.Add("dt", target); // Social Target.         Required.

            return nvc;
        }

        public static NameValueCollection CreateScreenHit(NameValueCollection baseValues, string screenName)
        {
            var nvc = new NameValueCollection(baseValues);

            nvc.Add("t", "pageview"); // Pageview hit type.
            nvc.Add("dl", screenName);  // Page.

            return nvc;
        }

        public static NameValueCollection CreateEventHit(NameValueCollection baseValues, string category, string action, string label, int? value)
        {
            var nvc = new NameValueCollection(baseValues);

            nvc.Add("t", "event");   // Event hit type
            nvc.Add("ec", category); // Event Category. Required.
            nvc.Add("ea", action);   // Event Action. Required.

            if (!string.IsNullOrEmpty(label))
            {
                nvc.Add("el", label); // Event label.
            }

            if (value.HasValue)
            {
                nvc.Add("ev", value.Value.ToString()); // Event value.
            }

            return nvc;
        }

        public static NameValueCollection CreateTimingHit(NameValueCollection baseValues, string category, string action, string label, long milliseconds)
        {
            var nvc = new NameValueCollection(baseValues);

            nvc.Add("t", "timing"); // Event hit type
            nvc.Add("utc", category); // Event Category. Required.
            nvc.Add("utv", action); // Event Action. Required.
            nvc.Add("utt", milliseconds.ToString());

            if (!string.IsNullOrEmpty(label))
            {
                nvc.Add("utl", label); // Event label.
            }

            return nvc;
        }

        public static NameValueCollection CreateExceptionHit(NameValueCollection baseValues, string description, bool fatal)
        {
            var nvc = new NameValueCollection(baseValues);

            nvc.Add("t", "exception"); // Exception hit type.
            nvc.Add("exd", description); // Exception description.         Required.
            nvc.Add("exf", fatal ? "1" : "0"); // Exception is fatal?            Required.

            return nvc;
        }
    }
}