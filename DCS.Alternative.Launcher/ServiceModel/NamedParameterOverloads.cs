using System.Collections.Generic;

namespace DCS.Alternative.Launcher.ServiceModel
{
    /// <summary>
    ///     Name/Value pairs for specifying "user" parameters when resolving
    /// </summary>
    public sealed class NamedParameterOverloads : Dictionary<string, object>
    {
        public NamedParameterOverloads()
        {
        }

        public NamedParameterOverloads(IDictionary<string, object> data)
            : base(data)
        {
        }

        public static NamedParameterOverloads Default { get; } = new NamedParameterOverloads();

        public static NamedParameterOverloads FromIDictionary(IDictionary<string, object> data)
        {
            return data as NamedParameterOverloads ?? new NamedParameterOverloads(data);
        }
    }
}