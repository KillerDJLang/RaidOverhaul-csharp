using System.Reflection;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using Path = System.IO.Path;

namespace RaidOverhaulMain.Controllers;

[Injectable(InjectionType.Singleton)]
public class ROTrader(
    ISptLogger<ROTrader> logger,
    ConfigServer configServer,
    ModHelper helper,
    ImageRouter imageRouter,
    ROTraderHelper traderHelper,
    ROAssortHelper assortHelper,
    ROQuestHelper questHelper,
    ROHelpers helpers
)
{
    private readonly TraderConfig _traderConfig = configServer.GetConfig<TraderConfig>();
    private static ConfigFile? _config;
    private static DebugFile? _debugConfig;

    public void PassTraderConfigs(ConfigFile config, DebugFile debugConfig)
    {
        _config = config;
        _debugConfig = debugConfig;
    }

    public void BuildTrader()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var pathToMod = helper.GetAbsolutePathToModFolder(assembly);
        var databasePath = Path.Combine(pathToMod, "db");
        var questPath = Path.Combine(databasePath, "questFiles", "bossEnabled");
        var questPathNoBoss = Path.Combine(databasePath, "questFiles", "bossDisabled");
        var traderImagePath = Path.Combine(databasePath, "res", "Reqs.jpg");
        var traderBase = helper.GetJsonDataFromFile<TraderBase>(databasePath, "baseBossEnabled.json");
        var traderBaseNoBoss = helper.GetJsonDataFromFile<TraderBase>(databasePath, "baseBossDisabled.json");

        if (_config.EnableRequisitionOffice)
        {
            if (_config.EnableCustomBoss)
            {
                imageRouter.AddRoute(traderBase?.Avatar.Replace(".jpg", ""), traderImagePath);
                traderHelper.SetTraderUpdateTime(_traderConfig, traderBase, 3600, 7200);
                traderHelper.AddTraderWithEmptyAssortToDb(traderBase);
                traderHelper.AddTraderToLocales(
                    traderBase,
                    "Requisitions Office",
                    "A collection of Ex-PMC's and rogue Scavs who formed a group to aid others in Tarkov. They routinely scour the battlefield for any leftover supplies and aren't afraid to fight their old comrades for it. They may not be the most trustworthy but they do have some much needed provisions in stock."
                );
                if (_config.EnableCustomItems)
                {
                    assortHelper.AddCustomItemsToTraderShop(helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps), _debugConfig);
                }
                assortHelper.GenerateTraderAssorts(helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps), _debugConfig);
                questHelper.CreateCustomQuests(assembly, questPath);
            }
            else
            {
                imageRouter.AddRoute(traderBaseNoBoss?.Avatar.Replace(".jpg", ""), traderImagePath);
                traderHelper.SetTraderUpdateTime(_traderConfig, traderBase, 3600, 7200);
                traderHelper.AddTraderWithEmptyAssortToDb(traderBaseNoBoss);
                traderHelper.AddTraderToLocales(
                    traderBaseNoBoss,
                    "Requisitions Office",
                    "A collection of Ex-PMC's and rogue Scavs who formed a group to aid others in Tarkov. They routinely scour the battlefield for any leftover supplies and aren't afraid to fight their old comrades for it. They may not be the most trustworthy but they do have some much needed provisions in stock."
                );
                if (_config.EnableCustomItems)
                {
                    assortHelper.AddCustomItemsToTraderShop(helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps), _debugConfig);
                }
                assortHelper.GenerateTraderAssorts(helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps), _debugConfig);
                questHelper.CreateCustomQuests(assembly, questPathNoBoss);
            }
            ROLogger.Log(logger, "Requisition Shop finished loading", LogTextColor.Magenta);
        }
        if (_config.EnableCustomItems && !_config.EnableRequisitionOffice)
        {
            assortHelper.AddCustomItemsToTraderShop(helpers.FetchIdFromMap("Peacekeeper", ClassMaps.TraderMaps), _debugConfig);
            ROLogger.Log(logger, "Added custom items to Peacekeeper", LogTextColor.Magenta);
        }

        return;
    }
}
