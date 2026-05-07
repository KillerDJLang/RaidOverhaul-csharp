using RaidOverhaulMain.Helpers;
using RaidOverhaulMain.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Cloners;

namespace RaidOverhaulMain.Callbacks;

[Injectable]
public class TransferRequestCallbacks(HttpResponseUtil httpResponseUtil, MailSendService mailSendService, ICloner cloner)
{
    public virtual ValueTask<string> ReceiveAndSendItems(TransferRequestData request, MongoId sessionId)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return new ValueTask<string>(httpResponseUtil.NullResponse());
        }

        var itemsClone = cloner.Clone(request.Items)?.ReplaceIDs().ToList();

        mailSendService.SendDirectNpcMessageToPlayer(
            sessionId,
            request.TraderId,
            MessageType.BtrItemsDelivery,
            request.Message ?? "Your items have been delivered. Don't forget to leave a tip!",
            itemsClone,
            172800
        );

        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }
}

[Injectable]
public class LogToServerRequestCallbacks(HttpResponseUtil httpResponseUtil)
{
    public virtual ValueTask<string> LogToServer<T>(LogToServerRequestData request, ISptLogger<T> logger)
    {
        ROLogger.LogToServer(logger, request.Message ?? string.Empty, LogTextColor.Cyan);

        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }
}
