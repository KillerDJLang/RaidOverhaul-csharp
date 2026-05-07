using System.Reflection;
using EFT;
using EFT.InputSystem;
using HarmonyLib;
using RaidOverhaul.Controllers;
using RaidOverhaul.Helpers;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    public class TarkovInitPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication), nameof(TarkovApplication.Init));
        }

        [PatchPostfix]
        private static void PatchPostfix(TarkovApplication __instance, IAssetsManager assetsManager, InputTree inputTree)
        {
            var eventController = __instance.GetOrAddComponent<EventController>();
            var doorController = __instance.GetOrAddComponent<DoorController>();
            __instance.GetOrAddComponent<SeasonalWeatherController>();
            __instance.GetOrAddComponent<InRaidUIController>();
            __instance.GetOrAddComponent<NotificationUIController>();
            __instance.GetOrAddComponent<EventsEffectsController>();
            if (ConfigController.DebugConfig.IsDev)
            {
                __instance.GetOrAddComponent<DebugUIController>();
            }

            Weighting.InitWeightings(eventController, doorController);

            Plugin.RegisterLegionBrainLayers();
            Plugin.RegisterSupportBrainLayers();
            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Brain layers successfully registered.");
            }
        }
    }
}
