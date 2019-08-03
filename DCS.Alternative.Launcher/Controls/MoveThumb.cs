using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DCS.Alternative.Launcher.Controls
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += OnDragDelta;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = this.DataContext as Control;
            var host = ItemsControl.ItemsControlFromItemContainer(designerItem);

            if (designerItem != null)
            {
                var left = Canvas.GetLeft(designerItem) + e.HorizontalChange;
                var top = Canvas.GetTop(designerItem) + e.VerticalChange;

                left = Math.Min(Math.Max(0, left), host.ActualWidth - designerItem.Width);
                top = Math.Min(Math.Max(0, top), host.ActualHeight - designerItem.Height);

                Canvas.SetLeft(designerItem, left);
                Canvas.SetTop(designerItem, top);
            }
        }
    }
}
