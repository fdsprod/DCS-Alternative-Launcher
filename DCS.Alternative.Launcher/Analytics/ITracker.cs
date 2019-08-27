namespace DCS.Alternative.Launcher.Analytics
{
    public interface ITracker
    {
        void SetCustomDimension(int key, string value);

        ITrackerBatch CreateBatch();

        void SendScreenView(string screenName);

        void SendEvent(string category, string action, string label = "", int? value = null);

        ITrackerTimingEvent CreateTimingEvent(string categoryName, string action, string label = "");

        void SendTiming(string category, string action, string label = "", long milliseconds = 0);

        void SendException(string description, bool fatal);

        void SendSocial(string action, string network, string target);
    }
}