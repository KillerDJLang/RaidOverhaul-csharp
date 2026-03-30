using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.UI;
using RaidOverhaul.Checkers;
using RaidOverhaul.Configs;
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
    [BepInDependency("me.sol.sain", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("xyz.drakia.bigbrain", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.arys.unitytoolkit", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ClientInfo.ROGUID, ClientInfo.ROPluginName, ClientInfo.PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static string ModPath = Path.Combine(Environment.CurrentDirectory, "SPT", "user", "mods", "RaidOverhaul");
        public static readonly string PluginPath = Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "RaidOverhaul");
        public static readonly string ResourcePath = Path.Combine(PluginPath, "Resources");
        internal static readonly List<string> _softDependancies = ["com.fika.core"];
        internal static TextAsset _legionText;

        private static GameObject _hook;
        public static EventController _ecScript;
        internal static DoorController _dcScript;
        internal static SeasonalWeatherController _wScript;
        internal static BodyCleanup _bcScript;
        internal static InRaidUIController _smScript;
        internal static InvasionController _icScript;
        internal static ManualLogSource _log;

        internal static ISession _session;

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

        private static bool RealismDetected { get; set; }
        private static bool StandaloneDetected { get; set; }
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

            if (Chainloader.PluginInfos.ContainsKey("com.fika.core"))
            {
                FikaDetected = true;
            }

            // Bind the configs
            DJConfig.BindConfig(Config);

            _log = Logger;
            Logger.LogInfo("Loading Raid Overhaul");
            _hook = new GameObject("Event Object");

            _ecScript = _hook.GetOrAddComponent<EventController>();
            _dcScript = _hook.GetOrAddComponent<DoorController>();
            _wScript = _hook.GetOrAddComponent<SeasonalWeatherController>();
            _bcScript = _hook.GetOrAddComponent<BodyCleanup>();
            _smScript = _hook.GetOrAddComponent<InRaidUIController>();
            _icScript = _hook.GetOrAddComponent<InvasionController>();

            DontDestroyOnLoad(_hook);
            _hook.SetActive(true);

            ConfigController.EventConfig = Utils.Get<EventsConfig>("/RaidOverhaul/GetEventConfig");
            ConfigController.ServerConfig = Utils.Get<ServerConfigs>("/RaidOverhaul/GetServerConfig");
            ConfigController.DebugConfig = Utils.Get<DebugConfigs>("/RaidOverhaul/GetDebugConfig");
            Weighting.InitWeightings();

            Utils.GetWeatherFields();

            if (DJConfig.TimeChanges.Value)
            {
                new GameWorldPatch().Enable();
                new GlobalsPatch().Enable();
                new EnableEntryPointPatch().Enable();
                new UIPanelPatch().Enable();
                new TimerUIPatch().Enable();
                new FactoryTimerPanelPatch().Enable();
                //new ExitTimerUIPatch().Enable();
                new WeatherControllerPatch().Enable();
                new WatchPatch().Enable();
            }

            if (DJConfig.Deafness.Value && !RealismDetected)
            {
                new DeafnessPatch().Enable();
                new GrenadeDeafnessPatch().Enable();
            }

            if (DJConfig.Concussion.Value && !RealismDetected)
            {
                new ConcussionPatch().Enable();
            }

            new KeyPatch().Enable();
            new KeycardPatch().Enable();
            new OnDeadPatch().Enable();
            new EnableEntryPointPatch().Enable();
            new RandomizeDefaultStatePatch().Enable();
            new EventExfilPatch().Enable();
            new BotNVGPatch().Enable();

            if (ConfigController.DebugConfig.DebugMode)
            {
                ConsoleCommands.RegisterCC();
            }

            TryInitFikaAssembly();
        }

        private void Update()
        {
            if (_hook == null)
            {
                var foundObject = GameObject.Find("Event Object");
                if (foundObject != null)
                {
                    _hook = foundObject;
                    _wScript = _hook.GetOrAddComponent<SeasonalWeatherController>();
                    _smScript = _hook.GetOrAddComponent<InRaidUIController>();
                    _ecScript = _hook.GetOrAddComponent<EventController>();
                    _dcScript = _hook.GetOrAddComponent<DoorController>();
                    _bcScript = _hook.GetOrAddComponent<BodyCleanup>();
                    _icScript = _hook.GetOrAddComponent<InvasionController>();
                }
                else
                {
                    RecreateGameObject();
                }
            }

            if (_smScript != null && _smScript.enabled)
            {
                _smScript.ManualUpdate();
            }

            if (_ecScript != null && _ecScript.enabled)
            {
                _ecScript.ManualUpdate();
            }

            if (_dcScript != null && _dcScript.enabled)
            {
                _dcScript.ManualUpdate();
            }

            if (_bcScript != null && _bcScript.enabled)
            {
                _bcScript.ManualUpdate();
            }

            if (_icScript != null && _icScript.enabled)
            {
                _icScript.ManualUpdate();
            }

            if (Chainloader.PluginInfos.ContainsKey(Utils.RealismKey) && PreloaderUI.Instantiated && !RealismDetected)
            {
                RealismDetected = true;
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Realism Detected, disabling ROs deafness and concussion mechanics.");
                }
            }

            if (Chainloader.PluginInfos.ContainsKey(Utils.ROStandaloneKey) && PreloaderUI.Instantiated && !StandaloneDetected)
            {
                StandaloneDetected = true;
                if (GameObject.Find("ErrorScreen"))
                {
                    PreloaderUI.Instance.CloseErrorScreen();
                }

                PreloaderUI.Instance.ShowErrorScreen(
                    "Raid Overhaul Error",
                    "Raid Overhaul is not compatible with Raid Overhaul Standalone. Install only one of the mods or errors will occur."
                );
            }

            if (_session != null && ClientAppUtils.GetMainApp().GetClientBackEndSession() == null)
            {
                return;
            }

            _session = ClientAppUtils.GetMainApp().GetClientBackEndSession();
        }

        private void RecreateGameObject()
        {
            _hook = new GameObject("Event Object");
            _ecScript = _hook.GetOrAddComponent<EventController>();
            _dcScript = _hook.GetOrAddComponent<DoorController>();
            _wScript = _hook.GetOrAddComponent<SeasonalWeatherController>();
            _bcScript = _hook.GetOrAddComponent<BodyCleanup>();
            _smScript = _hook.GetOrAddComponent<InRaidUIController>();
            _icScript = _hook.GetOrAddComponent<InvasionController>();
            DontDestroyOnLoad(_hook);
            _hook.SetActive(true);
        }

        private void OnEnable()
        {
            FikaBridge.PluginEnable();
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
