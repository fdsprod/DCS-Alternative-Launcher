using System.Collections.Generic;
using Newtonsoft.Json;

namespace DCS.Alternative.Launcher.DomainObjects.Dcs
{
    public class DcsPlugin
    {
        public string SelfId
        {
            get;
            set;
        }

        [JsonProperty("infoWaitScreen")]
        public string InfoWaitScreen
        {
            get;
            set;
        }

        [JsonProperty("image")]
        public string Image
        {
            get;
            set;
        }

        [JsonProperty("installed")]
        public bool Installed
        {
            get;
            set;
        }

        [JsonProperty("dirName")]
        public string DirName
        {
            get;
            set;
        }

        [JsonProperty("displayName")]
        public string DisplayName
        {
            get;
            set;
        }


        [JsonProperty("shortName")]
        public string ShortName
        {
            get;
            set;
        }

        [JsonProperty("developerName")]
        public string DeveloperName
        {
            get;
            set;
        }

        [JsonProperty("fileMenuName")]
        public string FileMenuName
        {
            get;
            set;
        }

        [JsonProperty("update_id")]
        public string UpdateId
        {
            get;
            set;
        }

        [JsonProperty("steam_appid")]
        public int SteamAppId
        {
            get;
            set;
        }

        [JsonProperty("registryPath")]
        public string RegistryPath
        {
            get;
            set;
        }

        [JsonProperty("DRM_controller")]
        public string DrmController
        {
            get;
            set;
        }

        [JsonProperty("version")]
        public string Version
        {
            get;
            set;
        }

        [JsonProperty("state")]
        public string State
        {
            get;
            set;
        }

        [JsonProperty("info")]
        public string Info
        {
            get;
            set;
        }

        [JsonProperty("binaries")]
        public Dictionary<int, string> Binaries
        {
            get;
            set;
        }

        [JsonProperty("InputProfiles")]
        public Dictionary<string, string> InputProfiles
        {
            get;
            set;
        }

        [JsonProperty("Skins")]
        public Dictionary<int, DcsPluginSkin> Skins
        {
            get;
            set;
        }

        [JsonProperty("Missions")]
        public Dictionary<int, DcsPluginMission> Missions
        {
            get;
            set;
        }

        [JsonProperty("Options")]
        public Dictionary<int, DcsPluginOption> Options
        {
            get;
            set;
        }

        [JsonProperty("LogBook")]
        public Dictionary<int, DcsPluginLogBook> LogBook
        {
            get;
            set;
        }

        [JsonProperty("preload_resources")]
        public object PreloadResources
        {
            get;
            set;
        }

        [JsonProperty("encyclopedia_path")]
        public string EncyclopediaPath
        {
            get;
            set;
        }
    }
}
