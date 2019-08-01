using System.Windows;
using System.Windows.Interactivity;

namespace DCS.Alternative.Launcher.Interaction.Behaviors
{
    public class DialogResultBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool?), typeof(DialogResultBehavior), new PropertyMetadata(null, OnValuePropertyChanged));
        
        public bool? Value
        {
            get { return (bool?) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as DialogResultBehavior;

            behavior?.OnValueChanged((bool?) e.OldValue, (bool?) e.NewValue);
        }

        private void OnValueChanged(bool? oldValue, bool? newValue)
        {
            AssociatedObject.DialogResult = newValue;
        }
    }
}