using System.Reflection;
using EFT;
using HarmonyLib;
using RaidOverhaul.Controllers;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using RaidOverhaul.Managers;
using RaidOverhaul.Models;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    public class GameWorldPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void Postfix(GameWorld __instance)
        {
            if (ConfigController.ServerConfig.TimeChangesEnabled)
            {
                var time = RaidTime.GetDateTime();
                __instance.GameDateTime.Reset(time, time, 1);
            }

            if (ConfigController.ServerConfig.WeatherChangesEnabled && ConfigController.ServerConfig.SeasonalProgression)
            {
                ConfigController.SeasonConfig = Utils.Get<SeasonalConfig>("/RaidOverhaul/GetWeatherConfig");
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Seasonal progression config refreshed.");
                }
            }

            if (ConfigController.ServerConfig.EnableCustomBoss && FikaBridge.AmHost())
            {
                ConfigController.LegionConfig = Utils.Get<LegionProgressionConfig>("/RaidOverhaul/GetLegionConfig");

                __instance.gameObject.GetOrAddComponent<LegionGroupManager>();
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("LegionGroupManager successfully attached to GameWorld.");
                }
            }

            if (ROPluginConfig.SpecialReqFeatures.Value && FikaBridge.AmHost())
            {
                __instance.gameObject.GetOrAddComponent<SupportBotManager>();
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("SupportBotManager successfully attached to GameWorld.");
                }
            }
        }
    }
}
