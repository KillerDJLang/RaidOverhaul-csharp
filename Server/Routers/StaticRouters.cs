using System.Reflection;
using System.Text.Json;
using RaidOverhaulMain.Callbacks;
using RaidOverhaulMain.Controllers;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Extensions;
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
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private static ConfigFile _config = null!;
    private static DebugFile _debugConfig = null!;
    private static EventsConfigFile _eventsConfig = null!;
    private static RODbEdits _dbController = null!;
    private static DatabaseService _databaseService = null!;
    private static ROHelpers _helpers = null!;
    private static ROBossHelper _bossHelper = null!;
    private static ModHelper _modHelper = null!;
    private static TraderHelper _traderHelper = null!;
    private static TransferRequestCallbacks _transferRequestCallbacks = null!;
    private static LogToServerRequestCallbacks _serverLogCallbacks = null!;
    private static ProfileHelper _profileHelper = null!;
    private static ProfileActivityService _profileActivityService = null!;
    private static ISptLogger<ROStaticRouter> _logger = null!;

    public ROStaticRouter(
        ISptLogger<ROStaticRouter> logger,
        JsonUtil jsonUtil,
        TraderHelper traderHelper,
        ProfileHelper profileHelper,
        DatabaseService databaseService,
        ModHelper modHelper,
        ROHelpers helper,
        ROBossHelper bossHelper,
        RODbEdits dbController,
        ProfileActivityService profileActivityService,
        TransferRequestCallbacks transferRequestCallbacks,
        LogToServerRequestCallbacks serverLogCallbacks
    )
        : base(jsonUtil, GetCustomRoutes())
    {
        _helpers = helper;
        _profileHelper = profileHelper;
        _bossHelper = bossHelper;
        _dbController = dbController;
        _databaseService = databaseService;
        _modHelper = modHelper;
        _traderHelper = traderHelper;
        _profileActivityService = profileActivityService;
        _transferRequestCallbacks = transferRequestCallbacks;
        _serverLogCallbacks = serverLogCallbacks;
        _logger = logger;
    }

    public void PassRouterConfigs(ConfigFile config, DebugFile debugConfig, EventsConfigFile eventsConfig)
    {
        _config = config;
        _debugConfig = debugConfig;
        _eventsConfig = eventsConfig;
    }

    private static List<RouteAction> GetCustomRoutes()
    {
        return
        [
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetEventConfig", async (_, _, _, _) => await HandleRoute(_eventsConfig)),
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetServerConfig", async (_, _, _, _) => await HandleRoute(_config)),
            new RouteAction<EmptyRequestData>("/RaidOverhaul/GetDebugConfig", async (_, _, _, _) => await HandleRoute(_debugConfig)),
            new RouteAction<EmptyRequestData>(
                "/RaidOverhaul/GetWeatherConfig",
                async (_, _, sessionId, _) => await HandleGetSeasonProgression(sessionId)
            ),
            new RouteAction<EmptyRequestData>(
                "/RaidOverhaul/GetLegionConfig",
                async (_, _, sessionId, _) => await HandleGetLegionProgression(sessionId)
            ),
            new RouteAction<LogToServerRequestData>(
                "/RaidOverhaul/LogToServer",
                async (_, info, _, _) => await _serverLogCallbacks.LogToServer(info, _logger)
            ),
            new RouteAction<TransferRequestData>(
                "/RaidOverhaul/TransferItemRequests",
                async (_, info, sessionId, _) => await _transferRequestCallbacks.ReceiveAndSendItems(info, sessionId)
            ),
            new RouteAction<GetRaidConfigurationRequestData>(
                "/client/raid/configuration",
                async (_, info, sessionId, output) => await HandleRaidConfiguration(info, sessionId, output)
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

    private static ValueTask<string> HandleRaidConfiguration(GetRaidConfigurationRequestData info, MongoId sessionId, string? output)
    {
        if (!_config.TimeChangesEnabled)
        {
            return new ValueTask<string>(output ?? string.Empty);
        }

        if (info.Location == "factory4_day" || info.Location == "factory4_night")
        {
            return new ValueTask<string>(output ?? string.Empty);
        }

        var raidData = _profileActivityService.GetProfileActivityRaidData(sessionId);
        if (raidData?.RaidConfiguration == null)
        {
            return new ValueTask<string>(output ?? string.Empty);
        }

        var localTime = DateTime.Now;
        if (info.TimeVariant == DateTimeEnum.PAST)
        {
            localTime = localTime.AddHours(12);
        }

        raidData.RaidConfiguration.IsNightRaid = localTime.Hour > 21 || localTime.Hour < 5;

        return new ValueTask<string>(output ?? string.Empty);
    }

    private static ValueTask<string> HandleStandardWeatherRoute(string? output)
    {
        if (_config.WeatherChangesEnabled)
        {
            if (_helpers.IsOnlyWeatherOption(_config.NoWinter, _config))
            {
                _dbController.WeatherChangesNoWinter();
            }

            if (_helpers.IsOnlyWeatherOption(_config.AllSeasons, _config))
            {
                _dbController.WeatherChangesAllSeasons();
            }
        }

        return new ValueTask<string>(output ?? string.Empty);
    }

    private static ValueTask<string> HandleROProgression(EndLocalRaidRequestData info, MongoId sessionId, string? output)
    {
        if (_config.WeatherChangesEnabled)
        {
            if (_helpers.IsOnlyWeatherOption(_config.SeasonalProgression, _config))
            {
                var modPath = _modHelper.GetAbsolutePathToModFolder(_assembly);
                var seasonProgressionDir = Path.Combine(modPath, "config", "SeasonProgression");
                var seasonFilePath = Path.Combine(seasonProgressionDir, $"{sessionId}.json");

                if (!Directory.Exists(seasonProgressionDir))
                {
                    Directory.CreateDirectory(seasonProgressionDir);
                }

                SeasonalProgression progression;
                if (!File.Exists(seasonFilePath))
                {
                    progression = new SeasonalProgression { SeasonsProgression = 1 };
                }
                else
                {
                    progression = _helpers.LoadConfig<SeasonalProgression>(
                        Path.Combine("config", "SeasonProgression"),
                        $"{sessionId}.json"
                    );
                }

                progression = _dbController.SeasonProgression(progression);
                _helpers.WriteConfigFile(progression, Path.Combine("config", "SeasonProgression"), $"{sessionId}.json");
            }

            if (_helpers.HasConflictingWeatherOptions(_config))
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
                var assortItems = trader?.Assort.LoyalLevelItems;
                if (assortItems != null)
                {
                    HandleAssortLlItems(assortItems);
                }
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
            var pmcProfile = _profileHelper.GetPmcProfile(sessionId);
            if (pmcProfile != null)
            {
                var questStatus = pmcProfile.GetQuestStatus("66f0eb2c12fb0ed12fbcfd46");

                if (questStatus == QuestStatusEnum.Success)
                {
                    if (_config.UseLegionGlobalSpawnChance)
                    {
                        _bossHelper.SetBossSpawns(_config.GlobalSpawnChance);
                    }
                    else
                    {
                        var legionChance = HandleLegionProgression(info, sessionId);
                        _bossHelper.SetBossSpawns(legionChance);
                    }
                }
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
        var reStatus = info.Results?.Result;

        if (reStatus == null)
        {
            return;
        }

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
        var pmcData = info.Results?.Profile;
        var victim = pmcData?.Stats?.Eft?.Victims;

        if (victim == null)
        {
            return;
        }

        foreach (var victimType in victim)
        {
            var victimRole = victimType?.Role?.ToLower();

            try
            {
                if (victimRole == null)
                {
                    continue;
                }
                if (victimRole.Contains("bosslegion"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.15);
                }
                else if (victimRole.Contains("legionnaire"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.03);
                }
                else if (victimRole.Contains("bossboar"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bossbully"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bossgluhar"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bosskilla"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bossknight"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bosskojaniy"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bosskolontay"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bosssanitar"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bosstagilla"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("bosszryachiy"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("followerbigpipe"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
                else if (victimRole.Contains("followerbirdeye"))
                {
                    _traderHelper.AddStandingToTrader(sessionId, traderRepToModify, 0.10);
                }
            }
            catch (Exception ex)
            {
                ROLogger.LogError(_logger, $"Error modifying Trader Rep on killing boss: {ex}");
            }
        }

        return;
    }

    private static ValueTask<string> HandleGetSeasonProgression(MongoId sessionId)
    {
        var modPath = _modHelper.GetAbsolutePathToModFolder(_assembly);
        var seasonFilePath = Path.Combine(modPath, "config", "SeasonProgression", $"{sessionId}.json");

        SeasonalProgression progression;
        if (!File.Exists(seasonFilePath))
        {
            progression = new SeasonalProgression { SeasonsProgression = 1 };
        }
        else
        {
            progression = _helpers.LoadConfig<SeasonalProgression>(Path.Combine("config", "SeasonProgression"), $"{sessionId}.json");
        }

        return new ValueTask<string>(JsonSerializer.Serialize(progression));
    }

    private static ValueTask<string> HandleGetLegionProgression(MongoId sessionId)
    {
        var modPath = _modHelper.GetAbsolutePathToModFolder(_assembly);
        var legionFilePath = Path.Combine(modPath, "config", "LegionProgression", $"{sessionId}.json");

        LegionProgression progression;
        if (!File.Exists(legionFilePath))
        {
            progression = new LegionProgression { LegionChance = 10 };
        }
        else
        {
            progression = _helpers.LoadConfig<LegionProgression>(Path.Combine("config", "LegionProgression"), $"{sessionId}.json");
        }

        return new ValueTask<string>(JsonSerializer.Serialize(progression));
    }

    private static double HandleLegionProgression(EndLocalRaidRequestData info, MongoId sessionId)
    {
        var modPath = _modHelper.GetAbsolutePathToModFolder(_assembly);
        var legionProgressionDir = Path.Combine(modPath, "config", "LegionProgression");
        var legionFilePath = Path.Combine(legionProgressionDir, $"{sessionId}.json");

        if (!Directory.Exists(legionProgressionDir))
        {
            Directory.CreateDirectory(legionProgressionDir);
        }

        if (!File.Exists(legionFilePath))
        {
            var defaultProgression = new LegionProgression { LegionChance = 15 };
            _helpers.WriteConfigFile(defaultProgression, Path.Combine("config", "LegionProgression"), $"{sessionId}.json");
        }

        var legionProgression = _helpers.LoadConfig<LegionProgression>(Path.Combine("config", "LegionProgression"), $"{sessionId}.json");

        var reStatus = info.Results?.Result;
        var pmcData = info.Results?.Profile;
        var victim = pmcData?.Stats?.Eft?.Victims;
        var bossLegionChance = legionProgression.LegionChance;

        if (victim == null)
        {
            return bossLegionChance;
        }

        foreach (var victimType in victim)
        {
            var victimRole = victimType.Role?.ToLower();

            if (victimRole == null)
            {
                continue;
            }
            try
            {
                if (victimRole.Contains("bosslegion"))
                {
                    bossLegionChance = 10;
                    break;
                }
            }
            catch (Exception ex)
            {
                ROLogger.LogError(_logger, $"Error processing Legion progression: {ex}");
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
        if (bossLegionChance > 100)
        {
            bossLegionChance = 100;
        }

        legionProgression.LegionChance = bossLegionChance;
        _helpers.WriteConfigFile(legionProgression, Path.Combine("config", "LegionProgression"), $"{sessionId}.json");

        return bossLegionChance;
    }
}
