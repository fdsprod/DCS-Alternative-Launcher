using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{

    public abstract class OptionModel
    {
        protected OptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Params = @params;
        }

        public Dictionary<string, object> Params
        {
            get;
        } = new Dictionary<string, object>();

        public string Description
        {
            get;
        }

        public string Id
        {
            get;
        }

        public string DisplayName
        {
            get;
        }

        public ReactiveProperty<object> ValueChangeObservable
        {
            get;
        } = new ReactiveProperty<object>(mode: ReactivePropertyMode.DistinctUntilChanged);
    }
}
