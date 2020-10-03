using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.DomainObjects.Dcs
{
    public class DcsPluginMission
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

        [JsonProperty("CLSID")]
        public string ClassId
        {
            get;
            set;
        }
        
        [JsonProperty("training_ids")]
        public object TrainingIds
        {
            get;
            set;
        }
    }
}