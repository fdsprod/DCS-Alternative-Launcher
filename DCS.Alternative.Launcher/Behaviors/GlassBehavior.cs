using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;

namespace DCS.Alternative.Launcher.Behaviors
{
    public class GlassBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.Register("Visual", typeof(Visual), typeof(GlassBehavior), new FrameworkPropertyMetadata(null, OnVisualChanged));

        public static readonly DependencyProperty EffectProperty =
            DependencyProperty.Register("Effect", typeof(Effect), typeof(GlassBehavior), new FrameworkPropertyMetadata(null, OnEffectChanged));

        private readonly VisualBrush _directVisualBrush = new VisualBrush();
        private readonly Rectangle _surrogateVisual = new Rectangle();
        private readonly VisualBrush _surrogateVisualBrush = new VisualBrush();
        private FrameworkElement _attachedObject;

        public GlassBehavior()
        {
            RenderOptions.SetEdgeMode(_directVisualBrush, EdgeMode.Aliased);
            RenderOptions.SetCachingHint(_directVisualBrush, CachingHint.Cache);

            RenderOptions.SetEdgeMode(_surrogateVisualBrush, EdgeMode.Aliased);
            RenderOptions.SetCachingHint(_surrogateVisualBrush, CachingHint.Cache);

            _directVisualBrush.Stretch = Stretch.None;

            _surrogateVisualBrush.ViewboxUnits = BrushMappingMode.Absolute;
            _surrogateVisualBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            _surrogateVisualBrush.Viewport = new Rect(0, 0, 1, 1);
        }

        public Visual Visual
        {
            get { return (Visual) GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        public Effect Effect
        {
            get { return (Effect) GetValue(EffectProperty); }
            set { SetValue(EffectProperty, value); }
        }

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GlassBehavior) d).OnVisualChanged(e);
        }

        protected virtual void OnVisualChanged(DependencyPropertyChangedEventArgs e)
        {
            SetupVisual();
        }

        private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GlassBehavior) d).OnEffectChanged(e);
        }

        protected virtual void OnEffectChanged(DependencyPropertyChangedEventArgs e)
        {
            SetEffect();
        }

        private void SetEffect()
        {
            if (_surrogateVisual == null)
                return;

            _surrogateVisual.Effect = Effect;
        }

        private void SetupVisual()
        {
            var element = Visual as FrameworkElement;
            if (element == null || _attachedObject == null)
                return;

            SetEffect();

            _surrogateVisualBrush.Visual = element;
            _surrogateVisual.Fill = _surrogateVisualBrush;

            _directVisualBrush.Visual = _surrogateVisual;
            EnsureBrushSyncWithVisual();
        }

        private void EnsureBrushSyncWithVisual()
        {
            if (_attachedObject == null || Visual == null)
                return;

            _surrogateVisual.Width = _attachedObject.ActualWidth;
            _surrogateVisual.Height = _attachedObject.ActualHeight;

            var trans = _attachedObject.TransformToVisual(Visual);
            var pos = trans.Transform(new Point(0, 0));
            var viewbox = new Rect
            {
                X = pos.X,
                Y = pos.Y,
                Width = _attachedObject.ActualWidth,
                Height = _attachedObject.ActualHeight
            };
            _surrogateVisualBrush.Viewbox = viewbox;
        }

        protected override void OnAttached()
        {
            if (_attachedObject != null)
                _attachedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;

            _attachedObject = AssociatedObject;

            var info = FindFillProperty(_attachedObject);
            if (info != null)
                info.SetValue(_attachedObject, _directVisualBrush, null);

            _attachedObject.LayoutUpdated += AssociatedObject_LayoutUpdated;
            SetupVisual();
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (_attachedObject != null)
                _attachedObject.LayoutUpdated -= AssociatedObject_LayoutUpdated;

            base.OnDetaching();
        }

        private static PropertyInfo FindFillProperty(DependencyObject obj)
        {
            var t = obj.GetType();
            var info = t.GetProperty("Background") ?? t.GetProperty("Fill");
            return info;
        }

        private void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
        {
            EnsureBrushSyncWithVisual();
        }
    }
}