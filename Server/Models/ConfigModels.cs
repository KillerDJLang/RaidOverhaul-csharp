using System.Text.Json.Serialization;

namespace RaidOverhaulMain.Models;

public class ConfigFile
{
    [JsonPropertyName("EnableCustomBoss")]
    public bool EnableCustomBoss { get; set; }

    [JsonPropertyName("EnableRequisitionOffice")]
    public bool EnableRequisitionOffice { get; set; }

    [JsonPropertyName("UseLegionGlobalSpawnChance")]
    public bool UseLegionGlobalSpawnChance { get; set; }

    [JsonPropertyName("GlobalSpawnChance")]
    public double GlobalSpawnChance { get; set; }

    [JsonPropertyName("EnableCustomItems")]
    public bool EnableCustomItems { get; set; }

    [JsonPropertyName("BackupProfile")]
    public bool BackupProfile { get; set; }

    [JsonPropertyName("ReduceFoodAndHydroDegradeEnabled")]
    public bool ReduceFoodAndHydroDegradeEnabled { get; set; }

    [JsonPropertyName("EnergyDecay")]
    public float EnergyDecay { get; set; }

    [JsonPropertyName("HydroDecay")]
    public float HydroDecay { get; set; }

    [JsonPropertyName("SaveQuestItems")]
    public bool SaveQuestItems { get; set; }

    [JsonPropertyName("NoRunThrough")]
    public bool NoRunThrough { get; set; }

    [JsonPropertyName("LootableMelee")]
    public bool LootableMelee { get; set; }

    [JsonPropertyName("LootableArmbands")]
    public bool LootableArmbands { get; set; }

    [JsonPropertyName("EnableExtendedRaids")]
    public bool EnableExtendedRaids { get; set; }

    [JsonPropertyName("TimeLimit")]
    public int TimeLimit { get; set; }

    [JsonPropertyName("HolsterAnything")]
    public bool HolsterAnything { get; set; }

    [JsonPropertyName("LowerExamineTime")]
    public bool LowerExamineTime { get; set; }

    [JsonPropertyName("SpecialSlotChanges")]
    public bool SpecialSlotChanges { get; set; }

    [JsonPropertyName("ChangeBackpackSizes")]
    public bool ChangeBackpackSizes { get; set; }

    [JsonPropertyName("ModifyEnemyBotHealth")]
    public bool ModifyEnemyBotHealth { get; set; }

    [JsonPropertyName("ChangeAirdropValuesEnabled")]
    public bool ChangeAirdropValuesEnabled { get; set; }

    [JsonPropertyName("Customs")]
    public int Customs { get; set; }

    [JsonPropertyName("Woods")]
    public int Woods { get; set; }

    [JsonPropertyName("Lighthouse")]
    public int Lighthouse { get; set; }

    [JsonPropertyName("Interchange")]
    public int Interchange { get; set; }

    [JsonPropertyName("Shoreline")]
    public int Shoreline { get; set; }

    [JsonPropertyName("Reserve")]
    public int Reserve { get; set; }

    [JsonPropertyName("Streets")]
    public int Streets { get; set; }

    [JsonPropertyName("GroundZero")]
    public int GroundZero { get; set; }

    [JsonPropertyName("PocketChangesEnabled")]
    public bool PocketChangesEnabled { get; set; }

    [JsonPropertyName("Pocket1Vertical")]
    public int Pocket1Vertical { get; set; }

    [JsonPropertyName("Pocket1Horizontal")]
    public int Pocket1Horizontal { get; set; }

    [JsonPropertyName("Pocket2Vertical")]
    public int Pocket2Vertical { get; set; }

    [JsonPropertyName("Pocket2Horizontal")]
    public int Pocket2Horizontal { get; set; }

    [JsonPropertyName("Pocket3Vertical")]
    public int Pocket3Vertical { get; set; }

    [JsonPropertyName("Pocket3Horizontal")]
    public int Pocket3Horizontal { get; set; }

    [JsonPropertyName("Pocket4Vertical")]
    public int Pocket4Vertical { get; set; }

    [JsonPropertyName("Pocket4Horizontal")]
    public int Pocket4Horizontal { get; set; }

    [JsonPropertyName("WeightChangesEnabled")]
    public bool WeightChangesEnabled { get; set; }

    [JsonPropertyName("WeightMultiplier")]
    public double WeightMultiplier { get; set; }

    [JsonPropertyName("DisableFleaBlacklist")]
    public bool DisableFleaBlacklist { get; set; }

    [JsonPropertyName("Ll1Items")]
    public bool Ll1Items { get; set; }

    [JsonPropertyName("RemoveFirRequirementsForQuests")]
    public bool RemoveFirRequirementsForQuests { get; set; }

    [JsonPropertyName("InsuranceChangesEnabled")]
    public bool InsuranceChangesEnabled { get; set; }

    [JsonPropertyName("PraporMinReturn")]
    public int PraporMinReturn { get; set; }

    [JsonPropertyName("PraporMaxReturn")]
    public int PraporMaxReturn { get; set; }

