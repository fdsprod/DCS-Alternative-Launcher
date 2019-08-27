using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DCS.Alternative.Launcher.Diagnostics.Trace;
using DCS.Alternative.Launcher.DomainObjects;
using Newtonsoft.Json.Linq;

namespace DCS.Alternative.Launcher.Plugins.Settings.Models
{
    public static class OptionModelFactory
    {
        public static IEnumerable<OptionModelBase> CreateAll(IEnumerable<Option> options)
        {
            var models = new List<OptionModelBase>();

            foreach (var option in options)
            {
                var type = option.Value.GetType();

                OptionModelBase model = null;

                if (option.Params.ContainsKey("Items"))
                {
                    model = new ComboBoxOptionModel(option);
                }
                else if (type == typeof(string))
                {
                    model = new TextOptionModel(option);
                }
                else if (type == typeof(long) || type == typeof(double))
                {
                    model = option.MinMax.Count == 0
                        ? (OptionModelBase) new SwitchOptionModel(option, true)
                        : new SliderOptionModel(option);
                }
                else if (type == typeof(bool))
                {
                    model = new SwitchOptionModel(option, false);
                }
                else if ((type == typeof(JArray) || type == typeof(int[])) && option.Id.EndsWith("color", StringComparison.CurrentCultureIgnoreCase))
                {
                    model = new ColorOptionModel(option);
                }
                else if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    var enumerable = (IEnumerable) option.Value;
                    var count = enumerable.OfType<object>().Count();

                    switch (count)
                    {
                        case 2:
                            model = new DoubleRangeSliderOptionModel(option);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
                else
                {
                    Tracer.Warn($"Could not create advanced option model for {option.Id} {option.DisplayName} {type}");
                }

                if (model != null)
                {
                    models.Add(model);
                }
            }

            return models;
        }
    }
}