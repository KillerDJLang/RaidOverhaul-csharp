using System.Reflection;
using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Callbacks;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Generators;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace RaidOverhaulMain.Routers;

[Injectable]
public class ROTraderDynamicRouter(
    JsonUtil jsonUtil,
    RandomUtil randomUtil,
    ItemHelper itemHelper,
    TraderCallbacks traderCallbacks,
    DatabaseService databaseService,
    RagfairOfferGenerator ragfairOfferGenerator,
    ROHelpers helpers
)
    : DynamicRouter(
        jsonUtil,
        [
            new RouteAction(
                "/client/trading/api/getTraderAssort/66f4db5ca4958508883d700c",
                async (url, info, sessionId, _) =>
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var config = helpers.LoadConfig<ConfigFile>(assembly, "config", "config.json");
                    var trader = databaseService.GetTrader(helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps));
                    var traderAssortItems = trader.Assort.Items;

                    if (config.Ll1Items)
                    {
                        RandomizeStock(traderAssortItems, randomUtil, itemHelper);
                    }

                    ragfairOfferGenerator.GenerateFleaOffersForTrader(helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps));

                    return await traderCallbacks.GetAssort(url, info as EmptyRequestData, sessionId);
                }
            ),
        ]
    )
{
    private static void RandomizeStock(List<Item> assortItems, RandomUtil _randomUtil, ItemHelper _itemHelper)
    {
        foreach (var item in assortItems)
        {
            if (item.ParentId == "hideout")
            {
                if (
                    _itemHelper.IsOfBaseclass(item.Template, BaseClasses.AMMO)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.AMMO_BOX)
                )
                {
                    var newStockCount = _randomUtil.RandInt(50, 300);
                    item.Upd.StackObjectsCount = newStockCount;
                }
                if (_itemHelper.IsOfBaseclass(item.Template, BaseClasses.ARMOR_PLATE))
                {
                    var newStockCount = _randomUtil.RandInt(0, 10);
                    item.Upd.StackObjectsCount = newStockCount;
                }
                if (
                    _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MEDS)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MED_KIT)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MEDICAL_SUPPLIES)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MEDICAL)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.STIMULATOR)
                )
                {
                    var newStockCount = _randomUtil.RandInt(0, 10);
                    item.Upd.StackObjectsCount = newStockCount;
                }
                if (
                    _itemHelper.IsOfBaseclass(item.Template, BaseClasses.EQUIPMENT)
                    && !_itemHelper.IsOfBaseclass(item.Template, BaseClasses.ARMORED_EQUIPMENT)
                )
                {
                    var newStockCount = _randomUtil.RandInt(0, 10);
                    item.Upd.StackObjectsCount = newStockCount;
                }
                if (_itemHelper.IsOfBaseclass(item.Template, BaseClasses.MOD))
                {
                    var newStockCount = _randomUtil.RandInt(0, 10);
                    item.Upd.StackObjectsCount = newStockCount;
                }
                if (_itemHelper.IsOfBaseclass(item.Template, BaseClasses.WEAPON))
                {
                    var newStockCount = _randomUtil.RandInt(0, 3);
                    item.Upd.StackObjectsCount = newStockCount;
                }

                if (
                    _itemHelper.IsOfBaseclass(item.Template, BaseClasses.BUILDING_MATERIAL)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.BARTER_ITEM)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.FOOD)
                    || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.DRINK)
                )
                {
                    var newStockCount = _randomUtil.RandInt(0, 10);
                    item.Upd.StackObjectsCount = newStockCount;
                }
                else
                {
                    var newStockCount = _randomUtil.RandInt(0, 1);
                    item.Upd.StackObjectsCount = newStockCount;
                }
            }
        }
    }
}
