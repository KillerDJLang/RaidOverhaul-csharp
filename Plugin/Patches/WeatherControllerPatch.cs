using System.Reflection;
using EFT.Weather;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    internal class WeatherControllerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WeatherController), nameof(WeatherController.Awake));
        }

        [PatchPostfix]
        private static void Postfix(ref WeatherController __instance)
        {
            __instance.WindController.CloudWindMultiplier = 1;
        }
    }
}
