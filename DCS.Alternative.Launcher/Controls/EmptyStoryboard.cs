using System.Windows.Media.Animation;

namespace DCS.Alternative.Launcher.Controls
{
    public static class EmptyStoryboard
    {
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

        internal static Storyboard Value
        {
            get;
        }
    }
}