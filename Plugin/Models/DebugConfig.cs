using Newtonsoft.Json;

namespace RaidOverhaul.Models
{
    internal struct DebugConfigs
    {
        [JsonProperty("isDev")]
        public bool IsDev;

        [JsonProperty("debugMode")]
        public bool DebugMode;

        [JsonProperty("dumpData")]
        public bool DumpData;
    }
}
