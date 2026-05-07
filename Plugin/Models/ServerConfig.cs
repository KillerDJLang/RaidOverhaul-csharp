using Newtonsoft.Json;

namespace RaidOverhaul.Models
{
    internal struct ServerConfigs
    {
        [JsonProperty("EnableCustomBoss")]
        public bool EnableCustomBoss { get; set; }

        [JsonProperty("EnableRequisitionOffice")]
        public bool EnableRequisitionOffice { get; set; }

        [JsonProperty("UseLegionGlobalSpawnChance")]
        public bool UseLegionGlobalSpawnChance { get; set; }

        [JsonProperty("GlobalSpawnChance")]
        public double GlobalSpawnChance { get; set; }

        [JsonProperty("EnableCustomItems")]
        public bool EnableCustomItems { get; set; }

        [JsonProperty("TimeChangesEnabled")]
        public bool TimeChangesEnabled { get; set; }

        [JsonProperty("ReduceFoodAndHydroDegradeEnabled")]
        public bool ReduceFoodAndHydroDegradeEnabled { get; set; }

        [JsonProperty("EnergyDecay")]
        public float EnergyDecay { get; set; }

        [JsonProperty("HydroDecay")]
        public float HydroDecay { get; set; }

        [JsonProperty("SaveQuestItems")]
        public bool SaveQuestItems { get; set; }

        [JsonProperty("NoRunThrough")]
        public bool NoRunThrough { get; set; }

        [JsonProperty("LootableMelee")]
        public bool LootableMelee { get; set; }

        [JsonProperty("LootableArmbands")]
        public bool LootableArmbands { get; set; }

        [JsonProperty("EnableExtendedRaids")]
        public bool EnableExtendedRaids { get; set; }

        [JsonProperty("TimeLimit")]
        public int TimeLimit { get; set; }

        [JsonProperty("HolsterAnything")]
        public bool HolsterAnything { get; set; }

        [JsonProperty("LowerExamineTime")]
        public bool LowerExamineTime { get; set; }

        [JsonProperty("SpecialSlotChanges")]
        public bool SpecialSlotChanges { get; set; }

        [JsonProperty("ChangeBackpackSizes")]
        public bool ChangeBackpackSizes { get; set; }

        [JsonProperty("ModifyEnemyBotHealth")]
        public bool ModifyEnemyBotHealth { get; set; }

        [JsonProperty("ChangeAirdropValuesEnabled")]
        public bool ChangeAirdropValuesEnabled { get; set; }

        [JsonProperty("Customs")]
        public int Customs { get; set; }

        [JsonProperty("Woods")]
        public int Woods { get; set; }

        [JsonProperty("Lighthouse")]
        public int Lighthouse { get; set; }

        [JsonProperty("Interchange")]
        public int Interchange { get; set; }

        [JsonProperty("Shoreline")]
        public int Shoreline { get; set; }

        [JsonProperty("Reserve")]
        public int Reserve { get; set; }

        [JsonProperty("Streets")]
        public int Streets { get; set; }

        [JsonProperty("GroundZero")]
        public int GroundZero { get; set; }

        [JsonProperty("PocketChangesEnabled")]
        public bool PocketChangesEnabled { get; set; }

        [JsonProperty("Pocket1Vertical")]
        public int Pocket1Vertical { get; set; }

        [JsonProperty("Pocket1Horizontal")]
        public int Pocket1Horizontal { get; set; }

        [JsonProperty("Pocket2Vertical")]
        public int Pocket2Vertical { get; set; }

        [JsonProperty("Pocket2Horizontal")]
        public int Pocket2Horizontal { get; set; }

        [JsonProperty("Pocket3Vertical")]
        public int Pocket3Vertical { get; set; }

        [JsonProperty("Pocket3Horizontal")]
        public int Pocket3Horizontal { get; set; }

        [JsonProperty("Pocket4Vertical")]
        public int Pocket4Vertical { get; set; }

        [JsonProperty("Pocket4Horizontal")]
        public int Pocket4Horizontal { get; set; }

        [JsonProperty("WeightChangesEnabled")]
        public bool WeightChangesEnabled { get; set; }

        [JsonProperty("WeightMultiplier")]
        public double WeightMultiplier { get; set; }

        [JsonProperty("DisableFleaBlacklist")]
        public bool DisableFleaBlacklist { get; set; }

        [JsonProperty("Ll1Items")]
        public bool Ll1Items { get; set; }

        [JsonProperty("RemoveFirRequirementsForQuests")]
        public bool RemoveFirRequirementsForQuests { get; set; }

        [JsonProperty("InsuranceChangesEnabled")]
        public bool InsuranceChangesEnabled { get; set; }

        [JsonProperty("PraporMinReturn")]
        public int PraporMinReturn { get; set; }

        [JsonProperty("PraporMaxReturn")]
        public int PraporMaxReturn { get; set; }

        [JsonProperty("TherapistMinReturn")]
        public int TherapistMinReturn { get; set; }

        [JsonProperty("TherapistMaxReturn")]
        public int TherapistMaxReturn { get; set; }

        [JsonProperty("BasicStackTuningEnabled")]
        public bool BasicStackTuningEnabled { get; set; }

        [JsonProperty("StackMultiplier")]
        public int StackMultiplier { get; set; }

        [JsonProperty("AdvancedStackTuningEnabled")]
        public bool AdvancedStackTuningEnabled { get; set; }

        [JsonProperty("ShotgunStack")]
        public int ShotgunStack { get; set; }

        [JsonProperty("FlaresAndUbgl")]
        public int FlaresAndUbgl { get; set; }

        [JsonProperty("SniperStack")]
        public int SniperStack { get; set; }

        [JsonProperty("SmgStack")]
        public int SmgStack { get; set; }

        [JsonProperty("RifleStack")]
        public int RifleStack { get; set; }

        [JsonProperty("MoneyStackMultiplierEnabled")]
        public bool MoneyStackMultiplierEnabled { get; set; }

        [JsonProperty("MoneyMultiplier")]
        public int MoneyMultiplier { get; set; }

        [JsonProperty("LootChangesEnabled")]
        public bool LootChangesEnabled { get; set; }

        [JsonProperty("StaticLootMultiplier")]
        public double StaticLootMultiplier { get; set; }

        [JsonProperty("LooseLootMultiplier")]
        public double LooseLootMultiplier { get; set; }

        [JsonProperty("MarkedRoomLootMultiplier")]
        public double MarkedRoomLootMultiplier { get; set; }

        [JsonProperty("WeatherChangesEnabled")]
        public bool WeatherChangesEnabled { get; set; }

        [JsonProperty("AllSeasons")]
        public bool AllSeasons { get; set; }

        [JsonProperty("NoWinter")]
        public bool NoWinter { get; set; }

        [JsonProperty("SeasonalProgression")]
        public bool SeasonalProgression { get; set; }

        [JsonProperty("WinterWonderland")]
        public bool WinterWonderland { get; set; }
    }
}
