using System.Reflection;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using RaidOverhaul.Helpers;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    internal class KeycardPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(KeycardDoor), nameof(KeycardDoor.UnlockOperation));
        }

        [PatchPrefix]
        private static bool PatchPrefix(
            ref GStruct156<KeyInteractionResultClass> __result,
            KeyComponent key,
            Player player,
            KeycardDoor __instance
        )
        {
            var canInteract = player.MovementContext.CanInteract;
            if (canInteract != null)
            {
                __result = canInteract;
                return false;
            }

            var isAuthorized = key.Template.KeyId == __instance.KeyId || key.Template.KeyId == Utils.VipKeycard;
            if (!isAuthorized)
            {
                __result = new KeyInteractionResultClass(key, null, false);
                return false;
            }

            key.NumberOfUsages++;
            if (key.NumberOfUsages >= key.Template.MaximumNumberOfUsage && key.Template.MaximumNumberOfUsage > 0)
            {
                var discardResult = InteractionsHandlerClass.Discard(key.Item, (TraderControllerClass)key.Item.Parent.GetOwner(), false);

                if (discardResult.Failed)
                {
                    __result = discardResult.Error;
                    return false;
                }
                __result = new KeyInteractionResultClass(key, discardResult.Value, true);
                return false;
            }
            __result = new KeyInteractionResultClass(key, null, true);
            return false;
        }
    }
}
