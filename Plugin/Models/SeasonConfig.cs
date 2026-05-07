using Newtonsoft.Json;

namespace RaidOverhaul.Models
{
    internal struct SeasonalConfig
    {
        [JsonProperty("seasonsProgression")]
        public int SeasonsProgression { get; set; }
    }
}
