using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DCS.Alternative.Launcher.Controls
{
    public class DesignerItem : ListViewItem
    {
        static DesignerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (IsMouseOver)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        Canvas.SetLeft(this, Canvas.GetLeft(this) - 1);
                        break;
                    case Key.Right:
                        Canvas.SetLeft(this, Canvas.GetLeft(this) + 1);
                        break;
                    case Key.Up:
                        Canvas.SetTop(this, Canvas.GetTop(this) - 1);
                        break;
                    case Key.Down:
                        Canvas.SetTop(this, Canvas.GetTop(this) + 1);
                        break;
                }
            }
        }
    }
}