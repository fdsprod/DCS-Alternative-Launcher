using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace DCS.Alternative.Launcher.Interaction.Behaviors
{
    public class WindowBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(WindowBehavior), new PropertyMetadata(true));

        public bool CanClose
        {
            get { return (bool) GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }
    }
}