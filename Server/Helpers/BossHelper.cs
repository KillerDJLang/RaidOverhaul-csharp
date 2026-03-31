using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Json;

namespace RaidOverhaulMain.Helpers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
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

    private record InvasionDetails(string BossName, string EscortType, string EscortCount, IEnumerable<BossSupport> Supports);

    private readonly Dictionary<string, InvasionDetails> _invasionDetails = new()
    {
        ["legionInvasion"] = new(BossName: "bosslegion", EscortType: "bosslegion", EscortCount: "0", Supports: null),
        ["legionnaireInvasion"] = new(
            BossName: "legionnaire",
            EscortType: "legionnaire",
            EscortCount: SetEscortCount(2, 4, randomUtil),
            Supports: null
        ),
        ["tagInvasion"] = new(BossName: "bossTagilla", EscortType: "followerBully", EscortCount: "0", Supports: null),
        ["killInvasion"] = new(BossName: "bossKilla", EscortType: "followerTagilla", EscortCount: "0", Supports: null),
        ["gluInvasion"] = new(
            BossName: "bossGluhar",
            EscortType: "followerGluharSecurity",
            EscortCount: "2",
            Supports:
            [
                new BossSupport
                {
                    BossEscortType = "followerGluharAssault",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerGluharSecurity",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerGluharScout",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
            ]
        ),
        ["goonsInvasion"] = new(
            BossName: "bossKnight",
            EscortType: "exUsec",
            EscortCount: "2",
            Supports:
            [
                new BossSupport
                {
                    BossEscortType = "followerBigPipe",
                    BossEscortAmount = "1",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerBirdEye",
                    BossEscortAmount = "1",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerGluharScout",
                    BossEscortAmount = "0",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
            ]
        ),
        ["zryInvasion"] = new(BossName: "bossZryachiy", EscortType: "followerZryachiy", EscortCount: "2", Supports: null),
        ["sanInvasion"] = new(BossName: "bossSanitar", EscortType: "followerSanitar", EscortCount: "3", Supports: null),
        ["kolInvasion"] = new(
            BossName: "bossKolontay",
            EscortType: "followerKolontaySecurity",
            EscortCount: "2",
            Supports:
            [
                new BossSupport
                {
                    BossEscortType = "followerKolontayAssault",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerKolontaySecurity",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerGluharScout",
                    BossEscortAmount = "0",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
            ]
        ),
        ["kabInvasion"] = new(
            BossName: "bossBoar",
            EscortType: "followerBoar",
            EscortCount: "6",
            Supports:
            [
                new BossSupport
                {
                    BossEscortType = "followerBoar",
                    BossEscortAmount = "4",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerBoarClose1",
                    BossEscortAmount = "1",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerBoarClose2",
                    BossEscortAmount = "1",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
            ]
        ),
        ["reshInvasion"] = new(BossName: "bossBully", EscortType: "followerBully", EscortCount: "4", Supports: null),
        ["shturInvasion"] = new(
            BossName: "bossKojaniy",
            EscortType: "followerKojaniy",
            EscortCount: SetEscortCount(2, 3, randomUtil),
            Supports: null
        ),
        ["rogueInvasion"] = new(BossName: "exUsec", EscortType: "exUsec", EscortCount: SetEscortCount(1, 4, randomUtil), Supports: null),
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

                spawns.RemoveAll(x => x.BossName.Contains("legion"));
                spawns.RemoveAll(x => x.TriggerId.Contains("Invasion"));

                AddInvasionsToMaps(map, spawns, debugConfig);
                AddLegionSpawnsToMaps(map, zones, spawns, config, legionConfig, debugConfig);
            }
        }
        catch (Exception ex)
        {
            ROLogger.Log(logger, $"Error adjusting Legion spawns: {ex.Message}", LogTextColor.Red);
            throw;
        }
    }

    private void AddInvasionsToMaps(string map, List<BossLocationSpawn> spawns, DebugFile debugConfig)
    {
        foreach (var (invasionType, details) in _invasionDetails)
        {
            var patrol = BuildInvasion(details.EscortCount, details.BossName, details.EscortType, details.Supports, invasionType);
            spawns.Add(patrol);

            if (debugConfig.DebugMode)
            {
                ROLogger.LogInfo(logger, $"Added {invasionType} of size {details.EscortCount} to {map}.");
            }
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
            BossEscortAmount = "2",
            BossEscortDifficulty = "normal",
            BossEscortType = "legionnaire",
            BossName = "bosslegion",
            IsBossPlayer = false,
            BossZone = zones,
            ForceSpawn = true,
            IgnoreMaxBots = true,
            IsRandomTimeSpawn = false,
            SpawnMode = ["regular", "pve"],
            Supports =
            [
                new BossSupport
                {
                    BossEscortType = "legionnaire",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "exUsec",
                    BossEscortAmount = "2",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
                new BossSupport
                {
                    BossEscortType = "followerGluharScout",
                    BossEscortAmount = "0",
                    BossEscortDifficulty = new ListOrT<string>(null, "normal"),
                },
            ],
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

    private BossLocationSpawn BuildInvasion(
        string escortCount,
        string bossName,
        string escort,
        IEnumerable<BossSupport> supports,
        string invasionType
    )
    {
        var bossInfo = new BossLocationSpawn
        {
            BossChance = 100,
            BossDifficulty = "normal",
            BossEscortAmount = escortCount,
            BossEscortDifficulty = "normal",
            BossEscortType = escort,
            BossName = bossName,
            IsBossPlayer = false,
            BossZone = "",
            ForceSpawn = true,
            IgnoreMaxBots = true,
            IsRandomTimeSpawn = false,
            SpawnMode = ["regular", "pve"],
            Supports = supports,
            Time = -1,
            TriggerId = invasionType,
            TriggerName = "botEvent",
        };

        return bossInfo;
    }

    private static string SetEscortCount(int min, int max, RandomUtil randomUtil)
    {
        var escortCount = randomUtil.GetInt(min, max);
        var finalCount = escortCount - 1;

        return finalCount.ToString();
    }
}
