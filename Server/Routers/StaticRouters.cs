using System.Reflection;
using System.Text.Json;
using RaidOverhaulMain.Callbacks;
using RaidOverhaulMain.Controllers;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace RaidOverhaulMain.Routers;

[Injectable]
public class ROStaticRouter : StaticRouter
{
    private static ConfigFile? _config;
    private static DebugFile? _debugConfig;
    private static EventsConfigFile? _eventsConfig;
    private static SeasonalProgression? _seasonsConfig;
    private static LegionProgression? _legionConfig;
    private static RODbEdits _dbController;
    private static DatabaseService _databaseService;
    private static ROHelpers _helpers;
    private static ROBossHelper _bossHelper;
    private static ModHelper _modHelper;
    private static TraderHelper _traderHelper;
    private static TransferRequestCallbacks _transferRequestCallbacks;
    private static LogToServerRequestCallbacks _serverLogCallbacks;
    private static ISptLogger<ROStaticRouter> _logger;

    public ROStaticRouter(
        ISptLogger<ROStaticRouter> logger,
        JsonUtil jsonUtil,
        TraderHelper traderHelper,
        DatabaseService databaseService,
        ModHelper modHelper,
        ROHelpers helper,
        ROBossHelper bossHelper,
        RODbEdits dbController,
        TransferRequestCallbacks transferRequestCallbacks,
        LogToServerRequestCallbacks serverLogCallbacks
    )
        : base(jsonUtil, GetCustomRoutes())
    {
        _helpers = helper;
        _bossHelper = bossHelper;
        _dbController = dbController;
        _databaseService = databaseService;
        _modHelper = modHelper;
        _traderHelper = traderHelper;
        _transferRequestCallbacks = transferRequestCallbacks;
        _serverLogCallbacks = serverLogCallbacks;
        _logger = logger;
    }

    public void PassRouterConfigs(
        ConfigFile config,
        SeasonalProgression seasonsConfig,
        DebugFile debugConfig,
        EventsConfigFile eventsConfig,
        LegionProgression legionConfig
    )
    {
        _config = config;
        _seasonsConfig = seasonsConfig;
        _debugConfig = debugConfig;
        _eventsConfig = eventsConfig;
        _legionConfig = legionConfig;
    }

    private static List<RouteAction> GetCustomRoutes()
    {
        return
        [
            new RouteAction<EmptyRequestData>(
                "/client/game/start",
                async (_, _, sessionId, output) => await HandleProfileRoute(sessionId, output)
            ),
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetEventConfig", async (_, _, _, _) => await HandleRoute(_eventsConfig)),
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetServerConfig", async (_, _, _, _) => await HandleRoute(_config)),
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetWeatherConfig", async (_, _, _, _) => await HandleRoute(_seasonsConfig)),
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetDebugConfig", async (_, _, _, _) => await HandleRoute(_debugConfig)),
            new RouteAction<LogToServerRequestData>(
                "/RaidOverhaul/LogToServer",
                async (_, info, _, _) => await _serverLogCallbacks.LogToServer(info, _logger)
            ),
            new RouteAction<TransferRequestData>(
                "/RaidOverhaul/TransferItemRequests",
                async (_, info, sessionId, _) => await _transferRequestCallbacks.ReceiveAndSendItems(info, sessionId)
            ),
            new RouteAction<StartLocalRaidRequestData>(
                "/client/match/local/start",
                async (_, _, _, output) => await HandleStandardWeatherRoute(output)
            ),
            new RouteAction<EndLocalRaidRequestData>(
                "/client/match/local/end",
                async (_, info, sessionId, output) => await HandleROProgression(info, sessionId, output)
            ),
        ];
    }

    private static ValueTask<string> HandleRoute<T>(T config)
    {
        return new ValueTask<string>(JsonSerializer.Serialize(config));
    }

    private static ValueTask<string> HandleProfileRoute(MongoId sessionId, string? output)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var modPath = _modHelper.GetAbsolutePathToModFolder(assembly);
        var pluginPath = Path.Combine(modPath, "../", "../", "../", "../", "BepInEx", "plugins", "Fika");

        if (_config.BackupProfile && !Directory.Exists(pluginPath))
        {
            _helpers.ProfileBackup(sessionId, assembly);
        }

