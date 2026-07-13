using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace RaidOverhaulMain.Controllers;

// Restocks the Requisitions Office on its refresh cycle (fixes "Requisitions Office does not
// update", issue #16).
//
// When a trader's timer expires SPT advances its NextResupply and clears each player's per-trader
// buy limits, but its stock-restore step (TraderAssortHelper.ResetExpiredTrader) only re-clones the
// already-depleted live assort back onto itself and never refills the shared StackObjectsCount — see
// the detailed note on ROAssortHelper.RegenerateTraderAssorts. The result is that the Req Office's
// countdown resets but its stock never comes back until a full server restart.
//
// This closes that gap. SPT runs every IOnUpdate roughly every 5s; each tick we read the Req Office
// trader's NextResupply. When SPT advances it (i.e. the cycle just rolled over and per-player limits
// were reset), we regenerate the shop with a fresh, full-stock inventory. Reacting to the observed
// NextResupply change — rather than keeping our own timer — keeps the stock refill in step with
// SPT's per-player reset regardless of the order update components run in.
[Injectable(InjectionType.Singleton)]
public class ROTraderRestock(
    ISptLogger<ROTraderRestock> logger,
    DatabaseService databaseService,
    ROAssortHelper assortHelper,
    ROHelpers helpers
) : IOnUpdate
{
    private int _lastSeenResupply;

    public Task<bool> OnUpdate(long secondsSinceLastRun)
    {
        // ROMain.OnLoad (which sets Config) always runs to completion before the update loop starts.
        if (!ROMain.Config.EnableRequisitionOffice)
        {
            return Task.FromResult(true);
        }

        var traderId = helpers.FetchIdFromMap("ReqShop", ClassMaps.TraderMaps);
        if (string.IsNullOrEmpty(traderId))
        {
            return Task.FromResult(true);
        }

        var trader = databaseService.GetTrader(traderId);
        if (trader?.Base is null)
        {
            return Task.FromResult(true);
        }

        var resupply = trader.Base.NextResupply ?? 0;

        // Wait for SPT to set a real resupply time, then adopt it as our baseline without restocking.
        if (_lastSeenResupply == 0)
        {
            if (resupply > 0)
            {
                _lastSeenResupply = resupply;
            }

            return Task.FromResult(true);
        }

        // SPT advanced the timer -> the refresh cycle rolled over. Refill the shop to match.
        if (resupply != _lastSeenResupply)
        {
            _lastSeenResupply = resupply;

            try
            {
                assortHelper.RegenerateTraderAssorts(traderId);
                ROLogger.Log(logger, "Requisition Shop restocked", LogTextColor.Magenta);
            }
            catch (Exception err)
            {
                ROLogger.LogError(logger, $"Requisition Shop restock failed: {err.Message}");
            }
        }

        return Task.FromResult(true);
    }
}
