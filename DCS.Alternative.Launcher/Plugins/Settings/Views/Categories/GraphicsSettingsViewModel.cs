using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;
using Reactive.Bindings;

namespace DCS.Alternative.Launcher.Plugins.Settings.Views.Categories
{
    public class GraphicsSettingsViewModel : SettingsCategoryViewModelBase
    {
        public GraphicsSettingsViewModel(SettingsController controller)
            : base("GRAPHICS", controller)
        {

        }

        protected override Task InitializeAsync()
        {
            var options = Controller.GetAdvancedGraphicsOptions();

            foreach (var option in options)
            {
                var type = option.Value.GetType();
                
                AdvancedOptionModel model = null;

                if (type == typeof(string))
                {
                    model = new TextAdvancedOptionModel(option);
                }
                else if (type == typeof(long))
                {
                    model = option.MinMax.Count == 0
                        ? (AdvancedOptionModel) new SwitchAdvancedOptionModel(option, true)
                        : new SliderAdvancedOptionModel(option);
                }
                else if (type == typeof(bool))
                {
                    model = new SwitchAdvancedOptionModel(option, false);
                }
                else if (type == typeof(JArray) && option.Id.EndsWith("color", StringComparison.CurrentCultureIgnoreCase))
                {
                    model = new ColorAdvancedOptionModel(option);
                }
                else
                {
                    Tracer.Warn($"Could not create advanced option model for {option.Id} {option.DisplayName} {type}");
                }

                if (model != null)
                {
                    model.ValueChangeObservable.Subscribe(value => onValueChanged(model, value));
                    Options.Add(model);
                }
            }

            return base.InitializeAsync();
        }

        private void onValueChanged(AdvancedOptionModel model, object value)
        {
            Controller.UpsertAdvancedOption(model.Id, value);
        }

        public ReactiveCollection<AdvancedOptionModel> Options
        {
            get;
        } = new ReactiveCollection<AdvancedOptionModel>();
    }

    public abstract class AdvancedOptionModel
    {
        protected AdvancedOptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
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

    public abstract class AdvancedOptionModel<T> : AdvancedOptionModel
    {
        protected AdvancedOptionModel(string id, string displayName, string description, Dictionary<string, object> @params)
            : base(id, displayName, description, @params)
        {
            Value.Subscribe(value => { ValueChangeObservable.Value = ConvertOutputValue(value); });
        }

        protected virtual object ConvertOutputValue(T value)
        {
            return value;
        }

        public ReactiveProperty<T> Value
        {
            get;
            set;
        } = new ReactiveProperty<T>(mode: ReactivePropertyMode.DistinctUntilChanged);
    }

    public class TextAdvancedOptionModel : AdvancedOptionModel<string>
    {
        public TextAdvancedOptionModel(AdvancedOption option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            Value.Value = (string)option.Value;

            MaxValue.Value = option.MinMax[0].Max;
            MinValue.Value = option.MinMax[0].Min;
        }

        public ReactiveProperty<double> MaxValue
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> MinValue
        {
            get;
        } = new ReactiveProperty<double>();
    }

    public class SliderAdvancedOptionModel : AdvancedOptionModel<double>
    {
        public SliderAdvancedOptionModel(AdvancedOption option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            Value.Value = (double)(long) option.Value;

            MaxValue.Value = option.MinMax[0].Max;
            MinValue.Value = option.MinMax[0].Min;
        }

        public ReactiveProperty<double> MinValue
        {
            get;
        } = new ReactiveProperty<double>();

        public ReactiveProperty<double> MaxValue
        {
            get;
        } = new ReactiveProperty<double>();
    }

    public class ColorAdvancedOptionModel : AdvancedOptionModel<Color>
    {
        public ColorAdvancedOptionModel(AdvancedOption option)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            var token = (JToken) option.Value;

            Value.Value = Color.FromRgb((byte)token[0], (byte)token[1], (byte)token[2]);
        }

        protected override object ConvertOutputValue(Color value)
        {
            return new int[] {(int)value.R, (int)value.G, (int)value.B};
        }
    }

    public class SwitchAdvancedOptionModel : AdvancedOptionModel<bool>
    {
        private readonly bool _isNumericOutput;

        public SwitchAdvancedOptionModel(AdvancedOption option, bool isNumericOutput)
            : base(option.Id, option.DisplayName, option.Description, option.Params)
        {
            Value.Value = isNumericOutput ? (long)option.Value == 1 : (bool) option.Value;

            _isNumericOutput = isNumericOutput;
        }

        protected override object ConvertOutputValue(bool value)
        {
            if (_isNumericOutput)
            {
                return value ? 1 : 0;
            }

            return base.ConvertOutputValue(value);
        }
    }
}