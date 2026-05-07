using System.Reflection;
using EFT;
using HarmonyLib;
using RaidOverhaul.Controllers;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    internal class RandomizeDefaultStatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        private static void PatchPrefix()
        {
            DoorController.RandomizeDefaultDoors();
            DoorController.RandomizeLampState();
        }
    }
}
