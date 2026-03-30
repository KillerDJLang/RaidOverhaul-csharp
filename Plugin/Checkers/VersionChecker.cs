using System;
using System.Diagnostics;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using RaidOverhaulPrepatch.Helpers;
using UnityEngine;

namespace RaidOverhaul.Checkers
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class VersionChecker : Attribute
    {
        public static bool CheckEftVersion(ManualLogSource logger, PluginInfo info, ConfigFile config = null)
        {
            var currentVersion = FileVersionInfo.GetVersionInfo(BepInEx.Paths.ExecutablePath).FilePrivatePart;
            const int buildVersion = ClientInfo.TarkovVersion;
            if (currentVersion != buildVersion)
            {
                var errorMessage =
                    $"ERROR: This version of Raid Overhaul {ClientInfo.PluginVersion} was built for Tarkov {buildVersion}, but you are running {currentVersion}. Please download the correct plugin version.";
                logger.LogError(errorMessage);
                Chainloader.DependencyErrors.Add(errorMessage);

                if (config != null)
                {
                    config.Bind(
                        "",
                        "TarkovVersion",
                        "",
                        new ConfigDescription(
                            errorMessage,
                            null,
                            new ConfigurationManagerAttributes
                            {
                                CustomDrawer = ErrorLabelDrawer,
                                ReadOnly = true,
                                HideDefaultButton = true,
                                HideSettingName = true,
                                Category = null,
                            }
                        )
                    );
                }

                return false;
            }

            return true;
        }

        private static void ErrorLabelDrawer(ConfigEntryBase entry)
        {
            var styleNormal = new GUIStyle(GUI.skin.label);
            styleNormal.wordWrap = true;
            styleNormal.stretchWidth = true;

            var styleError = new GUIStyle(GUI.skin.label);
            styleError.stretchWidth = true;
            styleError.alignment = TextAnchor.MiddleCenter;
            styleError.normal.textColor = Color.red;
            styleError.fontStyle = FontStyle.Bold;

            GUILayout.BeginVertical();
            GUILayout.Label(entry.Description.Description, styleNormal, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

            GUILayout.Label("Plugin has been disabled!", styleError, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            GUILayout.EndVertical();
        }

#pragma warning disable 0169, 0414, 0649
        internal sealed class ConfigurationManagerAttributes
        {
            public bool? ShowRangeAsPercent;
            public Action<ConfigEntryBase> CustomDrawer;
            public CustomHotkeyDrawerFunc CustomHotkeyDrawer;
            public delegate void CustomHotkeyDrawerFunc(ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);
            public bool? Browsable;
            public string Category;
            public object DefaultValue;
            public bool? HideDefaultButton;
            public bool? HideSettingName;
            public string Description;
            public string DispName;
            public int? Order;
            public bool? ReadOnly;
            public bool? IsAdvanced;
            public Func<object, string> ObjToStr;
            public Func<string, object> StrToObj;
        }
    }
}
