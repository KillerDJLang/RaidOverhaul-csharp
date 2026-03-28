using System.Reflection;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using Path = System.IO.Path;

namespace RaidOverhaulMain.Helpers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ROBossHelper(ISptLogger<ROBossHelper> logger, ModHelper modHelper, DatabaseService databaseService)
{
    public void SetBossSpawns(Assembly assembly, string dataPath, string dataFileName, DebugFile debugConfig, double? legionSpawnChance)
    {
        var pathToMod = modHelper.GetAbsolutePathToModFolder(assembly);
        var finalPath = Path.Combine(pathToMod, dataPath);
        var spawnConfig = modHelper.GetJsonDataFromFile<BotSpawnData>(finalPath, dataFileName);
        var locations = databaseService.GetLocations();

        if (spawnConfig is null)
        {
            return;
        }

        foreach (var (bot, data) in spawnConfig.SpawnData)
        {
            foreach (var (map, mapData) in data)
            {
                var finalMap = databaseService.GetLocations().GetMappedKey(map);
                var bossSpawns = locations.GetDictionary()[finalMap].Base.BossLocationSpawn;

                bossSpawns.RemoveAll(x => x.BossName.Contains(bot));

                var bossInfo = new BossLocationSpawn
                {
                    BossChance = legionSpawnChance,
                    BossDifficulty = mapData.BossDifficulty,
                    BossEscortAmount = mapData.BossEscortAmount,
                    BossEscortDifficulty = mapData.BossEscortDifficulty,
                    BossEscortType = mapData.BossEscortType,
                    BossName = mapData.BossName,
                    IsBossPlayer = false,
                    BossZone = mapData.BossZone,
                    ForceSpawn = mapData.ForceSpawn,
                    IgnoreMaxBots = mapData.IgnoreMaxBots,
                    IsRandomTimeSpawn = mapData.IsRandomTimeSpawn,
                    SpawnMode = mapData.SpawnMode,
                    Supports = mapData.Supports,
                    Time = -1,
                    TriggerId = mapData.TriggerId,
                    TriggerName = mapData.TriggerName,
                };

                bossSpawns.Add(bossInfo);

                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, $"Added {bot} to {map}.", LogTextColor.Magenta);
                }
            }
        }
    }
}
