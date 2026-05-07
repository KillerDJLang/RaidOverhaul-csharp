using System;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using RaidOverhaul.Helpers;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    internal class KeyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GetActionsClass), nameof(GetActionsClass.smethod_14));
        }

        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, Door door)
        {
            if (door.DoorState != EDoorState.Locked)
            {
                return;
            }

            var doorUnlockClass = new GetActionsClass.Class1780 { owner = owner, worldInteractiveObject = door };

            if (__result?.Actions != null)
            {
                if (!HasKey(Utils.SkeletonKey))
                {
                    return;
                }

                var position = 1;
                if (!HasKey(door.KeyId))
                {
                    position = 0;
                }

                __result.Actions.Insert(
                    position,
                    new ActionsTypesClass
                    {
                        Name = "Unlock With Skeleton Key",

                        Action = new Action(() =>
                        {
                            var originalKey = door.KeyId;
                            door.KeyId = Utils.SkeletonKey;
                            doorUnlockClass.key = owner.GetKey(door);
                            doorUnlockClass.method_0();
                            door.KeyId = originalKey;
                        }),
                        Disabled = !doorUnlockClass.worldInteractiveObject.Operatable,
                    }
                );
            }
        }

        private static bool HasKey(string keyId)
        {
            return Singleton<GameWorld>.Instance.MainPlayer.Profile.Inventory.Equipment.GetAllItems().Any(x => x.TemplateId == keyId);
        }
    }
}