    [JsonPropertyName("TherapistMinReturn")]
    public int TherapistMinReturn { get; set; }

    [JsonPropertyName("TherapistMaxReturn")]
    public int TherapistMaxReturn { get; set; }

    [JsonPropertyName("BasicStackTuningEnabled")]
    public bool BasicStackTuningEnabled { get; set; }

    [JsonPropertyName("StackMultiplier")]
    public int StackMultiplier { get; set; }

    [JsonPropertyName("AdvancedStackTuningEnabled")]
    public bool AdvancedStackTuningEnabled { get; set; }

    [JsonPropertyName("ShotgunStack")]
    public int ShotgunStack { get; set; }

    [JsonPropertyName("FlaresAndUbgl")]
    public int FlaresAndUbgl { get; set; }

    [JsonPropertyName("SniperStack")]
    public int SniperStack { get; set; }

    [JsonPropertyName("SmgStack")]
    public int SmgStack { get; set; }

    [JsonPropertyName("RifleStack")]
    public int RifleStack { get; set; }

    [JsonPropertyName("MoneyStackMultiplierEnabled")]
    public bool MoneyStackMultiplierEnabled { get; set; }

    [JsonPropertyName("MoneyMultiplier")]
    public int MoneyMultiplier { get; set; }

    [JsonPropertyName("LootChangesEnabled")]
    public bool LootChangesEnabled { get; set; }

    [JsonPropertyName("StaticLootMultiplier")]
    public double StaticLootMultiplier { get; set; }

    [JsonPropertyName("LooseLootMultiplier")]
    public double LooseLootMultiplier { get; set; }

    [JsonPropertyName("MarkedRoomLootMultiplier")]
    public double MarkedRoomLootMultiplier { get; set; }

    [JsonPropertyName("WeatherChangesEnabled")]
    public bool WeatherChangesEnabled { get; set; }

    [JsonPropertyName("AllSeasons")]
    public bool AllSeasons { get; set; }

    [JsonPropertyName("NoWinter")]
    public bool NoWinter { get; set; }

    [JsonPropertyName("SeasonalProgression")]
    public bool SeasonalProgression { get; set; }

    [JsonPropertyName("WinterWonderland")]
    public bool WinterWonderland { get; set; }
}

public class SeasonalProgression
{
    [JsonPropertyName("seasonsProgression")]
    public int SeasonsProgression { get; set; }
}

public class LegionProgression
{
    [JsonPropertyName("LegionChance")]
    public double LegionChance { get; set; }
}

public class DebugFile
{
    [JsonPropertyName("debugMode")]
    public bool DebugMode { get; set; }

    [JsonPropertyName("dumpData")]
    public bool DumpData { get; set; }
}

public class EventsConfigFile
{
    [JsonPropertyName("SwitchToggle")]
    public int SwitchToggle { get; set; }

    [JsonPropertyName("DoorUnlock")]
    public int DoorUnlock { get; set; }

    [JsonPropertyName("KeycardUnlock")]
    public int KeycardUnlock { get; set; }

    [JsonPropertyName("DoorEventRangeMinimum")]
    public float DoorEventRangeMinimum { get; set; }

    [JsonPropertyName("DoorEventRangeMaximum")]
    public float DoorEventRangeMaximum { get; set; }

    [JsonPropertyName("DamageEvent")]
    public int DamageEvent { get; set; }

    [JsonPropertyName("AirdropEvent")]
    public int AirdropEvent { get; set; }

    [JsonPropertyName("BlackoutEvent")]
    public int BlackoutEvent { get; set; }

    [JsonPropertyName("JokeEvent")]
    public int JokeEvent { get; set; }

    [JsonPropertyName("HealEvent")]
    public int HealEvent { get; set; }

    [JsonPropertyName("ArmorEvent")]
    public int ArmorEvent { get; set; }

    [JsonPropertyName("SkillEvent")]
    public int SkillEvent { get; set; }

    [JsonPropertyName("MetabolismEvent")]
    public int MetabolismEvent { get; set; }

    [JsonPropertyName("MalfunctionEvent")]
    public int MalfunctionEvent { get; set; }

    [JsonPropertyName("TraderEvent")]
    public int TraderEvent { get; set; }

    [JsonPropertyName("BerserkEvent")]
    public int BerserkEvent { get; set; }

    [JsonPropertyName("WeightEvent")]
    public int WeightEvent { get; set; }

    [JsonPropertyName("MaxLLEvent")]
    public int MaxLLEvent { get; set; }

    [JsonPropertyName("LockdownEvent")]
    public int LockdownEvent { get; set; }

    [JsonPropertyName("ArtilleryEvent")]
    public int ArtilleryEvent { get; set; }

    [JsonPropertyName("InvasionEvent")]
    public int InvasionEvent { get; set; }

    [JsonPropertyName("RandomEventRangeMinimum")]
    public float RandomEventRangeMinimum { get; set; }

    [JsonPropertyName("RandomEventRangeMaximum")]
    public float RandomEventRangeMaximum { get; set; }
}
