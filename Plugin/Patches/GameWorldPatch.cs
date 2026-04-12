using System.Reflection;
using EFT;
using RaidOverhaul.Configs;
using RaidOverhaul.Controllers;
using RaidOverhaul.Helpers;
using RaidOverhaul.Models;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    public class GameWorldPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void Postfix(GameWorld __instance)
        {
            if (ConfigController.ServerConfig.WeatherChangesEnabled && ConfigController.ServerConfig.SeasonalProgression)
            {
                ConfigController.SeasonConfig = Utils.Get<SeasonalConfig>("/RaidOverhaul/GetWeatherConfig");
            }

            if (DJConfig.TimeChanges.Value)
            {
                var time = RaidTime.GetDateTime();
                __instance.GameDateTime.Reset(time, time, 1);
            }
        }
    }
}
