using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace RaidOverhaulMain.Helpers;

[Injectable(InjectionType.Singleton)]
public class ROBossHelper(ISptLogger<ROBossHelper> logger, DatabaseService databaseService, RandomUtil randomUtil)
{
    private static readonly Dictionary<string, string> _mapData = new()
    {
        ["bigmap"] = "ZoneDormitory,ZoneGasStation,ZoneScavBase,ZoneBrige,ZoneCustoms,ZoneOldVill",
        ["factory4_day"] = "BotZone",
        ["factory4_night"] = "BotZone",
        ["interchange"] = "ZoneCenterBot,ZoneCenter,ZoneOLI,ZoneIDEA,ZoneGoshan",
        ["laboratory"] = "BotZoneFloor1,BotZoneFloor2,BotZoneBasement",
        ["labyrinth"] = "BotZone",
        ["lighthouse"] = "Zone_TreatmentContainers,Zone_Chalet,Zone_Blockpost,Zone_DestroyedHouse,Zone_Rocks,Zone_Village",
        ["rezervbase"] = "ZoneRailStrorage,ZonePTOR2,ZoneBarrack,ZoneSubStorage,ZonePTOR1",
        ["sandbox"] = "ZoneSandbox",
        ["sandbox_high"] = "ZoneSandbox",
        ["shoreline"] = "ZoneGreenHouses,ZonePort,ZoneSanatorium1,ZoneSanatorium2,ZoneSmuglers,ZoneMeteoStation",
        ["tarkovstreets"] = "ZoneCarShowroom,ZoneClimova,ZoneMvd,ZoneSW01,ZoneConcordia",
        ["woods"] = "ZoneWoodCutter,ZoneScavBase2,ZoneMiniHouse,ZoneBrokenVill,ZoneBigRocks",
    };

    public void SetBossSpawns(ConfigFile config, LegionProgression legionConfig, DebugFile debugConfig)
    {
        try
        {
            var locations = databaseService.GetLocations();

            foreach (var (map, zones) in _mapData)
            {
                if (!locations.GetDictionary().ContainsKey(locations.GetMappedKey(map)))
                {
                    continue;
                }

                var spawns = locations.GetDictionary()[locations.GetMappedKey(map)].Base.BossLocationSpawn;

                spawns.RemoveAll(x => x.BossName.Contains("bosslegion"));

                AddLegionSpawnsToMaps(map, zones, spawns, config, legionConfig, debugConfig);
            }
        }
        catch (Exception ex)
        {
            ROLogger.Log(logger, $"Error adjusting Legion spawns: {ex.Message}", LogTextColor.Red);
            throw;
        }
    }

    private void AddLegionSpawnsToMaps(
        string map,
        string zones,
        List<BossLocationSpawn> spawns,
        ConfigFile config,
        LegionProgression legionConfig,
        DebugFile debugConfig
    )
    {
        double spawnChance = 15;
        if (config.UseLegionGlobalSpawnChance)
        {
            spawnChance = config.GlobalSpawnChance;
        }
        else
        {
            spawnChance = legionConfig.LegionChance;
        }

        var bossInfo = new BossLocationSpawn
        {
            BossChance = spawnChance,
            BossDifficulty = "normal",
            BossEscortAmount = SetEscortCount(2, 4, randomUtil),
            BossEscortDifficulty = "normal",
            BossEscortType = "legionnaire",
            BossName = "bosslegion",
            IsBossPlayer = false,
            BossZone = zones,
            ForceSpawn = true,
            IgnoreMaxBots = true,
            IsRandomTimeSpawn = false,
            SpawnMode = ["regular", "pve"],
            Supports = null,
            Time = -1,
            TriggerId = "",
            TriggerName = "",
        };

        spawns.Add(bossInfo);

        if (debugConfig.DebugMode)
        {
            ROLogger.LogInfo(logger, $"Added Legion spawns to {map}.");
        }
    }

    private static string SetEscortCount(int min, int max, RandomUtil randomUtil)
    {
        var escortCount = randomUtil.GetInt(min, max);
        var finalCount = escortCount - 1;

        return finalCount.ToString();
    }
}
