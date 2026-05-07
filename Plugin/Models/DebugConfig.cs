using Newtonsoft.Json;

namespace RaidOverhaul.Models
{
    internal struct DebugConfigs
    {
        [JsonProperty("isDev")]
        public bool IsDev { get; set; }

        [JsonProperty("debugMode")]
        public bool DebugMode { get; set; }

        [JsonProperty("dumpData")]
        public bool DumpData { get; set; }
    }
}
