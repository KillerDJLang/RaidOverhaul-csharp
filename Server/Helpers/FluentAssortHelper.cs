using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace RaidOverhaulMain.Helpers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ROFluentTraderAssortHelper(DatabaseService databaseService, ISptLogger<ROFluentTraderAssortHelper> logger)
{
    private readonly List<Item> _itemsToSell = [];
    private readonly Dictionary<string, List<List<BarterScheme>>> _barterScheme = new();
    private readonly Dictionary<string, int> _loyaltyLevel = new();

    public ROFluentTraderAssortHelper CreateSingleAssortItem(MongoId itemTpl, MongoId? itemId = null)
    {
        var newItemToAdd = new Item
        {
            Id = itemId ?? new MongoId(),
            Template = itemTpl,
            ParentId = "hideout",
            SlotId = "hideout",
            Upd = new Upd { UnlimitedCount = false, StackObjectsCount = 100 },
        };

        _itemsToSell.Add(newItemToAdd);

        return this;
    }

    public ROFluentTraderAssortHelper AddStackCount(int stackCount)
    {
        _itemsToSell[0].Upd.StackObjectsCount = stackCount;

        return this;
    }

    public ROFluentTraderAssortHelper AddLoyaltyLevel(int level)
    {
        _loyaltyLevel[_itemsToSell[0].Id] = level;

        return this;
    }

    public ROFluentTraderAssortHelper AddBarterCost(MongoId itemTpl, int count)
    {
        var sellableItemId = _itemsToSell[0].Id;

        if (_barterScheme.Count == 0)
        {
            var dataToAdd = new BarterScheme { Count = count, Template = itemTpl };

            _barterScheme[sellableItemId] =
            [
                [dataToAdd],
            ];
        }
        else
        {
            var existingData = _barterScheme[sellableItemId][0].FirstOrDefault(x => x.Template == itemTpl);
            if (existingData is not null)
            {
                existingData.Count += count;
            }
            else
            {
                _barterScheme[sellableItemId][0].Add(new BarterScheme { Count = count, Template = itemTpl });
            }
        }

        return this;
    }

    public ROFluentTraderAssortHelper? Export(string traderId)
    {
        var traderData = databaseService.GetTables().Traders.GetValueOrDefault(traderId);

        var rootItemAddedId = _itemsToSell.FirstOrDefault().Id;
        if (traderData.Assort.Items.Exists(x => x.Id == rootItemAddedId))
        {
            logger.Error($"Unable to add complex item with item key: {_itemsToSell[0].Id}, key already in use");

            _itemsToSell.Clear();
            _barterScheme.Clear();
            _loyaltyLevel.Clear();

            return null;
        }

        traderData.Assort.Items.AddRange(_itemsToSell);
        traderData.Assort.BarterScheme[rootItemAddedId] = _barterScheme[rootItemAddedId];
        traderData.Assort.LoyalLevelItems[rootItemAddedId] = _loyaltyLevel[rootItemAddedId];

        _itemsToSell.Clear();
        _barterScheme.Clear();
        _loyaltyLevel.Clear();

        return this;
    }

    public void CreateSingleItemOffer(
        string ItemToAdd,
        int stackCount,
        int loyaltyLevelToPush,
        int reqCost,
        MongoId currencyToUse,
        MongoId traderToUse
    )
    {
        CreateSingleAssortItem(ItemToAdd)
            .AddBarterCost(currencyToUse, reqCost)
            .AddStackCount(stackCount)
            .AddLoyaltyLevel(loyaltyLevelToPush)
            .Export(traderToUse);
    }
}
