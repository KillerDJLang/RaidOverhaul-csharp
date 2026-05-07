using System;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace RaidOverhaul.Patches
{
    internal struct PlayerInfo
    {
        internal static bool PlayerHasEarPro()
        {
            CompoundItem helm;

            if (Plugin.ROPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Earpiece).ContainedItem != null)
            {
                return true;
            }

            if ((helm = Plugin.ROPlayer.Profile.Inventory.Equipment.GetSlot(EquipmentSlot.Headwear).ContainedItem as CompoundItem) == null)
            {
                return false;
            }

            var blocker = helm.GetItemComponent<SlotBlockerComponent>();
            if (blocker != null && blocker.ConflictingSlotNames.Contains("Earpiece"))
            {
                return true;
            }

            return helm.Slots.Any(slot =>
                slot.ContainedItem?.GetItemComponent<SlotBlockerComponent>()?.ConflictingSlotNames != null
                && slot.ContainedItem.GetItemComponent<SlotBlockerComponent>().ConflictingSlotNames.Contains("Earpiece")
            );
        }
    }

    internal class DeafnessPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.FirearmController), nameof(Player.FirearmController.RegisterShot));
        }

        [PatchPostfix]
        private static void Postfix(Player.FirearmController __instance, object shot)
        {
            if (Plugin.ROPlayer is HideoutPlayer)
            {
                return;
            }

            var bulletSpeed = (float)shot.GetType().GetField("Speed", BindingFlags.Instance | BindingFlags.Public).GetValue(shot);

            if (Plugin.ROFirearmController == __instance && GoodToDeafen(bulletSpeed))
            {
                DoEarDamage(false);
            }
            else if (TargetGoodToDeafen(__instance, bulletSpeed))
            {
                DoEarDamage(true);
            }
        }

        private static bool TargetGoodToDeafen(Player.FirearmController target, float bulletSpeed)
        {
            return Vector3.Distance(target.gameObject.transform.position, Plugin.ROPlayer.Transform.position) <= 45
                && !PlayerInfo.PlayerHasEarPro()
                && !target.IsSilenced
                && bulletSpeed > 343f;
        }

        private static bool GoodToDeafen(float bulletSpeed)
        {
            return !PlayerInfo.PlayerHasEarPro()
                && !Plugin.ROFirearmController.IsSilenced
                && (bulletSpeed > 343f || Plugin.ROPlayer.Environment == EnvironmentType.Indoor);
        }

        private static void DoEarDamage(bool invokedByBot)
        {
            if (!invokedByBot && Plugin.ROFirearmController.Item.AmmoCaliber == "86x70")
            {
                try
                {
                    Plugin.ROPlayer.ActiveHealthController.DoStun(1, 0);
                    Plugin.ROPlayer.ActiveHealthController.DoContusion(4, 50);
                }
                catch (Exception ex)
                {
                    Plugin._log.LogError("Attempting to access ActiveHealthController resulted in an exception" + ex);
                }
            }
            try
            {
                Plugin.ROPlayer.ActiveHealthController.DoStun(1, 0);
                Plugin.ROPlayer.ActiveHealthController.DoContusion(0, 100);
            }
            catch (Exception ex)
            {
                Plugin._log.LogError("Attempting to access ActiveHealthController resulted in an exception" + ex);
            }
        }
    }

    public class GrenadeDeafnessPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Grenade), nameof(Grenade.OnExplosion));
        }

        [PatchPrefix]
        private static void Prefix(Grenade __instance)
        {
            var dist = Vector3.Distance(__instance.transform.position, Plugin.ROPlayer.Transform.position);
            if (PlayerInfo.PlayerHasEarPro() && dist >= 30)
            {
                return;
            }

            Plugin.ROPlayer.ActiveHealthController.DoStun(1, 0);
            Plugin.ROPlayer.ActiveHealthController.DoContusion(30 / (dist / 2), 100 / dist);
        }
    }

    public class ConcussionPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(Player), nameof(Player.ApplyShot));
        }

        [PatchPostfix]
        private static void PatchPostfix(
            Player __instance,
            DamageInfoStruct damageInfo,
            EBodyPart bodyPartType,
            EBodyPartColliderType colliderType,
            EArmorPlateCollider armorPlateCollider,
            ShotIdStruct shotId
        )
        {
            if (bodyPartType == EBodyPart.Head && !string.IsNullOrEmpty(damageInfo.BlockedBy))
            {
                var hsDmg = damageInfo.Damage;

                try
                {
                    Plugin.ROPlayer.ActiveHealthController.DoStun(1, 0);
                    Plugin.ROPlayer.ActiveHealthController.DoContusion(4, hsDmg * 1.5f);
                }
                catch (Exception ex)
                {
                    Plugin._log.LogError("Attempting to access ActiveHealthController resulted in an exception" + ex);
                }
            }
        }
    }
}
