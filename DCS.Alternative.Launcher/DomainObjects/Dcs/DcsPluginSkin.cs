using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.DomainObjects.Dcs
{
    public class DcsPluginSkin
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("dir")]
        public string Dir
        {
            get;
            set;
        }
    }
}