using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DCS.Alternative.Launcher.Controls
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += OnDragDelta;
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case System.Windows.VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        designerItem.Height = Math.Max(designerItem.Height -= deltaVertical, 10);
                        break;
                    case System.Windows.VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + deltaVertical);
                        designerItem.Height = Math.Max(designerItem.Height -= deltaVertical, 10);
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case System.Windows.HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                        designerItem.Width = Math.Max(designerItem.Width -= deltaHorizontal, 10);
                        break;
                    case System.Windows.HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        designerItem.Width = Math.Max(designerItem.Width -= deltaHorizontal, 10);
                        break;
                }
            }

            e.Handled = true;
        }
    }
}