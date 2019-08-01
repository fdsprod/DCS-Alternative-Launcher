using System.Windows.Media;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Dialogs
{
    public class ViewportEditorWindowViewModel
    {
        public ViewportEditorWindowViewModel()
        {
            var vp = new ViewportModel();
            vp.X.Value = 200;
            vp.Y.Value = 200;
            vp.Width.Value = 200;
            vp.Height.Value = 200;
            Viewports.Add(vp);
        }

        public ReactiveCollection<ViewportModel> Viewports
        {
            get;
        } = new ReactiveCollection<ViewportModel>();

        public ReactiveProperty<ImageSource> BackgroundImageSource
        {
            get;
        } = new ReactiveProperty<ImageSource>();
    }

    public class ViewportModel
    {
        public ReactiveProperty<double> X
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Y
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Width
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> Height
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<string> InitFile
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Name
        {
            get;
        } = new ReactiveProperty<string>();

        public ReactiveProperty<ImageSource> MockImageSource
        {
            get;
        } = new ReactiveProperty<ImageSource>();
    }
}
