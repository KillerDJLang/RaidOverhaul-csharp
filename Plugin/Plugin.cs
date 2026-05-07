using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Comfort.Common;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using HarmonyLib;
using RaidOverhaul.Behavior.Layers;
using RaidOverhaul.Checkers;
using RaidOverhaul.Controllers;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using RaidOverhaul.Models;
using RaidOverhaul.Patches;
using RaidOverhaulPrepatch.Helpers;
using SPT.Reflection.Utils;
using UnityEngine;

[assembly: AssemblyTitle(ClientInfo.ROPluginName)]
[assembly: AssemblyDescription(ClientInfo.ROPluginDescription)]
[assembly: AssemblyCopyright(ClientInfo.ROCopyright)]
[assembly: AssemblyFileVersion(ClientInfo.PluginVersion)]

namespace RaidOverhaul
{
    [BepInIncompatibility(Utils.ROStandaloneKey)]
    [BepInDependency(Utils.UnityToolkitKey, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(Utils.BigBrainKey, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(Utils.SAINKey, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ClientInfo.ROGUID, ClientInfo.ROPluginName, ClientInfo.PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static string ModPath = Path.Combine(Environment.CurrentDirectory, "SPT", "user", "mods", "RaidOverhaul");
        public static readonly string PluginPath = Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "RaidOverhaul");
        public static readonly string ResourcePath = Path.Combine(PluginPath, "Resources");
        internal static readonly List<string> _softDependancies = [Utils.FikaCoreKey];

        public static GameObject UICanvasPrefab;
        public static GameObject UIItemRowPrefab;
        public static GameObject NotificationCanvasPrefab;
        public static GameObject DebugCanvasPrefab;
        public static GameObject EffectsCanvasPrefab;

        public static AudioClip SoundAlarm;
        public static AudioClip SoundHeartbeat;
        public static AudioClip SoundButtonClick;
        public static AudioClip SoundBeepGreen;
        public static AudioClip SoundBeepRed;
        public static AudioClip SoundBeepYellow;

        internal static ManualLogSource _log;
        internal static PropertyInfo _abstractQuestControllerQuestsProp;
        internal static MethodInfo _abstractQuestControllerGetMethod;

        public static GameWorld ROGameWorld
        {
            get { return Singleton<GameWorld>.Instance; }
        }

        public static Player ROPlayer
        {
            get { return ROGameWorld.MainPlayer; }
        }

        internal static SkillManager ROSkillManager
        {
            get { return ROGameWorld.MainPlayer.Skills; }
        }

        internal static Player.FirearmController ROFirearmController
        {
            get { return ROPlayer.HandsController as Player.FirearmController; }
        }

        internal static AbstractQuestControllerClass ROQuestController
        {
            get { return ROGameWorld.MainPlayer.AbstractQuestControllerClass; }
        }

        private static bool RealismDetected { get; set; }
        private static bool FikaDetected { get; set; }

        private void Awake()
        {
            if (!VersionChecker.CheckEftVersion(Logger, Info, Config))
            {
                throw new Exception("Invalid EFT Version");
            }

            if (!DependencyChecker.ValidateDependencies(Logger, Info, this.GetType(), Config))
            {
                throw new Exception("Missing Dependencies");
            }

            InitializeModDetections();

            ROPluginConfig.BindConfig(Config);

            _log = Logger;
            Logger.LogInfo("Loading Raid Overhaul");

            string bundlePath = Path.Combine(ResourcePath, "Bundles", "ui_elements_ro.nameless");
            var uiBundle = AssetBundle.LoadFromFile(bundlePath);
            if (uiBundle == null)
            {
                Logger.LogError("[RaidOverhaul] Failed to load ui_elements_ro bundle.");
            }
            else
            {
                UICanvasPrefab = uiBundle.LoadAsset<GameObject>("Canvas");
                UIItemRowPrefab = uiBundle.LoadAsset<GameObject>("ItemRow");
                NotificationCanvasPrefab = uiBundle.LoadAsset<GameObject>("NotificationCanvas");
                DebugCanvasPrefab = uiBundle.LoadAsset<GameObject>("DebugCanvas");
                uiBundle.Unload(false);
            }

            string effectsBundlePath = Path.Combine(ResourcePath, "Bundles", "effects_ro.nameless");
            var effectsBundle = AssetBundle.LoadFromFile(effectsBundlePath);
            if (effectsBundle == null)
            {
                Logger.LogError("[RaidOverhaul] Failed to load effects_ro bundle.");
            }
            else
            {
                EffectsCanvasPrefab = effectsBundle.LoadAsset<GameObject>("EffectsCanvas");
                effectsBundle.Unload(false);
            }

            string audioBundlePath = Path.Combine(ResourcePath, "Sound", "audio_ro.nameless");
            var audioBundle = AssetBundle.LoadFromFile(audioBundlePath);
            if (audioBundle == null)
            {
                Logger.LogError("[RaidOverhaul] Failed to load audio_ro bundle.");
            }
            else
            {
                SoundAlarm = audioBundle.LoadAsset<AudioClip>("alarm_oneshot");
                SoundHeartbeat = audioBundle.LoadAsset<AudioClip>("heartbeat");
                SoundButtonClick = audioBundle.LoadAsset<AudioClip>("button_click");
                SoundBeepGreen = audioBundle.LoadAsset<AudioClip>("beep_green");
                SoundBeepRed = audioBundle.LoadAsset<AudioClip>("beep_red");
                SoundBeepYellow = audioBundle.LoadAsset<AudioClip>("beep_yellow");
            }

            ConfigController.EventConfig = Utils.Get<EventsConfig>("/RaidOverhaul/GetEventConfig");
            ConfigController.ServerConfig = Utils.Get<ServerConfigs>("/RaidOverhaul/GetServerConfig");
            ConfigController.DebugConfig = Utils.Get<DebugConfigs>("/RaidOverhaul/GetDebugConfig");
            ConfigController.SeasonConfig = Utils.Get<SeasonalConfig>("/RaidOverhaul/GetWeatherConfig");
            ConfigController.LegionConfig = Utils.Get<LegionProgressionConfig>("/RaidOverhaul/GetLegionConfig");

            _abstractQuestControllerQuestsProp = AccessTools.Property(
                typeof(AbstractQuestControllerClass),
                nameof(AbstractQuestControllerClass.Quests)
            );
            _abstractQuestControllerGetMethod = AccessTools.Method(
                _abstractQuestControllerQuestsProp.PropertyType,
                "GetConditional",
                new Type[] { typeof(string) }
            );

            if (ConfigController.ServerConfig.TimeChangesEnabled)
            {
                new GlobalsPatch().Enable();
                new EnableEntryPointPatch().Enable();
                new UIPanelPatch().Enable();
                new TimerUIPatch().Enable();
                new FactoryTimerPanelPatch().Enable();
                new ExitTimerUIPatch().Enable();
                new WeatherControllerPatch().Enable();
                new WatchPatch().Enable();
                new LocationTimeUIPatch().Enable();
            }

            if (ROPluginConfig.Deafness.Value && !RealismDetected)
            {
                new DeafnessPatch().Enable();
                new GrenadeDeafnessPatch().Enable();
            }

            if (ROPluginConfig.Concussion.Value && !RealismDetected)
            {
                new ConcussionPatch().Enable();
            }

            //new KeyPatch().Enable();
            //new KeycardPatch().Enable();
            new TarkovInitPatch().Enable();
            new GameWorldPatch().Enable();
            new OnDeadPatch().Enable();
            new RandomizeDefaultStatePatch().Enable();
            if (ROPluginConfig.EnableClean.Value)
            {
                new AirdropLootOverridePatch().Enable();
            }

            TryInitFikaAssembly();
        }

        private void OnEnable()
        {
            FikaBridge.PluginEnable();
        }

        internal static ISession GetSession()
        {
            return ClientAppUtils.GetMainApp()?.GetClientBackEndSession();
        }

        public static EventController GetEventController()
        {
            return ClientAppUtils.GetMainApp()?.GetComponent<EventController>();
        }

        private void InitializeModDetections()
        {
            if (Chainloader.PluginInfos.ContainsKey(Utils.FikaCoreKey))
            {
                FikaDetected = true;
            }
            if (Chainloader.PluginInfos.ContainsKey(Utils.RealismKey))
            {
                RealismDetected = true;
            }
        }

        internal static void RegisterLegionBrainLayers()
        {
            var brains = new List<string> { "PMC", "ExUsec", "Assault", "PmcUsec", "PmcBear", "PmcUSEC", "PmcBEAR" };
            var types = new List<WildSpawnType> { (WildSpawnType)199, (WildSpawnType)200 };
            BrainManager.AddCustomLayer(typeof(LegionFallbackLayer), brains, 65, types);
            BrainManager.AddCustomLayer(typeof(LegionBreachLayer), brains, 60, types);
            BrainManager.AddCustomLayer(typeof(LegionSuppressFlankLayer), brains, 55, types);
            BrainManager.AddCustomLayer(typeof(LegionGroupLayer), brains, 50, types);
            BrainManager.AddCustomLayer(typeof(LegionAmbushLayer), brains, 45, types);
            BrainManager.AddCustomLayer(typeof(LegionSweepLayer), brains, 40, types);
        }

        internal static void RegisterSupportBrainLayers()
        {
            var brains = new List<string> { "PMC", "ExUsec", "Assault", "PmcUsec", "PmcBear", "PmcUSEC", "PmcBEAR" };
            var types = new List<WildSpawnType> { (WildSpawnType)201, (WildSpawnType)202 };
            BrainManager.AddCustomLayer(typeof(SupportDespawnLayer), brains, 65, types);
            BrainManager.AddCustomLayer(typeof(SupportBreachLayer), brains, 60, types);
            BrainManager.AddCustomLayer(typeof(SupportSuppressFlankLayer), brains, 55, types);
            BrainManager.AddCustomLayer(typeof(SupportGuardLayer), brains, 50, types);
        }

        private static void TryInitFikaAssembly()
        {
            if (!FikaDetected)
            {
                return;
            }

            var fikaModuleAssembly = Assembly.Load("RaidOverhaulFika");
            var main = fikaModuleAssembly.GetType("RaidOverhaul.FikaModule.FikaMain");
            var init = main.GetMethod("Init");

            init.Invoke(main, null);
        }
    }
}