        return new ValueTask<string>(output ?? string.Empty);
    }

    private static ValueTask<string> HandleStandardWeatherRoute(string? output)
    {
        if (_config.WeatherChangesEnabled)
        {
            if (_config.NoWinter && !_config.AllSeasons && !_config.SeasonalProgression && !_config.WinterWonderland)
            {
                _dbController.WeatherChangesNoWinter();
            }

            if (_config.AllSeasons && _config.NoWinter && !_config.SeasonalProgression && !_config.WinterWonderland)
            {
                _dbController.WeatherChangesAllSeasons();
            }
        }

        return new ValueTask<string>(output ?? string.Empty);
    }

    private static ValueTask<string> HandleROProgression(EndLocalRaidRequestData info, MongoId sessionId, string? output)
    {
        var assembly = Assembly.GetExecutingAssembly();

        if (_config.WeatherChangesEnabled)
        {
            if (_config.SeasonalProgression && !_config.AllSeasons && !_config.NoWinter && !_config.WinterWonderland)
            {
                _dbController.SeasonProgression(_seasonsConfig, _debugConfig, assembly, _helpers);
            }

            if (
                (_config.AllSeasons && _config.WinterWonderland)
                || (_config.NoWinter && _config.WinterWonderland)
                || (_config.SeasonalProgression && _config.WinterWonderland)
                || (_config.NoWinter && _config.SeasonalProgression)
                || (_config.NoWinter && _config.AllSeasons)
                || (_config.SeasonalProgression && _config.AllSeasons)
            )
            {
                ROLogger.Log(
                    _logger,
                    "Error modifying your weather. Make sure you only have ONE of the weather options enabled",
                    LogTextColor.Red
                );
            }
        }

        if (_config.EnableRequisitionOffice)
        {
            if (_config.Ll1Items)
            {
                var trader = _databaseService.GetTrader(_helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps));
                var assortItems = trader.Assort.LoyalLevelItems;
                HandleAssortLlItems(assortItems);
            }
            HandleREStatusRep(info, sessionId, _helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps));
            HandleBossRep(info, sessionId, _helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps));
        }
        if (!_config.EnableRequisitionOffice)
        {
            HandleREStatusRep(info, sessionId, _helpers.FetchIdFromMap("Fence", ClassMaps.TraderMaps));
            HandleBossRep(info, sessionId, _helpers.FetchIdFromMap("Fence", ClassMaps.TraderMaps));
        }
        if (_config.EnableCustomBoss)
        {
            var botFilesPath = Path.Combine("db", "botFiles");

            if (_config.UseLegionGlobalSpawnChance)
            {
                _bossHelper.SetBossSpawns(assembly, botFilesPath, "botSpawns.json", _debugConfig, _config.GlobalSpawnChance);
            }
            else
            {
                HandleLegionProgression(info);
                _bossHelper.SetBossSpawns(assembly, botFilesPath, "botSpawns.json", _debugConfig, _legionConfig.LegionChance);
            }
        }

        return new ValueTask<string>(output ?? string.Empty);
    }

    private static void HandleAssortLlItems(Dictionary<MongoId, int> assortItems)
    {
        foreach (var (item, _) in assortItems)
        {
            assortItems[item] = 1;
        }
    }

    private static void HandleREStatusRep(EndLocalRaidRequestData info, MongoId sessionId, MongoId traderRepToModify)
    {
        var reStatus = info.Results.Result;

        try
        {
            if (reStatus == ExitStatus.LEFT)
            {
                return;
            }
            else if (reStatus == ExitStatus.RUNNER)
            {
                return;
            }
            else if (reStatus == ExitStatus.MISSINGINACTION)
            {
                return;
            }
            else if (reStatus == ExitStatus.KILLED)
            {
                return;
            }
            else
            {
                _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.03);
                if (_debugConfig.DebugMode)
                {
                    ROLogger.Log(_logger, $"Raid survived. Increasing {traderRepToModify} Rep by 0.03", LogTextColor.Cyan);
                }
                return;
            }
        }
        catch (Exception ex)
        {
            ROLogger.LogError(_logger, $"Error modifying Trader Rep on Successful Raid Exfil: {ex}");
        }

        return;
    }

    private static void HandleBossRep(EndLocalRaidRequestData info, MongoId sessionId, MongoId traderRepToModify)
    {
        var pmcData = info.Results.Profile;
        var victim = pmcData.Stats.Eft.Victims;

        foreach (var victimType in victim)
        {
            var victimRole = victimType.Role.ToLower();
            try
            {
                if (victimRole.Contains("bosslegion"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bossboar"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bossbully"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bossgluhar"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bosskilla"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bossknight"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bosskojaniy"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bosskolontay"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bosssanitar"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bosstagilla"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("bosszryachiy"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("followerbigpipe"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else if (victimRole.Contains("followerbirdeye"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                    return;
                }
                else
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                ROLogger.LogError(_logger, $"Error modifying Trader Rep on killing boss: {ex}");
            }
        }

        return;
    }

    private static void HandleLegionProgression(EndLocalRaidRequestData info)
    {
        var reStatus = info.Results.Result;
        var pmcData = info.Results.Profile;
        var victim = pmcData.Stats.Eft.Victims;
        var bossLegionChance = _legionConfig.LegionChance;

        foreach (var victimType in victim)
        {
            var victimRole = victimType.Role.ToLower();
            try
            {
                if (victimRole.Contains("bosslegion"))
                {
                    bossLegionChance = 10;
                    return;
                }
                else
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                ROLogger.LogError(_logger, $"Error modifying Trader Rep on killing boss: {ex}");
            }
        }
        if (reStatus == ExitStatus.SURVIVED)
        {
            bossLegionChance += 1.5;
        }
        if (reStatus == ExitStatus.RUNNER)
        {
            bossLegionChance += 3;
        }
        if (reStatus == ExitStatus.LEFT)
        {
            bossLegionChance += 0.5;
        }
        if (reStatus == ExitStatus.KILLED)
        {
            bossLegionChance += 1;
        }
        if (reStatus == ExitStatus.MISSINGINACTION)
        {
            bossLegionChance += 1;
        }
        if (reStatus == ExitStatus.TRANSIT)
        {
            bossLegionChance += 1.5;
        }

        var assembly = Assembly.GetExecutingAssembly();
        var legionProgressionData = new LegionProgression();
        legionProgressionData.LegionChance = bossLegionChance;

        _helpers.WriteConfigFile(legionProgressionData, assembly, "config", "legionProgressionFile.json");

        return;
    }
}
