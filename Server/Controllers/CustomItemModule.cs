using System.Reflection;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using WTTServerCommonLib.Services;

namespace RaidOverhaulMain.Controllers;

[Injectable(InjectionType.Singleton)]
public class ROCustomItems(
    ISptLogger<ROCustomItems> logger,
    DatabaseService databaseService,
    ConfigServer configServer,
    WTTCustomItemServiceExtended wttItemService,
    WTTCustomRigLayoutService wttRigLayoutService,
    ROHelpers roHelpers
)
{
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
    private static ConfigFile? _config;

    public void PassCustomItemConfigs(ConfigFile config)
    {
        _config = config;
    }

    public async Task BuildCustomItems()
    {
        var assembly = Assembly.GetExecutingAssembly();
        await LoadCustomItems(assembly, roHelpers);
        wttRigLayoutService.CreateRigLayouts(assembly, "db/itemGen/customLayouts");
        ApplyFleaBlacklist();
        ApplyCasePushes();
        ROLogger.Log(logger, "Custom Items finished loading", LogTextColor.Magenta);
    }

    private async Task LoadCustomItems(Assembly assembly, ROHelpers helpers)
    {
        //var locations = databaseService.GetLocations();
        const string realismKey = "SPT-Realism";

        await wttItemService.CreateCustomItems(assembly, "db/itemGen/currency");
        await wttItemService.CreateCustomItems(assembly, "db/itemGen/constItems");
        await wttItemService.CreateCustomItems(assembly, "db/itemGen/customKeys");
        await wttItemService.CreateCustomItems(assembly, "db/itemGen/cases");

        if (_config.EnableCustomItems)
        {
            if (helpers.CheckForMod(realismKey))
            {
                await wttItemService.CreateCustomItems(assembly, "db/itemGen/ammoRealism");
                ROLogger.Log(logger, "Realism detected, modifying custom ammunition.", LogTextColor.Magenta);
            }
            else if (!helpers.CheckForMod(realismKey))
            {
                await wttItemService.CreateCustomItems(assembly, "db/itemGen/ammo");
            }
            await wttItemService.CreateCustomItems(assembly, "db/itemGen/weapons");
            await wttItemService.CreateCustomItems(assembly, "db/itemGen/gear");

            ApplyFleaBlacklistCustomWeapons();
            BuildSlots(helpers);
        }
        /*
            var labsKeys = locations.Laboratory.Base.AccessKeys?.ToList();
            labsKeys?.Add("66a2fc9886fbd5d38c5ca2a6");
            locations.Laboratory.Base.AccessKeys = labsKeys;
        */
    }

    private void ApplyFleaBlacklist()
    {
        string[] fleaBlacklistIds =
        [
            "67c957ce411e6263333a1c38", // Special Storage Crate (Requisitions)
            "6621b0dcbcfe66cdbbab48c7", // Custom Deadskul Carrying Pouch
            "666361eff60f4ea5a464eb70", // Secure Container Onyx
            "6621b12c9f46c3eb4a0c8f40", // Custom Waist Pouch
            "6621b143edb81061ceb5d7cc", // Custom Secure Container Alpha
            "6621b177ce1b117550362db5", // Custom Secure Container Beta
            "6621b1895c9cd0794d536d14", // Custom Secure Container Gamma
            "6621b1986f4ebd47e39eacb5", // Custom Secure Container Epsilon
            "6621b1b3166c301c04facfc8", // Custom Secure Container Kappa
            "67c95a09708ee99e7a575da5", // Special Request Form
            "66a2fc926af26cc365283f23", // Skeleton Key
            "66a2fc9886fbd5d38c5ca2a6", // VIP Keycard
        ];
        foreach (var id in fleaBlacklistIds)
        {
            _ragfairConfig.Dynamic.Blacklist.Custom.Add(id);
        }
    }

    private void ApplyFleaBlacklistCustomWeapons()
    {
        string[] fleaBlacklistIds =
        [
            "6628f96fd59ab54dedb55801", // The Executioner
            "6628f76df1a913e3afc16360", // The Judge
            "6628f8813e3fe94f5f035010", // The Jury
        ];
        foreach (var id in fleaBlacklistIds)
        {
            _ragfairConfig.Dynamic.Blacklist.Custom.Add(id);
        }
    }

    private void ApplyCasePushes()
    {
        var items = databaseService.GetItems();

        var casePushMap = new Dictionary<string, string[]>
        {
            // Requisition Coin → docs, sicc, wallet, wz, money
            ["66292e79a4d9da25e683ab55"] =
            [
                "5c093db286f7740a1b2617e3",
                "5d235bb686f77443f4331278",
                "5783c43d2459774bbe137486",
                "60b0f6c058e0b0481a09ad11",
                "59fb016586f7746d0d4b423a",
            ],
            // Requisition Slips → docs, sicc, wallet, wz, money
            ["668b3c71042c73c6f9b00704"] =
            [
                "5c093db286f7740a1b2617e3",
                "5d235bb686f77443f4331278",
                "5783c43d2459774bbe137486",
                "60b0f6c058e0b0481a09ad11",
                "59fb016586f7746d0d4b423a",
            ],
            // Special Request Form → docs, sicc, wallet, wz, money
            ["67c95a09708ee99e7a575da5"] =
            [
                "5c093db286f7740a1b2617e3",
                "5d235bb686f77443f4331278",
                "5783c43d2459774bbe137486",
                "60b0f6c058e0b0481a09ad11",
                "59fb016586f7746d0d4b423a",
            ],
            // Skeleton Key → docs, sicc, wallet, wz, keytool
            ["66a2fc926af26cc365283f23"] =
            [
                "5c093db286f7740a1b2617e3",
                "5d235bb686f77443f4331278",
                "5783c43d2459774bbe137486",
                "60b0f6c058e0b0481a09ad11",
                "59fafd4b86f7745ca07e1232",
            ],
            // VIP Keycard → docs, sicc, wallet, wz, keycards
            ["66a2fc9886fbd5d38c5ca2a6"] =
            [
                "5c093db286f7740a1b2617e3",
                "5d235bb686f77443f4331278",
                "5783c43d2459774bbe137486",
                "60b0f6c058e0b0481a09ad11",
                "619cbf9e0a7c3a1a2731940a",
            ],
        };

        foreach (var (itemId, caseIds) in casePushMap)
        {
            foreach (var caseId in caseIds)
            {
                items[caseId].Properties?.Grids?.FirstOrDefault()?.Properties?.Filters?.FirstOrDefault()?.Filter?.Add(itemId);
            }
        }
    }

    private void BuildSlots(ROHelpers helpers)
    {
        var tables = databaseService.GetTables();
        var items = tables.Templates.Items;
        var aug = helpers.FetchIdFromMap("Aug762", ClassMaps.CustomItemMap);
        var stm46 = helpers.FetchIdFromMap("Stm46", ClassMaps.CustomItemMap);
        var mcm4 = helpers.FetchIdFromMap("Mcm4", ClassMaps.CustomItemMap);
        var judge = helpers.FetchIdFromMap("Judge", ClassMaps.CustomItemMap);
        var jury = helpers.FetchIdFromMap("Jury", ClassMaps.CustomItemMap);
        var executioner = helpers.FetchIdFromMap("Exec", ClassMaps.CustomItemMap);
        var aug30 = helpers.FetchIdFromMap("Aug30Rd", ClassMaps.CustomItemMap);
        var aug42 = helpers.FetchIdFromMap("Aug42Rd", ClassMaps.CustomItemMap);
        var stm33 = helpers.FetchIdFromMap("Stm33Rd", ClassMaps.CustomItemMap);
        var stm50 = helpers.FetchIdFromMap("Stm50Rd", ClassMaps.CustomItemMap);
        var stmRec = helpers.FetchIdFromMap("StmRec", ClassMaps.CustomItemMap);
        var mag300 = helpers.FetchIdFromMap("Mag300", ClassMaps.CustomItemMap);
        var mag545 = helpers.FetchIdFromMap("Mag545", ClassMaps.CustomItemMap);
        var mag57 = helpers.FetchIdFromMap("Mag57", ClassMaps.CustomItemMap);
        var mag762 = helpers.FetchIdFromMap("Mag762", ClassMaps.CustomItemMap);
        var mag939 = helpers.FetchIdFromMap("Mag939", ClassMaps.CustomItemMap);
        var rec300 = helpers.FetchIdFromMap("Rec300", ClassMaps.CustomItemMap);
        var rec545 = helpers.FetchIdFromMap("Rec545", ClassMaps.CustomItemMap);
        var rec57 = helpers.FetchIdFromMap("Rec57", ClassMaps.CustomItemMap);
        var rec762 = helpers.FetchIdFromMap("Rec762", ClassMaps.CustomItemMap);
        var rec939 = helpers.FetchIdFromMap("Rec939", ClassMaps.CustomItemMap);
        var judge17 = helpers.FetchIdFromMap("Judge17Rd", ClassMaps.CustomItemMap);
        var judge33 = helpers.FetchIdFromMap("Judge33Rd", ClassMaps.CustomItemMap);
        var judge50 = helpers.FetchIdFromMap("Judge50Rd", ClassMaps.CustomItemMap);
        var judgeSlide = helpers.FetchIdFromMap("JudgeSlide", ClassMaps.CustomItemMap);
        var jury20 = helpers.FetchIdFromMap("Jury20Rd", ClassMaps.CustomItemMap);
        var jury25 = helpers.FetchIdFromMap("Jury25Rd", ClassMaps.CustomItemMap);
        var jury50 = helpers.FetchIdFromMap("Jury50Rd", ClassMaps.CustomItemMap);
        var juryRec = helpers.FetchIdFromMap("JuryRec", ClassMaps.CustomItemMap);
        var execAics = helpers.FetchIdFromMap("ExecAics", ClassMaps.CustomItemMap);
        var execPmag = helpers.FetchIdFromMap("ExecPmag", ClassMaps.CustomItemMap);
        var execWyatt = helpers.FetchIdFromMap("ExecWyatt", ClassMaps.CustomItemMap);

        items[aug].Properties.Slots.ElementAt(0).Properties.Filters.ElementAt(0).Filter = [aug30, aug42];
        items[stm46].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter = [stm33, stm50];
        items[stm46].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter = [stmRec];
        items[mcm4].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter.Add(mag300);
        items[mcm4].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter.Add(mag545);
        items[mcm4].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter.Add(mag57);
        items[mcm4].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter.Add(mag762);
        items[mcm4].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter.Add(mag939);
        items[mcm4].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter.Add(rec300);
        items[mcm4].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter.Add(rec545);
        items[mcm4].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter.Add(rec57);
        items[mcm4].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter.Add(rec762);
        items[mcm4].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter.Add(rec939);
        items[judge].Properties.Slots.ElementAt(3).Properties.Filters.ElementAt(0).Filter = [judge17, judge33, judge50];
        items[judge].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter = [judgeSlide];
        items[jury].Properties.Slots.ElementAt(1).Properties.Filters.ElementAt(0).Filter = [jury20, jury25, jury50];
        items[jury].Properties.Slots.ElementAt(2).Properties.Filters.ElementAt(0).Filter = [juryRec];
        items[executioner].Properties.Slots.ElementAt(0).Properties.Filters.ElementAt(0).Filter = [execAics, execPmag, execWyatt];
    }
}
