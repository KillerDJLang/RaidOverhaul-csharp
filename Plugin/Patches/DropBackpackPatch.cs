using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    internal class OnDeadPatch : ModulePatch
    {
        private static readonly Random _random = new();

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player), nameof(Player.OnDead));
        }

        [PatchPostfix]
        private static void PatchPostFix(ref Player __instance)
        {
            if (ROPluginConfig.DropBackPack.Value && ROPluginConfig.DropBackPackChance.Value > _random.NextDouble())
            {
                __instance.DropBackpack();
            }
        }
    }
}
