using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            
            if (IsSelected)
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
