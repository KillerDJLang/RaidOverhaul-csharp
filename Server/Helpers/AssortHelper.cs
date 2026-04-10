using System.Reflection;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;
using Path = System.IO.Path;

namespace RaidOverhaulMain.Helpers;

[Injectable(InjectionType.Singleton)]
public class ROAssortHelper(
    ISptLogger<ROAssortHelper> logger,
    DatabaseService databaseService,
    LocaleService localeService,
    HandbookHelper handbookHelper,
    ItemHelper itemHelper,
    PresetHelper presetHelper,
    RandomUtil randomUtil,
    ItemFilterService itemFilterService,
    SeasonalEventService seasonalEventService,
    ConfigServer configServer,
    ROHelpers helpers,
    ROFluentTraderAssortHelper fluentAssortHelper,
    ICloner cloner
)
{
    protected readonly TraderConfig TraderConfig = configServer.GetConfig<TraderConfig>();

    public void GenerateTraderAssorts(string traderId, DebugFile debugConfig)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var blockedSeasonalItems = seasonalEventService.GetInactiveSeasonalEventItems();
        var baseTraderAssort = databaseService.GetTrader(traderId)?.Assort;
        var defaultPresets = presetHelper.GetDefaultPresets().Values;
        var locales = localeService.GetLocaleDb();
        var items = databaseService.GetItems();
        var devFilesPath = Path.Combine("db", "devFiles");
        var shopInfoFile = helpers.LoadConfig<ShopInfoFile>(assembly, devFilesPath, "shopInfo.json");

        foreach (var (itemId, rootItemDb) in items)
        {
            if (!string.Equals(rootItemDb.Type, "Item", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (itemFilterService.IsItemBlacklisted(itemId) || shopInfoFile.ShopBlacklist.Contains(itemId))
            {
                continue;
            }

            if (itemFilterService.IsItemRewardBlacklisted(itemId))
            {
                continue;
            }

            if (!itemHelper.IsValidItem(itemId))
            {
                continue;
            }

            if (itemHelper.IsOfBaseclass(itemId, BaseClasses.BUILT_IN_INSERTS))
            {
                continue;
            }

            if (
                itemHelper.IsOfBaseclass(itemId, BaseClasses.VEST)
                && (rootItemDb.Properties?.Slots is not null && rootItemDb.Properties.Slots.Any())
            )
            {
                continue;
            }

            if (itemHelper.IsOfBaseclass(itemId, BaseClasses.AMMO) || itemHelper.IsOfBaseclass(itemId, BaseClasses.AMMO_BOX))
            {
                if (randomUtil.GetChance100(9))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 50, 300);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (itemHelper.IsOfBaseclass(itemId, BaseClasses.ARMOR_PLATE))
            {
                if (randomUtil.GetChance100(13))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 10);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (
                itemHelper.IsOfBaseclass(itemId, BaseClasses.MEDS)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.MED_KIT)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.MEDICAL_SUPPLIES)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.MEDICAL)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.STIMULATOR)
            )
            {
                if (randomUtil.GetChance100(20))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 10);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (itemHelper.IsOfBaseclass(itemId, BaseClasses.EQUIPMENT) && !itemHelper.IsOfBaseclass(itemId, BaseClasses.ARMORED_EQUIPMENT))
            {
                if (randomUtil.GetChance100(10))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 10);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (itemHelper.IsOfBaseclass(itemId, BaseClasses.MOD))
            {
                if (randomUtil.GetChance100(5))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 10);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (itemHelper.IsOfBaseclass(itemId, BaseClasses.WEAPON))
            {
                if (randomUtil.GetChance100(7))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 3);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (
                itemHelper.IsOfBaseclass(itemId, BaseClasses.BUILDING_MATERIAL)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.BARTER_ITEM)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.FOOD)
                || itemHelper.IsOfBaseclass(itemId, BaseClasses.DRINK)
            )
            {
                if (randomUtil.GetChance100(11))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 10);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }

            if (shopInfoFile.SpecialShopItems.Contains(itemId))
            {
                if (randomUtil.GetChance100(9))
                {
                    GenerateAssortItems(itemId, rootItemDb, baseTraderAssort, 0, 1);

                    if (debugConfig.DebugMode)
                    {
                        ROLogger.Log(logger, $"Finished adding item {locales[itemId + " Name"]} to the Req shop", LogTextColor.Cyan);
                    }
                }
            }
        }

        foreach (var defaultPreset in defaultPresets)
        {
            if (randomUtil.GetChance100(7))
            {
                var itemAndChildren = cloner.Clone(defaultPreset.Items).ReplaceIDs();
                var rootItem = itemAndChildren.FirstOrDefault(item => string.IsNullOrEmpty(item.ParentId));

                rootItem.ParentId = "hideout";
                rootItem.SlotId = "hideout";
                rootItem.Upd = new Upd { StackObjectsCount = helpers.GenRandomCount(1, 3), SptPresetId = defaultPreset.Id };
                baseTraderAssort.Items.AddRange(itemAndChildren);

                var price = handbookHelper.GetTemplatePriceForItems(itemAndChildren);
                var itemQualityModifier = itemHelper.GetItemQualityModifierForItems(itemAndChildren);
                var qualPrice = price * itemQualityModifier;
                var finalPrice = GetFinalItemCost(qualPrice);
                var moneyType = GetMoneyType(qualPrice);

                baseTraderAssort.BarterScheme[itemAndChildren.First().Id] =
                [
                    new()
                    {
                        new BarterScheme { Template = moneyType, Count = finalPrice },
                    },
                ];

                baseTraderAssort.LoyalLevelItems[itemAndChildren.First().Id] = helpers.GenRandomCount(1, 4);

                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, $"Finished adding preset {defaultPreset.Name} to the Req shop", LogTextColor.Cyan);
                }
            }
        }
    }

    public void AddCustomItemsToTraderShop(string traderId, DebugFile debugConfig)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var devFilesPath = Path.Combine("db", "devFiles");
        var baseTraderAssort = databaseService.GetTrader(traderId)?.Assort;
        var customPresetsFile = helpers.LoadConfig<Dictionary<MongoId, Preset>>(assembly, devFilesPath, "customPresets.json");
        var customPresets = customPresetsFile.Values;

        var reqCoins = helpers.FetchIdFromMap("ReqCoins", ClassMaps.CustomItemMap);
        var reqSlips = helpers.FetchIdFromMap("ReqSlips", ClassMaps.CustomItemMap);
        var specialReqs = helpers.FetchIdFromMap("SpecialSlips", ClassMaps.CustomItemMap);
        var roubles = helpers.FetchIdFromMap("MONEY_RUB", ClassMaps.AllItemList);

        fluentAssortHelper.CreateSingleItemOffer(
            "66280a30d3b6f288cb6b9653",
            randomUtil.RandInt(50, 300),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("66280a30d3b6f288cb6b9653").Price),
            reqCoins,
            traderId
        );
        fluentAssortHelper.CreateSingleItemOffer(
            "662809f445b5ff428e21ac0a",
            randomUtil.RandInt(50, 300),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("662809f445b5ff428e21ac0a").Price),
            reqCoins,
            traderId
        );
        fluentAssortHelper.CreateSingleItemOffer(
            "662808ec26a8e83120bb25fe",
            randomUtil.RandInt(50, 300),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("662808ec26a8e83120bb25fe").Price),
            reqCoins,
            traderId
        );

        fluentAssortHelper.CreateSingleItemOffer(
            "6628185208dd86f969db7e03",
            randomUtil.RandInt(50, 300),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("6628185208dd86f969db7e03").Price),
            reqCoins,
            traderId
        );
        fluentAssortHelper.CreateSingleItemOffer(
            "662818a23a552da6aef8fada",
            randomUtil.RandInt(50, 300),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("662818a23a552da6aef8fada").Price),
            reqCoins,
            traderId
        );

        fluentAssortHelper.CreateSingleItemOffer(
            "66281ab7fca966e5021f81b5",
            randomUtil.RandInt(10, 50),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("66281ab7fca966e5021f81b5").Price),
            reqCoins,
            traderId
        );
        fluentAssortHelper.CreateSingleItemOffer(
            "66281ac038f9aebf6f914138",
            randomUtil.RandInt(5, 30),
            1,
            (int)GetCoinCost((double)helpers.GetItemInHandbook("66281ac038f9aebf6f914138").Price),
            reqCoins,
            traderId
        );
        fluentAssortHelper.CreateSingleItemOffer("67c957ce411e6263333a1c38", 1, 4, 1, specialReqs, traderId);
        fluentAssortHelper.CreateSingleItemOffer("666361eff60f4ea5a464eb70", 1, 4, 3, specialReqs, traderId);
        fluentAssortHelper.CreateSingleItemOffer("664a55d84a90fc2c8a6305c9", 1, 1, 1, specialReqs, traderId);
        fluentAssortHelper.CreateSingleItemOffer("67222453e6aee984bcfcf9d1", 1, 2, 5, reqSlips, traderId);
        fluentAssortHelper.CreateSingleItemOffer("6722254fd847a7aafccfbb54", 1, 1, 10, reqSlips, traderId);
        fluentAssortHelper.CreateSingleItemOffer("6722252e82ca09a7e62c4d84", 1, 3, 20, reqSlips, traderId);
        fluentAssortHelper.CreateSingleItemOffer("67d4373526a3cfb1ff5338bb", 1, 2, 15, reqSlips, traderId);
        fluentAssortHelper.CreateSingleItemOffer("67fefea22cc2bce48d31e21f", 1, 2, 5, reqSlips, traderId);
        fluentAssortHelper.CreateSingleItemOffer("67fefe885041d8c121e93bf8", 1, 2, 20, reqSlips, traderId);
        if (randomUtil.GetChance100(10))
        {
            fluentAssortHelper.CreateSingleItemOffer("66a2fc926af26cc365283f23", 1, 1, 10, specialReqs, traderId);
        }

        if (randomUtil.GetChance100(10))
        {
            fluentAssortHelper.CreateSingleItemOffer("66a2fc9886fbd5d38c5ca2a6", 1, 1, 10, specialReqs, traderId);
        }
        fluentAssortHelper.CreateSingleItemOffer(specialReqs, helpers.GenRandomCount(100, 7000), 1, 50, reqSlips, traderId);
        fluentAssortHelper.CreateSingleItemOffer(
            reqCoins,
            helpers.GenRandomCount(100, 7000),
            1,
            helpers.GenRandomCount((int)(175 * 0.75), (int)(175 * 1.25)),
            roubles,
            traderId
        );
        var formCost = Math.Round(53999D / 175D);

        fluentAssortHelper.CreateSingleItemOffer(
            reqSlips,
            helpers.GenRandomCount(1, 20),
            1,
            helpers.GenRandomCount((int)(formCost * 0.75), (int)(formCost * 1.25)),
            reqCoins,
            traderId
        );

        foreach (var customPreset in customPresets)
        {
            if (randomUtil.GetChance100(15))
            {
                var itemAndChildren = cloner.Clone(customPreset.Items).ReplaceIDs();
                var rootItem = itemAndChildren.FirstOrDefault(item => string.IsNullOrEmpty(item.ParentId));

                rootItem.ParentId = "hideout";
                rootItem.SlotId = "hideout";
                rootItem.Upd = new Upd { StackObjectsCount = 1, SptPresetId = customPreset.Id };
                baseTraderAssort.Items.AddRange(itemAndChildren);

                var price = handbookHelper.GetTemplatePriceForItems(itemAndChildren);
                var itemQualityModifier = itemHelper.GetItemQualityModifierForItems(itemAndChildren);
                var qualPrice = price * itemQualityModifier;
                var finalPrice = GetFinalItemCost(qualPrice);
                var moneyType = GetMoneyType(qualPrice);

                baseTraderAssort.BarterScheme[itemAndChildren.First().Id] =
                [
                    new()
                    {
                        new BarterScheme { Template = moneyType, Count = finalPrice },
                    },
                ];

                baseTraderAssort.LoyalLevelItems[itemAndChildren.First().Id] = helpers.GenRandomCount(1, 4);

                if (debugConfig.DebugMode)
                {
                    ROLogger.Log(logger, $"Finished adding preset {customPreset.Name} to the Req shop", LogTextColor.Cyan);
                }
            }
        }
    }

    public void GenerateAssortItems(
        MongoId itemId,
        TemplateItem rootItemDb,
        TraderAssort baseTraderAssort,
        int minStockCount,
        int maxStockCount
    )
    {
        var itemWithChildrenToAdd = new List<Item>
        {
            new()
            {
                Id = new MongoId(),
                Template = itemId,
                ParentId = "hideout",
                SlotId = "hideout",
                Upd = new Upd { StackObjectsCount = helpers.GenRandomCount(minStockCount, maxStockCount) },
            },
        };

        if (itemHelper.IsOfBaseclass(itemId, BaseClasses.AMMO_BOX))
        {
            if (itemWithChildrenToAdd.Count == 1)
            {
                itemHelper.AddCartridgesToAmmoBox(itemWithChildrenToAdd, rootItemDb);
            }
        }
        itemWithChildrenToAdd.RemapRootItemId();

        if (itemWithChildrenToAdd.Count > 1)
        {
            itemHelper.ReparentItemAndChildren(itemWithChildrenToAdd[0], itemWithChildrenToAdd);
            itemWithChildrenToAdd[0].ParentId = "hideout";
        }

        var price = Math.Round((double)helpers.GetStackedItemPrice(itemId, itemWithChildrenToAdd));
        var finalPrice = GetFinalItemCost(price);
        var moneyType = GetMoneyType(price);
        var barterSchemeToAdd = new BarterScheme { Count = finalPrice, Template = moneyType };

        baseTraderAssort.BarterScheme[itemWithChildrenToAdd[0].Id] =
        [
            [barterSchemeToAdd],
        ];

        baseTraderAssort.Items.AddRange(itemWithChildrenToAdd);
        baseTraderAssort.LoyalLevelItems[itemWithChildrenToAdd[0].Id] = helpers.GenRandomCount(1, 4);
    }

    public double GetFinalItemCost(double cost)
    {
        var reqCost = 53999D;

        if (cost >= reqCost)
        {
            var finalCost = GetSlipCost(cost);
            return finalCost;
        }
        else
        {
            var finalCost = GetCoinCost(cost);
            return finalCost;
        }
    }

    public MongoId GetMoneyType(double cost)
    {
        var reqCost = 53999D;

        if (cost >= reqCost)
        {
            return "668b3c71042c73c6f9b00704";
        }
        else
        {
            return "66292e79a4d9da25e683ab55";
        }
    }

    public double GetCoinCost(double cost)
    {
        var finalCost = Math.Round(cost / 175D);
        if (finalCost >= 1)
        {
            return finalCost;
        }
        else
        {
            return 1;
        }
    }

    public double GetSlipCost(double cost)
    {
        var finalCost = Math.Round(cost / 53999D);
        if (finalCost >= 1)
        {
            return finalCost;
        }
        else
        {
            return 1;
        }
    }
}
