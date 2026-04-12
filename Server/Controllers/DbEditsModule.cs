using System.Reflection;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace RaidOverhaulMain.Controllers;

[Injectable(InjectionType.Singleton)]
public class RODbEdits(
    ISptLogger<RODbEdits> logger,
    DatabaseService databaseService,
    ConfigServer configServer,
    ROHelpers roHelpers,
    RandomUtil randomUtil
)
{
    private readonly LostOnDeathConfig _lostOnDeathConfig = configServer.GetConfig<LostOnDeathConfig>();
    private readonly LocationConfig _locationConfig = configServer.GetConfig<LocationConfig>();
    private readonly WeatherConfig _weatherConfig = configServer.GetConfig<WeatherConfig>();
    private readonly RagfairConfig _ragfairConfig = configServer.GetConfig<RagfairConfig>();
    private static ConfigFile? _config;

    public void PassDbConfigs(ConfigFile config)
    {
        _config = config;
    }

    public void BuildDbEdits()
    {
        RaidChanges();
        WeightChanges();
        ItemChanges(roHelpers);
        StackChanges(roHelpers);
        TraderTweaks(roHelpers);
        if (_config.LootChangesEnabled)
        {
            LootChanges();
        }
        if (_config.ModifyEnemyBotHealth)
        {
            ModifyEnemyHealth();
        }
        if (_config.WeatherChangesEnabled && _config.WinterWonderland)
        {
            WeatherChangesWinterWonderland(roHelpers);
        }
        ROLogger.Log(logger, "Database Edits finished loading", LogTextColor.Magenta);
    }

    private void RaidChanges()
    {
        var globals = databaseService.GetGlobals().Configuration;
        var locations = databaseService.GetLocations().GetDictionary();

        if (_config.EnableExtendedRaids)
        {
            foreach (Location location in locations.Values)
            {
                location.Base.ExitAccessTime = _config.TimeLimit;
                location.Base.EscapeTimeLimit = _config.TimeLimit;
                location.Base.EscapeTimeLimitCoop = _config.TimeLimit;
                location.Base.EscapeTimeLimitPVE = _config.TimeLimit;
            }
        }

        if (_config.ReduceFoodAndHydroDegradeEnabled)
        {
            globals.Health.Effects.Existence.EnergyDamage = _config.EnergyDecay;
            globals.Health.Effects.Existence.HydrationDamage = _config.HydroDecay;
        }

        if (_config.ChangeAirdropValuesEnabled)
        {
            foreach (Location location in locations.Values)
            {
                var airdropParams = location.Base.AirdropParameters?.FirstOrDefault();

                if (airdropParams == null)
                {
                    continue;
                }

                if (location.Base.Id == "bigmap")
                {
                    airdropParams.PlaneAirdropChance = _config.Customs;
                }
                if (location.Base.Id == "woods")
                {
                    airdropParams.PlaneAirdropChance = _config.Woods;
                }
                if (location.Base.Id == "lighthouse")
                {
                    airdropParams.PlaneAirdropChance = _config.Lighthouse;
                }
                if (location.Base.Id == "shoreline")
                {
                    airdropParams.PlaneAirdropChance = _config.Shoreline;
                }
                if (location.Base.Id == "interchange")
                {
                    airdropParams.PlaneAirdropChance = _config.Interchange;
                }
                if (location.Base.Id == "rezervbase")
                {
                    airdropParams.PlaneAirdropChance = _config.Reserve;
                }
                if (location.Base.Id == "tarkovstreets")
                {
                    airdropParams.PlaneAirdropChance = _config.Streets;
                }
                if (location.Base.Id == "sandbox")
                {
                    airdropParams.PlaneAirdropChance = _config.GroundZero;
                }
                if (location.Base.Id == "sandbox_high")
                {
                    airdropParams.PlaneAirdropChance = _config.GroundZero;
                }
            }
        }

        if (_config.SaveQuestItems)
        {
            _lostOnDeathConfig.QuestItems = false;
        }

        if (_config.NoRunThrough)
        {
            globals.Exp.MatchEnd.SurvivedExperienceRequirement = 0;
            globals.Exp.MatchEnd.SurvivedSecondsRequirement = 0;
        }
    }

    private void WeightChanges()
    {
        var globals = databaseService.GetGlobals().Configuration;

        if (_config.WeightChangesEnabled)
        {
            globals.Stamina.BaseOverweightLimits.X *= _config.WeightMultiplier;
            globals.Stamina.BaseOverweightLimits.Y *= _config.WeightMultiplier;
            globals.Stamina.WalkOverweightLimits.X *= _config.WeightMultiplier;
            globals.Stamina.WalkOverweightLimits.Y *= _config.WeightMultiplier;
            globals.Stamina.WalkSpeedOverweightLimits.X *= _config.WeightMultiplier;
            globals.Stamina.WalkSpeedOverweightLimits.Y *= _config.WeightMultiplier;
            globals.Stamina.SprintOverweightLimits.X *= _config.WeightMultiplier;
            globals.Stamina.SprintOverweightLimits.Y *= _config.WeightMultiplier;
            globals.Inertia.InertiaLimits.Y *= _config.WeightMultiplier;
        }
    }

    private void LootChanges()
    {
        var locations = databaseService.GetLocations().GetDictionary();

        foreach (var (map, _) in _locationConfig.LooseLootMultiplier)
        {
            _locationConfig.LooseLootMultiplier[map] = _config.LooseLootMultiplier;
        }
        foreach (var (map, _) in _locationConfig.StaticLootMultiplier)
        {
            _locationConfig.StaticLootMultiplier[map] = _config.StaticLootMultiplier;
        }

        foreach (var (id, location) in locations)
        {
            location.LooseLoot?.AddTransformer(lazyLoadedLooseLootData =>
            {
                ModifyMarkedRoomLoot(id, lazyLoadedLooseLootData);

                return lazyLoadedLooseLootData;
            });
        }
    }

    private void TraderTweaks(ROHelpers helpers)
    {
        var tables = databaseService.GetTables();
        var quests = tables.Templates.Quests;
        var traders = tables.Traders;

        if (_config.InsuranceChangesEnabled)
        {
            traders["54cb50c76803fa8b248b4571"].Base.Insurance.MinReturnHour = _config.PraporMinReturn;
            traders["54cb50c76803fa8b248b4571"].Base.Insurance.MaxReturnHour = _config.PraporMaxReturn;
            traders["54cb57776803fa99248b456e"].Base.Insurance.MinReturnHour = _config.TherapistMinReturn;
            traders["54cb57776803fa99248b456e"].Base.Insurance.MaxReturnHour = _config.TherapistMaxReturn;
        }

        if (_config.Ll1Items && _config.EnableRequisitionOffice)
        {
            var reqShop = helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps);

            foreach (var (item, _) in traders[reqShop].Assort.LoyalLevelItems)
            {
                traders[reqShop].Assort.LoyalLevelItems[item] = 1;
            }
        }

        if (_config.DisableFleaBlacklist)
        {
            _ragfairConfig.Dynamic.Blacklist.EnableBsgList = false;
        }

        if (_config.RemoveFirRequirementsForQuests)
        {
            foreach (var (id, _) in quests)
            {
                var quest = quests[id];
                if (quest.Conditions.AvailableForFinish != null)
                {
                    var availableForFinish = quest.Conditions.AvailableForFinish;
                    foreach (var requirement in availableForFinish)
                    {
                        if (requirement.OnlyFoundInRaid == true)
                        {
                            requirement.OnlyFoundInRaid = false;
                        }
                    }
                }
            }
        }
    }

    private void ModifyEnemyHealth()
    {
        var botTypes = databaseService.GetBots().Types;

        foreach (var (_, botType) in botTypes)
        {
            if (botType == null)
            {
                continue;
            }

            foreach (var bodyPart in botType.BotHealth.BodyParts)
            {
                SetHealth(bodyPart.Chest, 85);
                SetHealth(bodyPart.Head, 35);
                SetHealth(bodyPart.Stomach, 70);
                SetHealth(bodyPart.LeftLeg, 65);
                SetHealth(bodyPart.RightLeg, 65);
                SetHealth(bodyPart.LeftArm, 60);
                SetHealth(bodyPart.RightArm, 60);
            }
        }
    }

    private static void SetHealth(MinMax<double> botHealth, double setValue)
    {
        var newValue = Math.Min(botHealth.Min, setValue);
        botHealth.Min = newValue;
        botHealth.Max = newValue;
    }

    private void ItemChanges(ROHelpers helpers)
    {
        var tables = databaseService.GetTables();
        var presets = databaseService.GetGlobals();
        var bots = databaseService.GetBots();
        var items = tables.Templates.Items;
        var pockets = tables.Templates.Items[helpers.FetchIdFromMap("POCKETS_SPECIAL", ClassMaps.AllItemList)];
        var uhPockets = tables.Templates.Items[helpers.FetchIdFromMap("POCKETS_UNHEARD", ClassMaps.AllItemList)];

        foreach (var (id, _) in items)
        {
            var baseItem = items[id];

            if (_config.LootableMelee)
            {
                if (baseItem.Parent == BaseClasses.KNIFE)
                {
                    baseItem.Properties.Unlootable = false;
                    baseItem.Properties.UnlootableFromSide = [];
                }
            }

            if (_config.LootableArmbands)
            {
                if (baseItem.Parent == BaseClasses.ARM_BAND)
                {
                    baseItem.Properties.Unlootable = false;
                    baseItem.Properties.UnlootableFromSide = [];
                }
            }

            if (baseItem.Properties?.BlocksEarpiece == true)
            {
                baseItem.Properties.BlocksEarpiece = false;
            }

            if (baseItem.Properties?.BlocksFaceCover == true)
            {
                baseItem.Properties.BlocksFaceCover = false;
            }

            if (baseItem.Id == helpers.FetchIdFromMap("ARMOREDEQUIPMENT_TK_HEAVY_TROOPER", ClassMaps.AllItemList))
            {
                baseItem.Properties.ArmorClass = 4;
            }
        }

        if (_config.PocketChangesEnabled)
        {
            var normalGrids = pockets.Properties?.Grids?.ToList();
            var uhGrids = uhPockets.Properties?.Grids?.ToList();

            if (normalGrids != null)
            {
                normalGrids[0].Properties.CellsH = _config.Pocket1Horizontal;
                normalGrids[0].Properties.CellsV = _config.Pocket1Vertical;
                normalGrids[1].Properties.CellsH = _config.Pocket2Horizontal;
                normalGrids[1].Properties.CellsV = _config.Pocket2Vertical;
                normalGrids[2].Properties.CellsH = _config.Pocket3Horizontal;
                normalGrids[2].Properties.CellsV = _config.Pocket3Vertical;
                normalGrids[3].Properties.CellsH = _config.Pocket4Horizontal;
                normalGrids[3].Properties.CellsV = _config.Pocket4Vertical;

                pockets.Properties.Grids = normalGrids;
            }

            if (uhGrids != null)
            {
                uhGrids[0].Properties.CellsH = _config.Pocket1Horizontal;
                uhGrids[0].Properties.CellsV = _config.Pocket1Vertical;
                uhGrids[1].Properties.CellsH = _config.Pocket2Horizontal;
                uhGrids[1].Properties.CellsV = _config.Pocket2Vertical;
                uhGrids[2].Properties.CellsH = _config.Pocket3Horizontal;
                uhGrids[2].Properties.CellsV = _config.Pocket3Vertical;
                uhGrids[3].Properties.CellsH = _config.Pocket4Horizontal;
                uhGrids[3].Properties.CellsV = _config.Pocket4Vertical;

                uhPockets.Properties.Grids = uhGrids;
            }
        }

        if (_config.SpecialSlotChanges)
        {
            foreach (var slot in pockets.Properties.Slots)
            {
                slot.Properties.Filters.FirstOrDefault().Filter = new HashSet<MongoId>() { "54009119af1c881c07000029" };
            }

            foreach (var slot in uhPockets.Properties.Slots)
            {
                slot.Properties.Filters.FirstOrDefault().Filter = new HashSet<MongoId>() { "54009119af1c881c07000029" };
            }
        }

        if (_config.HolsterAnything)
        {
            var inventory = items["55d7217a4bdc2d86028b456d"].Properties?.Slots;
            var slotToPush = helpers.FetchIdFromMap("Holster", ClassMaps.SlotIds);

            if (inventory is null)
            {
                return;
            }

            foreach (var slot in inventory)
            {
                var filters = slot.Properties?.Filters?.ToList();

                if (filters is null)
                {
                    continue;
                }

                var slotId = slot.Id;

                if (slotId is null)
                {
                    continue;
                }

                if (slotId != slotToPush)
                {
                    continue;
                }

                filters.FirstOrDefault()?.Filter?.Add("5422acb9af1c889c16000029");
                slot.Properties.Filters = filters;
            }
        }

        if (_config.LowerExamineTime)
        {
            foreach (var (id, _) in items)
            {
                items[id].Properties.ExamineTime = 0.1;
            }
        }

        foreach (var (_, botType) in bots.Types)
        {
            botType?.BotInventory.Items.Pockets.TryAdd("668b3c71042c73c6f9b00704", 1);
        }

        foreach (var (_, botType) in bots.Types)
        {
            botType?.BotInventory.Items.Pockets.TryAdd("66292e79a4d9da25e683ab55", 1);
        }

        helpers.AddToCases(
            [
                "5732ee6a24597719ae0c0281",
                "544a11ac4bdc2d470e8b456a",
                "5857a8b324597729ab0a0e7d",
                "5857a8bc2459772bad15db29",
                "59db794186f77448bc595262",
                "5c093ca986f7740a1867ab12",
                "6621b12c9f46c3eb4a0c8f40",
                "6621b143edb81061ceb5d7cc",
                "6621b177ce1b117550362db5",
                "6621b1895c9cd0794d536d14",
                "6621b1986f4ebd47e39eacb5",
                "6621b1b3166c301c04facfc8",
                "666361eff60f4ea5a464eb70",
                "666362befb4578a9f2450bd8",
            ],
            "64d4b23dc1b37504b41ac2b6"
        );

        helpers.AddToCases(
            ["5783c43d2459774bbe137486", "60b0f6c058e0b0481a09ad11", "590c60fc86f77412b13fddcf", "5d235bb686f77443f4331278"],
            "59f32c3b86f77472a31742f0"
        );
        helpers.AddToCases(
            ["5783c43d2459774bbe137486", "60b0f6c058e0b0481a09ad11", "590c60fc86f77412b13fddcf", "5d235bb686f77443f4331278"],
            "59f32bb586f774757e1e8442"
        );

        if (_config.ChangeBackpackSizes)
        {
            helpers.ModifyContainerSize("5df8a4d786f77412672a1e3b", 6, 12);
            helpers.ModifyContainerSize("628bc7fb408e2b2e9c0801b1", 6, 11);
            helpers.ModifyContainerSize("5c0e774286f77468413cc5b2", 6, 10);
            helpers.ModifyContainerSize("5e4abc6786f77406812bd572", 6, 9);
            helpers.ModifyContainerSize("5e997f0b86f7741ac73993e2", 6, 6);
            helpers.ModifyContainerSize("5ab8ebf186f7742d8b372e80", 6, 9);
            helpers.ModifyContainerSize("61b9e1aaef9a1b5d6a79899a", 6, 9);
            helpers.ModifyContainerSize("59e763f286f7742ee57895da", 6, 9);
            helpers.ModifyContainerSize("639346cc1c8f182ad90c8972", 6, 8);
            helpers.ModifyContainerSize("628e1ffc83ec92260c0f437f", 6, 6);
            helpers.ModifyContainerSize("62a1b7fbc30cfa1d366af586", 6, 6);
            helpers.ModifyContainerSize("5b44c6ae86f7742d1627baea", 6, 6);
            helpers.ModifyContainerSize("545cdae64bdc2d39198b4568", 6, 6);
            helpers.ModifyContainerSize("5f5e467b0bc58666c37e7821", 6, 6);
            helpers.ModifyContainerSize("618bb76513f5097c8d5aa2d5", 6, 5);
            helpers.ModifyContainerSize("619cf0335771dd3c390269ae", 6, 5);
            helpers.ModifyContainerSize("60a272cc93ef783291411d8e", 6, 5);
            helpers.ModifyContainerSize("618cfae774bb2d036a049e7c", 6, 5);
            helpers.ModifyContainerSize("6034d103ca006d2dca39b3f0", 4, 8);
            helpers.ModifyContainerSize("6038d614d10cbf667352dd44", 4, 8);
            helpers.ModifyContainerSize("60a2828e8689911a226117f9", 6, 5);
            helpers.ModifyContainerSize("5e9dcf5986f7746c417435b3", 5, 5);
            helpers.ModifyContainerSize("56e335e4d2720b6c058b456d", 5, 5);
            helpers.ModifyContainerSize("5ca20d5986f774331e7c9602", 5, 5);
            helpers.ModifyContainerSize("544a5cde4bdc2d39388b456b", 4, 5);
            helpers.ModifyContainerSize("56e33634d2720bd8058b456b", 5, 3);
            helpers.ModifyContainerSize("5f5e45cc5021ce62144be7aa", 3, 5);
            helpers.ModifyContainerSize("56e33680d2720be2748b4576", 4, 3);
            helpers.ModifyContainerSize("5ab8ee7786f7742d8f33f0b9", 3, 4);
            helpers.ModifyContainerSize("5ab8f04f86f774585f4237d8", 3, 3);
            helpers.ModifyContainerSize("66a9f98f3bd5a41b162030f4", 6, 9);
            helpers.ModifyContainerSize("66b5f247af44ca0014063c02", 5, 5);
            helpers.ModifyContainerSize("66b5f22b78bbc0200425f904", 6, 6);
        }
    }

    private void StackChanges(ROHelpers helpers)
    {
        var tables = databaseService.GetTables();
        var items = tables.Templates.Items;

        if (_config.AdvancedStackTuningEnabled && !_config.BasicStackTuningEnabled)
        {
            foreach (var (StackType, StackList) in DevModels.AmmoStackList)
            {
                int stackSize = StackType switch
                {
                    "RifleList" => _config.RifleStack,
                    "ShotgunList" => _config.ShotgunStack,
                    "SmgList" => _config.SmgStack,
                    "SniperList" => _config.SniperStack,
                    "UbglList" => _config.FlaresAndUbgl,
                    _ => -1,
                };

                if (stackSize == -1)
                {
                    continue;
                }

                foreach (var item in StackList)
                {
                    items[item].Properties.StackMaxSize = stackSize;
                }
            }
        }

        if (_config.BasicStackTuningEnabled && !_config.AdvancedStackTuningEnabled)
        {
            foreach (var (id, _) in items)
            {
                if (
                    items[id].Parent == helpers.FetchIdFromMap("AMMO", ClassMaps.ItemBaseClasses)
                    && items[id].Properties?.StackMaxSize != null
                )
                {
                    items[id].Properties.StackMaxSize *= _config.StackMultiplier;
                }
            }
        }

        if (_config.BasicStackTuningEnabled && _config.AdvancedStackTuningEnabled)
        {
            logger.LogWithColor(
                "Error multiplying your ammo stacks. Make sure you only have ONE of the Stack Tuning options enabled",
                LogTextColor.Red
            );
        }

        if (_config.MoneyStackMultiplierEnabled)
        {
            foreach (var (id, _) in items)
            {
                if (
                    items[id].Parent == helpers.FetchIdFromMap("MONEY", ClassMaps.ItemBaseClasses)
                    && items[id].Properties?.StackMaxSize != null
                )
                {
                    items[id].Properties.StackMaxSize *= _config.MoneyMultiplier;
                }
            }
        }
    }

    private static void ModifyMarkedRoomLoot(string location, LooseLoot looseLoot)
    {
        if (looseLoot.Spawnpoints == null)
        {
            return;
        }

        foreach (var sp in looseLoot.Spawnpoints)
        {
            var spawnpoint = sp.Template?.Position;

            if (string.Equals(location.ToLower(), "bigmap"))
            {
                if (
                    spawnpoint?.X > 180
                    && spawnpoint.X < 185
                    && spawnpoint.Y > 6
                    && spawnpoint.Y < 7
                    && spawnpoint.Z > 180
                    && spawnpoint.Z < 185
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
            }
            else if (string.Equals(location.ToLower(), "rezervbase"))
            {
                if (
                    spawnpoint?.X > -125
                    && spawnpoint.X < -120
                    && spawnpoint.Y > -15
                    && spawnpoint.Y < -14
                    && spawnpoint.Z > 25
                    && spawnpoint.Z < 30
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
                if (
                    spawnpoint?.X > -155
                    && spawnpoint.X < -150
                    && spawnpoint.Y > -9
                    && spawnpoint.Y < -8
                    && spawnpoint.Z > 70
                    && spawnpoint.Z < 75
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
                if (
                    spawnpoint?.X > 190
                    && spawnpoint.X < 195
                    && spawnpoint.Y > -6
                    && spawnpoint.Y < -5
                    && spawnpoint.Z > -230
                    && spawnpoint.Z < -225
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
            }
            else if (string.Equals(location.ToLower(), "tarkovstreets"))
            {
                if (
                    spawnpoint?.X > -133
                    && spawnpoint.X < -129
                    && spawnpoint.Y > 8.5
                    && spawnpoint.Y < 11
                    && spawnpoint.Z > 265
                    && spawnpoint.Z < 275
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
                if (
                    spawnpoint?.X > 186
                    && spawnpoint.X < 191
                    && spawnpoint.Y > -0.5
                    && spawnpoint.Y < 1.5
                    && spawnpoint.Z > 224
                    && spawnpoint.Z < 229
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
            }
            else if (string.Equals(location.ToLower(), "lighthouse"))
            {
                if (
                    spawnpoint?.X > 319
                    && spawnpoint.X < 330
                    && spawnpoint.Y > 5
                    && spawnpoint.Y < 6.5
                    && spawnpoint.Z > 482
                    && spawnpoint.Z < 489
                )
                {
                    sp.Probability *= _config.MarkedRoomLootMultiplier;
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    public void WeatherChangesAllSeasons(ROHelpers helpers)
    {
        if (helpers.IsOnlyWeatherOption(_config.AllSeasons, _config))
        {
            var weatherChance = randomUtil.GetInt(1, 100);

            if (weatherChance >= 1 && weatherChance <= 20)
            {
                _weatherConfig.OverrideSeason = Season.SUMMER;
                ROLogger.Log(logger, "Summer is active.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 21 && weatherChance <= 40)
            {
                _weatherConfig.OverrideSeason = Season.AUTUMN;
                ROLogger.Log(logger, "Autumn is active.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 41 && weatherChance <= 60)
            {
                _weatherConfig.OverrideSeason = Season.WINTER;
                ROLogger.Log(logger, "Winter is coming.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 61 && weatherChance <= 80)
            {
                _weatherConfig.OverrideSeason = Season.SPRING;
                ROLogger.Log(logger, "Spring is active.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 81 && weatherChance <= 100)
            {
                _weatherConfig.OverrideSeason = Season.STORM;
                ROLogger.Log(logger, "Storm is active.", LogTextColor.Magenta);
            }
        }
        else if (helpers.HasConflictingWeatherOptions(_config))
        {
            ROLogger.Log(
                logger,
                "Error modifying your weather. Make sure you only have ONE of the weather options enabled",
                LogTextColor.Red
            );
        }
    }

    public void WeatherChangesNoWinter(ROHelpers helpers)
    {
        if (helpers.IsOnlyWeatherOption(_config.NoWinter, _config))
        {
            var weatherChance = randomUtil.GetInt(1, 100);

            if (weatherChance >= 1 && weatherChance <= 25)
            {
                _weatherConfig.OverrideSeason = Season.SUMMER;
                ROLogger.Log(logger, "Summer is active.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 26 && weatherChance <= 50)
            {
                _weatherConfig.OverrideSeason = Season.AUTUMN;
                ROLogger.Log(logger, "Autumn is active.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 51 && weatherChance <= 75)
            {
                _weatherConfig.OverrideSeason = Season.SPRING;
                ROLogger.Log(logger, "Spring is active.", LogTextColor.Magenta);

                return;
            }
            if (weatherChance >= 76 && weatherChance <= 100)
            {
                _weatherConfig.OverrideSeason = Season.STORM;
                ROLogger.Log(logger, "Storm is active.", LogTextColor.Magenta);

                return;
            }
        }

        if (helpers.HasConflictingWeatherOptions(_config))
        {
            ROLogger.Log(
                logger,
                "Error modifying your weather. Make sure you only have ONE of the weather options enabled",
                LogTextColor.Red
            );
        }
    }

    private void WeatherChangesWinterWonderland(ROHelpers helpers)
    {
        if (helpers.IsOnlyWeatherOption(_config.WinterWonderland, _config))
        {
            _weatherConfig.OverrideSeason = Season.WINTER;
            ROLogger.Log(logger, "Snow is active. It's a whole fuckin' winter wonderland out there.", LogTextColor.Magenta);

            return;
        }

        if (helpers.HasConflictingWeatherOptions(_config))
        {
            ROLogger.Log(
                logger,
                "Error modifying your weather. Make sure you only have ONE of the weather options enabled",
                LogTextColor.Red
            );
        }
    }

    public void SeasonProgression(SeasonalProgression seasonProgressionFile, DebugFile debugConfig, Assembly assembly, ROHelpers helpers)
    {
        var raidsRun = seasonProgressionFile.SeasonsProgression;

        switch (raidsRun)
        {
            //Spring
            case 1:
            case 2:
            case 3:
                raidsRun++;
                _weatherConfig.OverrideSeason = Season.SPRING;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Spring is active.", LogTextColor.Magenta);
                }
                break;
            //Storm (handled client side)
            case 4:
            case 5:
            case 6:
                raidsRun++;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Storm is active.", LogTextColor.Magenta);
                }
                break;
            //Summer
            case 7:
            case 8:
            case 9:
                raidsRun++;
                _weatherConfig.OverrideSeason = Season.SUMMER;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Summer is active.", LogTextColor.Magenta);
                }
                break;
            //Autumn
            case 10:
            case 11:
            case 12:
                raidsRun++;
                _weatherConfig.OverrideSeason = Season.AUTUMN;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Autumn is active.", LogTextColor.Magenta);
                }
                break;
            //Late Autumn
            case 13:
            case 14:
                raidsRun++;
                _weatherConfig.OverrideSeason = Season.AUTUMN_LATE;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Autumn is active.", LogTextColor.Magenta);
                }
                break;
            //Winter
            case 15:
            case 16:
            case 17:
            case 18:
                raidsRun++;
                _weatherConfig.OverrideSeason = Season.WINTER;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Winter is coming.", LogTextColor.Magenta);
                }
                break;
            //Default catch
            default:
                _weatherConfig.OverrideSeason = Season.SPRING_EARLY;
                raidsRun = 1;
                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, "Defaulting to spring.", LogTextColor.Magenta);
                }
                break;
        }
        try
        {
            seasonProgressionFile.SeasonsProgression = raidsRun;

            helpers.WriteConfigFile(seasonProgressionFile, assembly, Path.Combine("db", "devFiles"), "seasonsProgressionFile.json");
            if (debugConfig.DebugMode)
            {
                ROLogger.Log(logger, $"Seasonal progress updated to {raidsRun}", LogTextColor.Cyan);
            }
        }
        catch (Exception ex)
        {
            ROLogger.LogError(logger, "Error writing season progression file: " + ex);
        }
    }
}
