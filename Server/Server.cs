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
[assembly: AssemblyFileVersion("3.1.0")]

namespace RaidOverhaulMain;

public sealed record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "nameless.raidoverhaul.server";
    public override string Name { get; init; } = "Raid Overhaul Server";
    public override string Author { get; init; } = "nameless";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("3.1.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } =
        new()
        {
            { "com.morebotsapi.tacticaltoaster", new SemanticVersioning.Range(">=2.0.0") },
            { "com.wtt.commonlib", new SemanticVersioning.Range(">=2.0.20") },
        };
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; } = true;
    public override string License { get; init; } = "CC BY-NC-ND 4.0";
}

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 10)]
public sealed class ROMain(
    ROStaticRouter roStaticRouter,
    ROCustomItems roCustomItems,
    ROBossHelper roBossHelper,
    RODbEdits roDbEdits,
    ROTrader roTrader,
    ROHelpers helpers,
    WTTServerCommonLib.WTTServerCommonLib commonLib
) : IOnLoad
{
    internal static ConfigFile Config { get; private set; } = null!;
    internal static DebugFile DebugConfig { get; private set; } = null!;

    public async Task OnLoad()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var devFilesPath = Path.Combine("db", "devFiles");
        var hideoutCraftsPath = Path.Combine("db", "itemGen", "hideoutCrafts");

        Config = helpers.LoadConfig<ConfigFile>("config", "config.json");
        DebugConfig = helpers.LoadConfig<DebugFile>(devFilesPath, "debugOptions.json");
        var eventsConfig = helpers.LoadConfig<EventsConfigFile>("config", "eventWeightings.json");

        if (DebugConfig.DebugMode && DebugConfig.DumpData)
        {
            helpers.DumpDataMaps();
        }

        roTrader.PassTraderConfigs(Config, DebugConfig);
        roDbEdits.PassDbConfigs(Config, DebugConfig);
        roCustomItems.PassCustomItemConfigs(Config);
        roStaticRouter.PassRouterConfigs(Config, DebugConfig, eventsConfig);
        roBossHelper.PassBossConfig(DebugConfig);

        await roCustomItems.BuildCustomItems();
        roTrader.BuildTrader();
        roDbEdits.BuildDbEdits();
        await commonLib.CustomHideoutRecipeService.CreateHideoutRecipes(assembly, hideoutCraftsPath);
        await commonLib.CustomBuffService.CreateCustomBuffs(assembly, Path.Combine("db", "itemGen", "customBuffs"));

        await Task.CompletedTask;
    }
}

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = MoreBotsServer.MoreBotsLoadOrder.LoadFactions)]
public sealed class ROFactions(MoreBotsServer.Services.FactionService factionService) : IOnLoad
{
    public async Task OnLoad()
    {
        factionService.Factions.Add("wolves", new Faction() { Name = "wolves", BotTypes = { (WildSpawnType)201, (WildSpawnType)202 } });

        if (ROMain.Config.EnableCustomBoss)
        {
            factionService.Factions.Add("legion", new Faction() { Name = "legion", BotTypes = { (WildSpawnType)199, (WildSpawnType)200 } });
        }

        await Task.CompletedTask;
    }
}

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = MoreBotsServer.MoreBotsLoadOrder.LoadBots)]
public sealed class ROBotLoader(
    ISptLogger<ROBotLoader> logger,
    MoreBotsServer.MoreBotsAPI moreBotsLib,
    MoreBotsServer.Services.FactionService factionService,
    MoreBotsServer.Services.MoreBotsCustomBotTypeService customBotTypeService,
    WTTServerCommonLib.WTTServerCommonLib commonLib,
    IReadOnlyList<SptMod> modList,
    ROBossHelper roBossHelper
) : IOnLoad
{
    public async Task OnLoad()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var botLoadouts = Path.Combine("db", "bots", "botLoadouts");
        var legionTypeList = new List<string> { "bosslegion", "legionnaire" };
        var wolvesTypeList = new List<string> { "wolfsupport", "wolflead" };
        var legionTypeDictionary = new Dictionary<int, string>() { { 199, "bosslegion" }, { 200, "legionnaire" } };
        var wolvesTypeDictionary = new Dictionary<int, string>() { { 201, "wolflead" }, { 202, "wolfsupport" } };

        await moreBotsLib.LoadBotsShared(assembly, "wolflead", new List<string> { "wolflead" });
        await moreBotsLib.LoadBotsShared(assembly, "wolfsupport", new List<string> { "wolfsupport" });
        await commonLib.CustomBotLoadoutService.CreateCustomBotLoadouts(assembly, Path.Combine(botLoadouts, "wolves"));

        customBotTypeService.AddCustomWildSpawnTypeNames(wolvesTypeDictionary);

        factionService.AddEnemyByFaction(wolvesTypeList, "savage");
        factionService.AddEnemyByFaction(wolvesTypeList, "usec");
        factionService.AddEnemyByFaction(wolvesTypeList, "bear");
        factionService.AddEnemyByFaction(wolvesTypeList, "infected");
        factionService.AddRevengeByFaction(wolvesTypeList, "wolves");

        if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.ruafcomehome.tacticaltoaster"))
        {
            factionService.AddEnemyByFaction(wolvesTypeList, "ruaf");
        }

        if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.untargh.tacticaltoaster"))
        {
            factionService.AddEnemyByFaction(wolvesTypeList, "untar");
        }

        if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.blackdiv.tacticaltoaster"))
        {
            factionService.AddEnemyByFaction(wolvesTypeList, "blackdiv");
        }

        if (ROMain.Config.EnableCustomBoss)
        {
            await commonLib.CustomAchievementService.CreateCustomAchievements(assembly, Path.Join("db", "achievements"));
            await commonLib.CustomLocaleService.CreateCustomLocales(assembly, Path.Combine("db", "locales", "bossEnabled"));

            if (ROMain.Config.EnableCustomItems)
            {
                await moreBotsLib.LoadBotsShared(assembly, "legion", new List<string> { "bosslegion" });
                await moreBotsLib.LoadBotsShared(assembly, "legionnaire", new List<string> { "legionnaire" });
                await commonLib.CustomBotLoadoutService.CreateCustomBotLoadouts(assembly, Path.Combine(botLoadouts, "customItems"));
            }
            else
            {
                await moreBotsLib.LoadBotsShared(assembly, "legionNoCustomItems", new List<string> { "bosslegion" });
                await moreBotsLib.LoadBotsShared(assembly, "legionnaireNoCustomItems", new List<string> { "legionnaire" });
                await commonLib.CustomBotLoadoutService.CreateCustomBotLoadouts(assembly, Path.Combine(botLoadouts, "customItemsDisabled"));
            }

            customBotTypeService.AddCustomWildSpawnTypeNames(legionTypeDictionary);

            factionService.AddEnemyByFaction(legionTypeList, "savage");
            factionService.AddEnemyByFaction(legionTypeList, "usec");
            factionService.AddEnemyByFaction(legionTypeList, "bear");
            factionService.AddEnemyByFaction(legionTypeList, "infected");
            factionService.AddEnemyByFaction(legionTypeList, "wolves");
            factionService.AddEnemyByFaction("savage", "legion");
            factionService.AddEnemyByFaction("usec", "legion");
            factionService.AddEnemyByFaction("bear", "legion");
            factionService.AddEnemyByFaction("infected", "legion");
            factionService.AddEnemyByFaction(wolvesTypeList, "legion");
            factionService.AddRevengeByFaction(legionTypeList, "legion");

            if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.ruafcomehome.tacticaltoaster"))
            {
                factionService.AddEnemyByFaction(legionTypeList, "ruaf");
            }

            if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.untargh.tacticaltoaster"))
            {
                factionService.AddEnemyByFaction(legionTypeList, "untar");
            }

            if (modList.Any(mod => mod.ModMetadata.ModGuid == "com.blackdiv.tacticaltoaster"))
            {
                factionService.AddEnemyByFaction(legionTypeList, "blackdiv");
            }

            roBossHelper.SetBossSpawns(ROMain.Config.UseLegionGlobalSpawnChance ? ROMain.Config.GlobalSpawnChance : 5.0);
            ROLogger.Log(logger, "Custom bots finished loading", LogTextColor.Magenta);
        }
        else
        {
            await commonLib.CustomLocaleService.CreateCustomLocales(assembly, Path.Combine("db", "locales", "bossDisabled"));
        }

        ROLogger.Log(logger, "Raid Overhaul Finished Loaded", LogTextColor.Magenta);

        await Task.CompletedTask;
    }
}
