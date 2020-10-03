using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.DomainObjects.Dcs
{
    public class DcsPluginOption
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("nameId")]
        public string NameId
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

        [JsonProperty("CLSID")]
        public string ClassId
        {
            get;
            set;
        }

        [JsonProperty("AircraftSettingsFile")]
        public string AircraftSettingsFile
        {
            get;
            set;
        }
    }
}