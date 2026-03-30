using Newtonsoft.Json;

namespace RaidOverhaul.Models
{
    internal struct LegionProgressionConfig
    {
        [JsonProperty("LegionChance")]
        public double LegionChance;
    }
}
