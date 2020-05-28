using System.Windows;
using System.Windows.Media;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Models
{
    public class ScreenModel
    {
        public string Id
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public double RelativeX
        {
            get;
            set;
        }

        public double RelativeY
        {
            get;
            set;
        }

        public double RelativeWidth
        {
            get;
            set;
        }

        public double RelativeHeight
        {
            get;
            set;
        }

        public ReactiveProperty<bool> IsSelected
        {
            get;
        } = new ReactiveProperty<bool>();

        public ImageSource ImageSource
        {
            get;
            set;
        }

        public Rect ScreenBounds
        {
            get;
            set;
        }

        public bool IsUIViewport
        {
            get;
            set;
        }

        public bool IsGameDisplay
        {
            get;
            set;
        }
    }
}