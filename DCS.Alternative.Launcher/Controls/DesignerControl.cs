using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DCS.Alternative.Launcher.Controls;

namespace DCS.Alternative.Launcher.Controls
{
    public class DesignerControl : ItemsControl
    {
        static DesignerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerControl), new FrameworkPropertyMetadata(typeof(DesignerControl)));
        }


        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DesignerItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DesignerItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
