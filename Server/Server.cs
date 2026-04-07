using System.Reflection;
using RaidOverhaulMain.Controllers;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using RaidOverhaulMain.Routers;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

[assembly: AssemblyTitle("Raid Overhaul Server")]
[assembly: AssemblyDescription("A large overhaul for raids including events, dead body clean up, and much more. Server component.")]
[assembly: AssemblyCopyright("Copyright © 2025 nameless")]
[assembly: AssemblyFileVersion("3.0.2")]

namespace RaidOverhaulMain;

public sealed record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "nameless.raidoverhaul.server";
    public override string Name { get; init; } = "Raid Overhaul Server";
    public override string Author { get; init; } = "nameless";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("3.0.2");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } =
        new()
        {
            { "com.morebotsapi.tacticaltoaster", new SemanticVersioning.Range(">=1.0.0") },
            { "com.wtt.commonlib", new SemanticVersioning.Range(">=2.0.15") },
        };
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; } = true;
    public override string License { get; init; } = "CC BY-NC-ND 4.0";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 10)]
public sealed class ROMain(
    WTTServerCommonLib.WTTServerCommonLib wttCommon,
    ISptLogger<ROMain> logger,
    ROStaticRouter roStaticRouter,
    ROCustomItems roCustomItems,
    RODbEdits roDbEdits,
    ROTrader roTrader,
    ROHelpers helpers
) : IOnLoad
{
    public async Task OnLoad()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var devFilesPath = Path.Combine("db", "devFiles");
        var config = helpers.LoadConfig<ConfigFile>(assembly, "config", "config.json");
        var debugConfig = helpers.LoadConfig<DebugFile>(assembly, devFilesPath, "debugOptions.json");
        var eventsConfig = helpers.LoadConfig<EventsConfigFile>(assembly, "config", "eventWeightings.json");
        var seasonsConfig = helpers.LoadConfig<SeasonalProgression>(assembly, devFilesPath, "seasonsProgressionFile.json");
        var ammoListFile = helpers.LoadConfig<AmmoStackList>(assembly, devFilesPath, "ammoStackList.json");
        var legionConfig = helpers.LoadConfig<LegionProgression>(assembly, "config", "legionProgressionFile.json");

        if (debugConfig.DebugMode && debugConfig.DumpData)
        {
            helpers.DumpDataMaps(assembly);
        }
        roTrader.PassTraderConfigs(config, debugConfig);
        roDbEdits.PassDbConfigs(config, ammoListFile);
        roCustomItems.PassCustomItemConfigs(config);
        roStaticRouter.PassRouterConfigs(config, seasonsConfig, debugConfig, eventsConfig, legionConfig);

        await roCustomItems.BuildCustomItems();
        roTrader.BuildTrader();
        roDbEdits.BuildDbEdits();

        if (config.EnableCustomBoss)
        {
            await wttCommon.CustomLocaleService.CreateCustomLocales(assembly, Path.Combine("db", "locales", "bossEnabled"));
        }

        if (!config.EnableCustomBoss)
        {
            await wttCommon.CustomLocaleService.CreateCustomLocales(assembly, Path.Combine("db", "locales", "bossDisabled"));
        }

        ROLogger.Log(logger, "Raid Overhaul Finished Loaded", LogTextColor.Magenta);

        await Task.CompletedTask;
    }

    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 99001)]
    public sealed class ROBotSetup(
        ISptLogger<ROMain> logger,
        MoreBotsServer.MoreBotsAPI moreBotsLib,
        MoreBotsServer.Services.MoreBotsCustomBotTypeService customBotTypeService,
        MoreBotsServer.Services.FactionService factionService,
        WTTServerCommonLib.WTTServerCommonLib commonLib,
        IReadOnlyList<SptMod> modList,
        ROBossHelper roBossHelper,
        ROHelpers helpers
    ) : IOnLoad
    {
        public async Task OnLoad()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var devFilesPath = Path.Combine("db", "devFiles");
            var config = helpers.LoadConfig<ConfigFile>(assembly, "config", "config.json");
            var debugConfig = helpers.LoadConfig<DebugFile>(assembly, devFilesPath, "debugOptions.json");
            var legionConfig = helpers.LoadConfig<LegionProgression>(assembly, "config", "legionProgressionFile.json");
            var botLoadouts = Path.Combine("db", "bots", "botLoadouts");
            var typeList = new List<string> { "bosslegion", "legionnaire" };
            var typeDictionary = new Dictionary<int, string>() { { 199, "bosslegion" }, { 200, "legionnaire" } };

            if (config.EnableCustomItems)
            {
                await moreBotsLib.LoadBotsShared(assembly, "legion", new List<string> { "bosslegion" });
                await moreBotsLib.LoadBotsShared(assembly, "legionnaire", new List<string> { "legionnaire" });
                await commonLib.CustomBotLoadoutService.CreateCustomBotLoadouts(assembly, Path.Combine(botLoadouts, "customItems"));
            }
            else if (!config.EnableCustomItems)
            {
                await moreBotsLib.LoadBotsShared(assembly, "legionNoCustomItems", new List<string> { "bosslegion" });
                await moreBotsLib.LoadBotsShared(assembly, "legionnaireNoCustomItems", new List<string> { "legionnaire" });
                await commonLib.CustomBotLoadoutService.CreateCustomBotLoadouts(assembly, Path.Combine(botLoadouts, "customItemsDisabled"));
            }

            customBotTypeService.AddCustomWildSpawnTypeNames(typeDictionary);

            factionService.AddEnemyByFaction(typeList, "savage");
            factionService.AddEnemyByFaction(typeList, "usec");
            factionService.AddEnemyByFaction(typeList, "bear");
            factionService.AddEnemyByFaction(typeList, "infected");
            factionService.AddEnemyByFaction("savage", "legion");
            factionService.AddEnemyByFaction("usec", "legion");
            factionService.AddEnemyByFaction("bear", "legion");
            factionService.AddEnemyByFaction("infected", "legion");
            factionService.AddRevengeByFaction(typeList, "legion");

            if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.ruafcomehome.tacticaltoaster"))
            {
                factionService.AddEnemyByFaction(typeList, "ruaf");
            }

            if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.untargh.tacticaltoaster"))
            {
                factionService.AddEnemyByFaction(typeList, "untar");
            }

            if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.blackdiv.tacticaltoaster"))
            {
                factionService.AddEnemyByFaction(typeList, "blackdiv");
            }

            if (config.EnableCustomBoss)
            {
                roBossHelper.SetBossSpawns(config, legionConfig, debugConfig);
            }

            ROLogger.Log(logger, "Custom bots finished loading", LogTextColor.Magenta);

            await Task.CompletedTask;
        }
    }

    [Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 11)]
    public sealed class ROLegionFaction(MoreBotsServer.Services.FactionService factionService) : IOnLoad
    {
        public async Task OnLoad()
        {
            factionService.Factions.Add("legion", new Faction() { Name = "legion", BotTypes = { (WildSpawnType)199, (WildSpawnType)200 } });

            await Task.CompletedTask;
        }
    }
}
