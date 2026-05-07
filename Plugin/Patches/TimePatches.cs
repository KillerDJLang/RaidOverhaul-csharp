using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EFT;
using EFT.UI;
using EFT.UI.BattleTimer;
using EFT.UI.Map;
using EFT.UI.Matchmaker;
using HarmonyLib;
using JsonType;
using SPT.Reflection.Patching;
using TMPro;

namespace RaidOverhaul.Patches
{
    public struct RaidTime
    {
        internal static bool _inverted = false;

        private static DateTime InverseTime
        {
            get
            {
                var result = DateTime.Now.AddHours(12);
                return result.Day > DateTime.Now.Day ? result.AddDays(-1)
                    : result.Day < DateTime.Now.Day ? result.AddDays(1)
                    : result;
            }
        }

        public static DateTime GetCurrTime()
        {
            return DateTime.Now;
        }

        public static DateTime GetInverseTime()
        {
            return InverseTime;
        }

        public static DateTime GetDateTime()
        {
            return _inverted ? GetInverseTime() : GetCurrTime();
        }
    }

    public class GlobalsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TarkovApplication), nameof(TarkovApplication.InternalStartGame));
        }

        [PatchPrefix]
        private static void Prefix(ref string gameMap)
        {
            if (gameMap != "factory4_day" && gameMap != "factory4_night")
            {
                return;
            }

            bool isDaytime = RaidTime.GetCurrTime().Hour >= 6 && RaidTime.GetCurrTime().Hour < 18;

            if (gameMap == "factory4_day")
            {
                gameMap = isDaytime ? "factory4_day" : "factory4_night";
            }
            else
            {
                gameMap = isDaytime ? "factory4_night" : "factory4_day";
            }
        }

        [PatchPostfix]
        private static async void Postfix(TarkovApplication __instance)
        {
            while (__instance.GetClientBackEndSession() == null || __instance.GetClientBackEndSession().BackEndConfig == null)
            {
                await Task.Yield();
            }

            var globals = __instance.GetClientBackEndSession().BackEndConfig.Config;
            globals.AllowSelectEntryPoint = true;
        }
    }

    public class EnableEntryPointPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EntryPointView), nameof(EntryPointView.Show));
        }

        [PatchPrefix]
        private static void Prefix(ref bool allowSelection)
        {
            allowSelection = true;
        }
    }

    public class UIPanelPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocationConditionsPanel), nameof(LocationConditionsPanel.method_1));
        }

        [PatchPostfix]
        private static void Postfix(
            TextMeshProUGUI ____currentPhaseTime,
            TextMeshProUGUI ____nextPhaseTime,
            bool ___bool_1,
            EDateTime ___edateTime_0
        )
        {
            if (____currentPhaseTime == null)
            {
                return;
            }

            if (___bool_1)
            {
                ____currentPhaseTime.SetMonospaceText(RaidTime.GetCurrTime().ToString("HH:mm:ss"), true);
                if (____nextPhaseTime != null)
                {
                    ____nextPhaseTime.SetMonospaceText(RaidTime.GetInverseTime().ToString("HH:mm:ss"), true);
                }
            }
            else
            {
                string time =
                    (___edateTime_0 == EDateTime.PAST)
                        ? RaidTime.GetInverseTime().ToString("HH:mm:ss")
                        : RaidTime.GetCurrTime().ToString("HH:mm:ss");
                ____currentPhaseTime.SetMonospaceText(time, true);
            }
        }
    }

    public class TimerUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TimerPanel), nameof(TimerPanel.SetTimerText));
        }

        [PatchPrefix]
        private static void Prefix(ref TimeSpan timeSpan)
        {
            timeSpan = new TimeSpan(RaidTime.GetDateTime().Ticks);
        }
    }

    public class FactoryTimerPanelPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.FirstMethod(
                typeof(LocationConditionsPanel),
                x => x.Name == nameof(LocationConditionsPanel.Set) && x.GetParameters()[0].Name == "session"
            );
        }

        [PatchPostfix]
        private static void Postfix(RaidSettings raidSettings, bool takeFromCurrent, LocationConditionsPanel __instance)
        {
            TextMeshProUGUI timePanel;

            try
            {
                timePanel = __instance
                    .transform.Find("TimePanel")
                    .gameObject.transform.Find("Time")
                    .gameObject.GetComponent<TextMeshProUGUI>();
            }
            catch (Exception)
            {
                return;
            }

            if (raidSettings.SelectedLocation.Id == "factory4_day" || raidSettings.SelectedLocation.Id == "factory4_night")
            {
                SetTimePanelText(timePanel, RaidTime.GetDateTime().ToString("HH:mm:ss"));
            }
        }

        private static void SetTimePanelText(TextMeshProUGUI timePanel, string text)
        {
            try
            {
                timePanel.text = text;
            }
            catch (Exception ex)
            {
                Plugin._log.LogError(ex);
            }
        }
    }

    public class ExitTimerUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MainTimerPanel), nameof(MainTimerPanel.UpdateTimer));
        }

        [PatchTranspiler]
        static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();
            for (int i = 0; i < 2 && i < list.Count; i++)
            {
                yield return list[i];
            }

            yield return new CodeInstruction(OpCodes.Ret);
        }
    }

    public class WatchPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(Watch), nameof(Watch.DateTime_0));
        }

        [PatchPostfix]
        private static void Postfix(ref DateTime __result)
        {
            __result = RaidTime.GetDateTime();
        }
    }

    public class LocationTimeUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.PropertyGetter(typeof(LocationTimeUIPanel), nameof(LocationTimeUIPanel.DateTime_0));
        }

        [PatchPostfix]
        private static void Postfix(ref DateTime __result)
        {
            __result = RaidTime.GetDateTime();
        }
    }
}
