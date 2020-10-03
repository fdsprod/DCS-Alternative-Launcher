using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.DomainObjects.Dcs
{
    public class DcsPluginLogBook
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("type")]
        public string Type
        {
            get;
            set;
        }
    }
}