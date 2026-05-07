using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils.Cloners;

namespace RaidOverhaulMain.Helpers;

[Injectable(InjectionType.Singleton)]
public class ROTraderHelper(ISptLogger<ROTraderHelper> logger, ICloner cloner, DatabaseService databaseService)
{
    public void SetTraderUpdateTime(TraderConfig traderConfig, TraderBase baseJson, int refreshTimeSecondsMin, int refreshTimeSecondsMax)
    {
        var traderRefreshRecord = new UpdateTime
        {
            TraderId = baseJson.Id,
            Seconds = new MinMax<int>(refreshTimeSecondsMin, refreshTimeSecondsMax),
        };

        traderConfig.UpdateTime.Add(traderRefreshRecord);
    }

    public void AddTraderWithEmptyAssortToDb(TraderBase traderDetailsToAdd)
    {
        if (traderDetailsToAdd == null)
        {
            return;
        }
        var emptyTraderItemAssortObject = new TraderAssort
        {
            Items = [],
            BarterScheme = new Dictionary<MongoId, List<List<BarterScheme>>>(),
            LoyalLevelItems = new Dictionary<MongoId, int>(),
        };

        var traderDataToAdd = new Trader
        {
            Assort = emptyTraderItemAssortObject,
            Base = cloner.Clone(traderDetailsToAdd)!,
            QuestAssort = new()
            {
                { "Started", new() },
                { "Success", new() },
                { "Fail", new() },
            },
            Dialogue = [],
        };

        if (!databaseService.GetTables().Traders.TryAdd(traderDetailsToAdd.Id, traderDataToAdd))
        {
            ROLogger.LogWarning(logger, "Failed to add trader details to database");
        }
    }

    public void AddTraderToLocales(TraderBase baseJson, string firstName, string description)
    {
        var locales = databaseService.GetTables().Locales.Global;
        var newTraderId = baseJson.Id;
        var fullName = baseJson.Name;
        var nickName = baseJson.Nickname;
        var location = baseJson.Location;

        foreach (var (localeKey, localeKvP) in locales)
        {
            localeKvP.AddTransformer(lazyloadedLocaleData =>
            {
                var localeData = lazyloadedLocaleData!;
                localeData.Add($"{newTraderId} FullName", fullName);
                localeData.Add($"{newTraderId} FirstName", firstName);
                localeData.Add($"{newTraderId} Nickname", nickName ?? string.Empty);
                localeData.Add($"{newTraderId} Location", location ?? string.Empty);
                localeData.Add($"{newTraderId} Description", description);
                return localeData;
            });
        }
    }
}
