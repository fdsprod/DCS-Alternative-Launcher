using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugin.Viewports.Dialogs
{
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

        public ReactiveProperty<int> SeatIndex
        {
            get;
        } = new ReactiveProperty<int>();

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

        public ReactiveProperty<string> ImageUrl
        {
            get;
        } = new ReactiveProperty<string>();
    }
}