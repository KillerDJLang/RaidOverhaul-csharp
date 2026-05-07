using Newtonsoft.Json;

namespace RaidOverhaul.Models
{
    internal struct EventsConfig
    {
        [JsonProperty("SwitchToggle")]
        public int SwitchWeights { get; set; }

        [JsonProperty("DoorUnlock")]
        public int LockedDoorWeights { get; set; }

        [JsonProperty("KeycardUnlock")]
        public int KeycardWeights { get; set; }

        [JsonProperty("DoorEventRangeMinimum")]
        public float DoorEventRangeMinimumServer { get; set; }

        [JsonProperty("DoorEventRangeMaximum")]
        public float DoorEventRangeMaximumServer { get; set; }

        [JsonProperty("DamageEvent")]
        public int DamageEventWeights { get; set; }

        [JsonProperty("AirdropEvent")]
        public int AirdropEventWeights { get; set; }

        [JsonProperty("BlackoutEvent")]
        public int BlackoutEventWeights { get; set; }

        [JsonProperty("JokeEvent")]
        public int JokeEventWeights { get; set; }

        [JsonProperty("HealEvent")]
        public int HealEventWeights { get; set; }

        [JsonProperty("ArmorEvent")]
        public int ArmorEventWeights { get; set; }

        [JsonProperty("SkillEvent")]
        public int SkillEventWeights { get; set; }

        [JsonProperty("MetabolismEvent")]
        public int MetabolismEventWeights { get; set; }

        [JsonProperty("MalfunctionEvent")]
        public int MalfEventWeights { get; set; }

        [JsonProperty("TraderEvent")]
        public int TraderEventWeights { get; set; }

        [JsonProperty("BerserkEvent")]
        public int BerserkEventWeights { get; set; }

        [JsonProperty("WeightEvent")]
        public int WeightEventWeights { get; set; }

        [JsonProperty("MaxLLEvent")]
        public int MaxLLEventWeights { get; set; }

        [JsonProperty("LockdownEvent")]
        public int ExfilEventWeights { get; set; }

        [JsonProperty("ArtilleryEvent")]
        public int ArtilleryEventWeights { get; set; }

        [JsonProperty("InvasionEvent")]
        public int InvasionEventWeights { get; set; }

        [JsonProperty("HuntedEvent")]
        public int HuntedEventWeights { get; set; }

        [JsonProperty("RandomEventRangeMinimum")]
        public float RandomEventRangeMinimumServer { get; set; }

        [JsonProperty("RandomEventRangeMaximum")]
        public float RandomEventRangeMaximumServer { get; set; }
    }
}
