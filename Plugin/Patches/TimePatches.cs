using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.UI.BattleTimer;
using EFT.UI.Map;
using EFT.UI.Matchmaker;
using HarmonyLib;
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
            return typeof(TarkovApplication).GetMethod("InternalStartGame", BindingFlags.Instance | BindingFlags.Public);
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
            return typeof(EntryPointView).GetMethod("Show", BindingFlags.Instance | BindingFlags.Public);
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
            return typeof(LocationConditionsPanel).GetMethod("method_1", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPostfix]
        private static void Postfix(ref TextMeshProUGUI ____currentPhaseTime, ref TextMeshProUGUI ____nextPhaseTime)
        {
            try
            {
                ____nextPhaseTime.text = RaidTime.GetInverseTime().ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                Plugin._log.LogError(ex);
            }
            finally
            {
                ____currentPhaseTime.text = RaidTime.GetCurrTime().ToString("HH:mm:ss");
            }
        }
    }

    public class TimerUIPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(TimerPanel).GetMethod("SetTimerText", BindingFlags.Instance | BindingFlags.Public);
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
        private static void Postfix(RaidSettings raidSettings, bool takeFromCurrent, MatchMakerAcceptScreen __instance)
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

            if (raidSettings.SelectedLocation.Id == "factory4_day")
            {
                SetTimePanelText(timePanel, "15:28:00");
            }

            if (raidSettings.SelectedLocation.Id == "factory4_night")
            {
                SetTimePanelText(timePanel, "03:28:00");
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

    /*
        public class ExitTimerUIPatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod() => typeof(MainTimerPanel).GetMethod("UpdateTimer", BindingFlags.Instance | BindingFlags.Public);

            [PatchTranspiler]
            static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
            {
                int shift = 0;

                instructions.ExecuteForEach((inst) =>
                {
                    if (shift == 2)
                        inst.opcode = OpCodes.Ret;
                    if (shift >= 3)
                        inst.opcode = OpCodes.Nop;
                    shift++;
                });

                return instructions;
            }
        }
    */
    public class WatchPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Watch).GetProperty("DateTime_0", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(true);
        }

        [PatchPostfix]
        private static void Postfix(ref DateTime __result)
        {
            __result = RaidTime.GetDateTime();
        }
    }
}
