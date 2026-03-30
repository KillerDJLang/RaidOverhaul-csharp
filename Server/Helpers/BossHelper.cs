using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace RaidOverhaulMain.Helpers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ROBossHelper(ISptLogger<ROBossHelper> logger, DatabaseService databaseService, RandomUtil randomUtil)
{
    private readonly List<string> _mapList =
    [
        "factory4_day",
        "factory4_night",
        "laboratory",
        "sandbox_high",
        "bigmap",
        "interchange",
        "labyrinth",
        "shoreline",
        "lighthouse",
        "rezervbase",
        "tarkovstreets",
    ];

    public void SetBossSpawns(DebugFile debugConfig)
    {
        try
        {
            var locations = databaseService.GetLocations();

            foreach (var map in _mapList)
            {
                if (!locations.GetDictionary().ContainsKey(locations.GetMappedKey(map)))
                {
                    continue;
                }

                var spawns = locations.GetDictionary()[locations.GetMappedKey(map)].Base.BossLocationSpawn;

                spawns.RemoveAll(x => x.BossName.Contains("legion"));

                AdjustLegionSpawns(map, spawns, debugConfig);
            }
        }
        catch (Exception ex)
        {
            ROLogger.Log(logger, $"Error adjusting Legion spawns: {ex.Message}", LogTextColor.Red);
            throw;
        }
    }

    private void AdjustLegionSpawns(string map, List<BossLocationSpawn> spawns, DebugFile debugConfig)
    {
        spawns.RemoveAll(x => x.TriggerId == "legionInvasion");
        AddLegionToMaps(map, spawns, debugConfig);
    }

    private void AddLegionToMaps(string map, List<BossLocationSpawn> spawns, DebugFile debugConfig)
    {
        var patrolSize = randomUtil.GetInt(2, 4);
        var patrol = GeneratePatrol(patrolSize, false);

        patrol.Time = -1;
        patrol.BossZone = "";
        patrol.TriggerName = "botEvent";
        patrol.TriggerId = "legionInvasion";
        patrol.ForceSpawn = true;

        spawns.Add(patrol);

        if (debugConfig.DebugMode)
        {
            ROLogger.LogInfo(logger, $"Added Legion Invasion of size {patrolSize} to {map}.");
        }
    }

    private BossLocationSpawn GeneratePatrol(int patrolSize, bool isPatrol = true)
    {
        var followers = patrolSize - 1;

        var bossInfo = new BossLocationSpawn
        {
            BossChance = 100,
            BossDifficulty = "normal",
            BossEscortAmount = followers.ToString(),
            BossEscortDifficulty = "normal",
            BossEscortType = "legionnaire",
            BossName = "bosslegion",
            IsBossPlayer = false,
            BossZone = string.Empty,
            ForceSpawn = false,
            IgnoreMaxBots = true,
            IsRandomTimeSpawn = false,
            SpawnMode = ["regular", "pve"],
            Supports = null,
            Time = -1,
            TriggerId = string.Empty,
            TriggerName = string.Empty,
        };

        return bossInfo;
    }
}
