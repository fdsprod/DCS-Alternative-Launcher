using System.Windows.Media.Animation;

namespace DCS.Alternative.Launcher.Controls
{
    public static class EmptyStoryboard
    {
        internal static Storyboard Value
        {
            get;
        }

        static EmptyStoryboard()
        {
            Value = new Storyboard
            {
                FillBehavior = FillBehavior.Stop
            };

            if (Value.CanFreeze)
            {
                Value.Freeze();
            }
        }
    }
}