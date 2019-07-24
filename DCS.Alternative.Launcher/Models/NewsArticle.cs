using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Models
{
    public class NewsArticleModel
    {
        public ReactiveProperty<string> Day { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Year { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Url { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Summary { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> ImageSource { get; } = new ReactiveProperty<string>();
    }
}