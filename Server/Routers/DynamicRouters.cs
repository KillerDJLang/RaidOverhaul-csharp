using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Callbacks;
using SPTarkov.Server.Core.DI;
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
    ROHelpers helpers
)
    : DynamicRouter(
        jsonUtil,
        [
            new RouteAction(
                "/client/trading/api/getTraderAssort/66f4db5ca4958508883d700c",
                async (url, info, sessionId, _) =>
                {
                    var traderId = helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps);
                    var trader = databaseService.GetTrader(traderId);
                    var currentResupply = trader.Base.NextResupply ?? 0;

                    if (_lastKnownResupply == 0)
                    {
                        _lastKnownResupply = currentResupply;
                    }

                    if (currentResupply != _lastKnownResupply)
                    {
                        RandomizeStock(trader.Assort.Items, randomUtil, itemHelper);
                        _lastKnownResupply = currentResupply;
                    }

                    return await traderCallbacks.GetAssort(url, info as EmptyRequestData, sessionId);
                }
            ),
        ]
    )
{
    private static long _lastKnownResupply;

    private static void RandomizeStock(List<Item> assortItems, RandomUtil _randomUtil, ItemHelper _itemHelper)
    {
        foreach (var item in assortItems)
        {
            if (item.ParentId != "hideout")
            {
                continue;
            }

            if (
                _itemHelper.IsOfBaseclass(item.Template, BaseClasses.AMMO) || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.AMMO_BOX)
            )
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(50, 300);
            }
            else if (_itemHelper.IsOfBaseclass(item.Template, BaseClasses.ARMOR_PLATE))
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 10);
            }
            else if (
                _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MEDS)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MED_KIT)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MEDICAL_SUPPLIES)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.MEDICAL)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.STIMULATOR)
            )
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 10);
            }
            else if (
                _itemHelper.IsOfBaseclass(item.Template, BaseClasses.EQUIPMENT)
                && !_itemHelper.IsOfBaseclass(item.Template, BaseClasses.ARMORED_EQUIPMENT)
            )
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 10);
            }
            else if (_itemHelper.IsOfBaseclass(item.Template, BaseClasses.MOD))
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 10);
            }
            else if (_itemHelper.IsOfBaseclass(item.Template, BaseClasses.WEAPON))
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 3);
            }
            else if (
                _itemHelper.IsOfBaseclass(item.Template, BaseClasses.BUILDING_MATERIAL)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.BARTER_ITEM)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.FOOD)
                || _itemHelper.IsOfBaseclass(item.Template, BaseClasses.DRINK)
            )
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 10);
            }
            else
            {
                item.Upd.StackObjectsCount = _randomUtil.RandInt(0, 1);
            }
        }
    }
}
