using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using CommonAssets.Scripts.Game;
using Cysharp.Threading.Tasks;
using EFT;
using EFT.Airdrop;
using EFT.HealthSystem;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.MovingPlatforms;
using EFT.Quests;
using EFT.UI;
using EFT.UI.BattleTimer;
using EFT.UI.Matchmaker;
using HarmonyLib;
using JsonType;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using RaidOverhaul.Models;
using RaidOverhaul.Patches;
using SAIN.Components;
using UnityEngine;
using UnityEngine.UI;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class EventController : MonoBehaviour
    {
        private static bool _pmcExfilEventRunning;
        private static bool _huntedEventRunning;
        private static bool _eventIsRunning;
        public static bool _exfilLockdown;
        public static bool GearExfil;
        private bool _metabolismDisabled;
        private bool _jokeEventHasRun;
        private bool _airdropEventHasRun;
        private bool _berserkEventHasRun;
        private bool _malfEventHasRun;
        private bool _weightEventHasRun;
        private bool _artyEventHasRun;
        private bool _blackoutEventHasRun;
        private bool _invasionHasRun;
        private bool _legionSpawnedForQuest;
        private bool _huntedEventHasRun;
        private bool _hasInitializedReflection;
        private bool _isReady;
        private bool _flagsChecked;
        private bool _exfilUIChanged;
        private bool _cleanupTimerStarted;

        private int _skillEventCount;
        private int _repairEventCount;
        private int _healthEventCount;
        private int _damageEventCount;
        private int _maxLLEventCount;
        private int _exfilEventCount;

        public static List<Item> PendingCorpseItems = new();
        public static int AirdropCrateCapacity = -1;

        private static readonly EquipmentSlot[] _allEquipmentSlots = (EquipmentSlot[])System.Enum.GetValues(typeof(EquipmentSlot));

        private static bool ShouldCollectSlot(EquipmentSlot slot)
        {
            if (slot == EquipmentSlot.SecuredContainer)
            {
                return false;
            }

            if (slot == EquipmentSlot.ArmBand && !ConfigController.ServerConfig.LootableArmbands)
            {
                return false;
            }

            if (slot == EquipmentSlot.Scabbard && !ConfigController.ServerConfig.LootableMelee)
            {
                return false;
            }

            return true;
        }

        private Switch[] _pswitchs;
        private KeycardDoor[] _keydoor;
        private LampController[] _lamp;

        private static MethodInfo _switchCloseMethod;
        private static MethodInfo _switchLockMethod;
        private static MethodInfo _switchUnlockMethod;
        private static MethodInfo _keycardUnlockMethod;
        private static MethodInfo _keycardOpenMethod;

        private static FieldInfo _edateTimeField;

        private ExtractionTimersPanel _cachedExtractionPanel;
        private ExitTimerPanel[] _cachedExitPanels;

        private static readonly System.Random SharedRandom = new System.Random();

        public DamageInfoStruct Blunt { get; private set; }

        private static readonly HashSet<string> _invalidAirdropLocations = new HashSet<string>
        {
            "factory4_day",
            "factory4_night",
            "laboratory",
            "sandbox",
            "sandbox_high",
        };

        private static readonly HashSet<string> _invalidArtilleryLocations = new HashSet<string>
        {
            "factory4_day",
            "factory4_night",
            "laboratory",
        };

        private static readonly Dictionary<string, string[]> _legionQuestMapZones = new Dictionary<string, string[]>
        {
            { "interchange", new[] { "ZoneOLI", "ZoneIDEA", "ZoneGoshan" } },
            { "tarkovstreets", new[] { "ZoneConstruction", "ZoneFactory", "ZoneConcordiaParking" } },
        };

        private void Awake()
        {
            if (!_hasInitializedReflection)
            {
                InitializeDoorReflection();
                InitializeTimeChangeReflection();
                _hasInitializedReflection = true;
            }
        }

        private void InitializeDoorReflection()
        {
            _switchCloseMethod = AccessTools.Method(typeof(Switch), nameof(Switch.Close));
            _switchLockMethod = AccessTools.Method(typeof(Switch), nameof(Switch.Lock));
            _switchUnlockMethod = AccessTools.Method(typeof(Switch), nameof(Switch.Unlock));
            _keycardUnlockMethod = AccessTools.Method(typeof(KeycardDoor), nameof(KeycardDoor.Unlock));
            _keycardOpenMethod = AccessTools.Method(typeof(KeycardDoor), nameof(KeycardDoor.Open));
        }

        private void InitializeTimeChangeReflection()
        {
            _edateTimeField = AccessTools.Field(typeof(MatchMakerSelectionLocationScreen), "edateTime_0");
        }

        private void Update()
        {
            _isReady = Utils.IsInRaid();

            if (!_isReady || !ROPluginConfig.EnableEvents.Value)
            {
                ResetEventState();
                return;
            }

            if (ConfigController.ServerConfig.TimeChangesEnabled)
            {
                UpdateTimeChanges();
            }

            if (FikaBridge.AmHost())
            {
                if (_pswitchs == null)
                {
                    _pswitchs = FindObjectsOfType<Switch>();
                }
                if (_keydoor == null)
                {
                    _keydoor = FindObjectsOfType<KeycardDoor>();
                }
                if (_lamp == null)
                {
                    _lamp = FindObjectsOfType<LampController>();
                }

                if (!_flagsChecked)
                {
                    CheckForFlags();
                    _flagsChecked = true;
                }

                if (!_eventIsRunning)
                {
                    StartEvents().Forget();
                    _eventIsRunning = true;
                }

                if (!_cleanupTimerStarted && ROPluginConfig.EnableClean.Value)
                {
                    _cleanupTimerStarted = true;
                    RunBodyCleanupCycle().Forget();
                }
            }

            if (_exfilLockdown && !_exfilUIChanged)
            {
                ChangeExfilUI().Forget();
            }
        }

        private void UpdateTimeChanges()
        {
            var menuUI = MonoBehaviourSingleton<MenuUI>.Instance;
            if (menuUI == null || menuUI.MatchMakerSelectionLocationScreen == null)
            {
                return;
            }

            if (_edateTimeField != null)
            {
                var dateTime = (EDateTime)_edateTimeField.GetValue(menuUI.MatchMakerSelectionLocationScreen);
                RaidTime._inverted = dateTime != EDateTime.CURR;
            }
        }

        private void ResetEventState()
        {
            _metabolismDisabled = false;
            _jokeEventHasRun = false;
            _airdropEventHasRun = false;
            _berserkEventHasRun = false;
            _malfEventHasRun = false;
            _weightEventHasRun = false;
            _artyEventHasRun = false;
            _blackoutEventHasRun = false;
            _pmcExfilEventRunning = false;
            _eventIsRunning = false;
            _skillEventCount = 0;
            _repairEventCount = 0;
            _healthEventCount = 0;
            _damageEventCount = 0;
            _maxLLEventCount = 0;
            _exfilEventCount = 0;
            _cachedExtractionPanel = null;
            _cachedExitPanels = null;
            _invasionHasRun = false;
            _legionSpawnedForQuest = false;
            _flagsChecked = false;
            _huntedEventRunning = false;
            _huntedEventHasRun = false;
            _cleanupTimerStarted = false;
            PendingCorpseItems.Clear();
        }

        private async UniTaskVoid StartEvents()
        {
            await UniTask.WaitForSeconds(
                Random.Range(
                    ConfigController.EventConfig.RandomEventRangeMinimumServer,
                    ConfigController.EventConfig.RandomEventRangeMaximumServer
                ) * 60f
            );

            if (Utils.IsInRaid() && FikaBridge.AmHost())
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
            else
            {
                _pswitchs = null;
                _keydoor = null;
                _lamp = null;
            }

            _eventIsRunning = false;
        }

        private async UniTask ChangeExfilUI()
        {
            if (_exfilLockdown)
            {
                var red = new Color(0.8113f, 0.0376f, 0.0714f, 0.8627f);
                var green = new Color(0.4863f, 0.7176f, 0.0157f, 0.8627f);

                if (_cachedExtractionPanel == null)
                {
                    _cachedExtractionPanel = FindObjectOfType<ExtractionTimersPanel>();
                }

                if (_cachedExtractionPanel == null)
                {
                    return;
                }

                var mainDescription = (RectTransform)
                    AccessTools.Field(typeof(ExtractionTimersPanel), "_mainDescription").GetValue(_cachedExtractionPanel);

                var text = mainDescription.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                var box = mainDescription.gameObject.GetComponentInChildren<Image>();

                text.text = "Extraction unavailable";
                box.color = red;

                if (_cachedExitPanels == null)
                {
                    _cachedExitPanels = FindObjectsOfType<ExitTimerPanel>();
                }

                foreach (var panel in _cachedExitPanels)
                {
                    if (panel != null)
                    {
                        panel.enabled = false;
                    }
                }

                _exfilUIChanged = true;

                await UniTask.WaitUntil(() => !_exfilLockdown);

                text.text = "Find an extraction point";
                box.color = green;

                foreach (var panel in _cachedExitPanels)
                {
                    if (panel != null)
                    {
                        panel.enabled = true;
                    }
                }

                _exfilUIChanged = false;
            }
        }

        #region Core Events Controller

        public void DoHealPlayer()
        {
            if (_healthEventCount >= 2)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Heal);
            }

            NotificationHelper.Show(
                "Heal Event: On your feet you ain't dead yet.",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Gold
            );
            EventsEffectsController.Instance?.ShowPositiveEffect();
            ROPlayer.ActiveHealthController.RestoreFullHealth();
            _healthEventCount++;

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Heal Event has run");
            }
        }

        public void DoDamageEvent()
        {
            if (_damageEventCount >= 1)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Damage);
            }

            NotificationHelper.Show(
                "Heart Attack Event: Better get to a medic quick, you don't have long left.",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Red
            );
            EventsEffectsController.Instance?.ShowNegativeEffect();
            ROPlayer.ActiveHealthController.DoContusion(4f, 50f);
            ROPlayer.ActiveHealthController.DoStun(5f, 0f);
            ROPlayer.ActiveHealthController.DoFracture(EBodyPart.LeftArm);
            ROPlayer.ActiveHealthController.ApplyDamage(EBodyPart.Chest, 65f, Blunt);
            _damageEventCount++;

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Heart Attack Event has run");
            }
        }

        public void DoArmorRepair()
        {
            if (_repairEventCount >= 2)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Repair);
            }

            NotificationHelper.Show(
                "Armor Repair Event: All equipped armor repaired... nice!",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Gold
            );
            EventsEffectsController.Instance?.ShowPositiveEffect();
            ROPlayer
                .Profile.Inventory.GetPlayerItems()
                .ExecuteForEach(
                    (item) =>
                    {
                        if (item.GetItemComponent<ArmorComponent>() != null)
                        {
                            item.GetItemComponent<RepairableComponent>().Durability =
                                item.GetItemComponent<RepairableComponent>().MaxDurability;
                        }
                    }
                );

            _repairEventCount++;

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Armor Repair Event has run");
            }
        }

        public void DoAirdropEvent()
        {
            if (!_invalidAirdropLocations.Contains(ROPlayer.Location.ToLowerInvariant()) && !_airdropEventHasRun)
            {
                ROGameWorld.InitAirdrop(null, true, ROPlayer.Transform.position);

                NotificationHelper.Show(
                    "Aidrop Event: Incoming Airdrop!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.White
                );
                EventsEffectsController.Instance?.ShowPositiveEffect();

                _airdropEventHasRun = true;

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Aidrop Event has run");
                }
            }
            else
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
        }

        internal async UniTask DoFunny()
        {
            if (!_jokeEventHasRun)
            {
                if (FikaBridge.AmHost())
                {
                    FikaBridge.SendRandomEventPacket(Utils.Jokes);
                }

                NotificationHelper.Show(
                    "Heart Attack Event: Nice knowing ya, you've got 10 seconds",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Red
                );
                EventsEffectsController.Instance?.ShowCornerGlowEffect();

                await UniTask.WaitForSeconds(10);

                NotificationHelper.Show("jk", NotificationHelper.NotificationLength.Short, NotificationHelper.NotificationColor.White);

                await UniTask.WaitForSeconds(3);

                NotificationHelper.Show(
                    "Rolling the roulette wheel, good luck o7",
                    NotificationHelper.NotificationLength.Short,
                    NotificationHelper.NotificationColor.White
                );

                await UniTask.WaitForSeconds(2);

                var player = ROPlayer;
                if (player != null)
                {
                    var roll = SharedRandom.Next(0, 100);

                    if (roll <= 1)
                    {
                        player.ActiveHealthController.DoExternalBuff("JokeToxic");
                        NotificationHelper.Show(
                            "Roulette: You feel something wrong... that can't be good.",
                            NotificationHelper.NotificationLength.Long,
                            NotificationHelper.NotificationColor.Red
                        );
                        EventsEffectsController.Instance?.ShowNegativeEffect();
                    }
                    else if (roll <= 51)
                    {
                        player.ActiveHealthController.DoExternalBuff("JokeBuff");
                        NotificationHelper.Show(
                            "Roulette: Lady luck is on your side!",
                            NotificationHelper.NotificationLength.Long,
                            NotificationHelper.NotificationColor.Gold
                        );
                        EventsEffectsController.Instance?.ShowPositiveEffect();
                    }
                    else
                    {
                        player.ActiveHealthController.DoExternalBuff("JokeDebuff");
                        NotificationHelper.Show(
                            "Roulette: Not your lucky day, I'm afraid.",
                            NotificationHelper.NotificationLength.Long,
                            NotificationHelper.NotificationColor.Red
                        );
                        EventsEffectsController.Instance?.ShowNegativeEffect();
                    }
                }

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Joke Event has run");
                }

                _jokeEventHasRun = true;
            }

            if (_jokeEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
        }

        internal async UniTask DoBlackoutEvent()
        {
            if (_blackoutEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Blackout);
            }

            _blackoutEventHasRun = true;

            _pswitchs ??= FindObjectsOfType<Switch>();
            _lamp ??= FindObjectsOfType<LampController>();
            _keydoor ??= FindObjectsOfType<KeycardDoor>();

            foreach (var pSwitch in _pswitchs)
            {
                if (pSwitch == null)
                {
                    continue;
                }

                _switchCloseMethod?.Invoke(pSwitch, null);
                _switchLockMethod?.Invoke(pSwitch, null);
            }

            foreach (var lamp in _lamp)
            {
                if (lamp == null)
                {
                    continue;
                }

                lamp.Switch(Turnable.EState.Off);
                lamp.enabled = false;
            }

            foreach (var door in _keydoor)
            {
                if (door == null)
                {
                    continue;
                }

                _keycardUnlockMethod?.Invoke(door, null);
                _keycardOpenMethod?.Invoke(door, null);
            }

            NotificationHelper.Show(
                "Blackout Event: All power switches and lights disabled for 10 minutes",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Red
            );
            EventsEffectsController.Instance?.ShowNegativeEffect();

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Blackout Event: All power switches and lights disabled for 10 minutes");
            }

            await UniTask.WaitForSeconds(600);

            foreach (var pSwitch in _pswitchs)
            {
                if (pSwitch == null)
                {
                    continue;
                }

                _switchUnlockMethod?.Invoke(pSwitch, null);
            }

            foreach (var lamp in _lamp)
            {
                if (lamp == null)
                {
                    continue;
                }

                lamp.Switch(Turnable.EState.On);
                lamp.enabled = true;
            }

            NotificationHelper.Show(
                "Blackout Event over",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Green
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Blackout Event has run");
            }
        }

        public void DoSkillEvent()
        {
            if (_skillEventCount >= 3)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Skill);
            }

            var chance = SharedRandom.Next(0, 101);
            var selectedSkill = ROSkillManager.DisplayList.RandomElement();
            var level = selectedSkill.Level;

            if (selectedSkill.Locked)
            {
                DoSkillEvent();
                return;
            }

            if (chance >= 0 && chance <= 55)
            {
                if (level > 50 || level < 0)
                {
                    return;
                }

                selectedSkill.SetLevel(level + 1);
                _skillEventCount++;
                NotificationHelper.Show(
                    "Skill Event: You've advanced a skill to the next level!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Gold
                );
                EventsEffectsController.Instance?.ShowPositiveEffect();
            }
            else
            {
                if (level <= 0)
                {
                    return;
                }

                selectedSkill.SetLevel(level - 1);
                _skillEventCount++;
                NotificationHelper.Show(
                    "Skill Event: You've lost a skill level, unlucky!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Red
                );
                EventsEffectsController.Instance?.ShowNegativeEffect();
            }

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Skill Event has run");
            }
        }

        public void DoMetabolismEvent()
        {
            if (!_metabolismDisabled)
            {
                if (FikaBridge.AmHost())
                {
                    FikaBridge.SendRandomEventPacket(Utils.Metabolism);
                }

                var chance = SharedRandom.Next(0, 101);

                if (chance >= 0 && chance <= 33)
                {
                    ROPlayer.ActiveHealthController.DisableMetabolism();
                    _metabolismDisabled = true;
                    NotificationHelper.Show(
                        "Metabolism Event: You've got an iron stomach, No hunger or hydration drain!",
                        NotificationHelper.NotificationLength.Long,
                        NotificationHelper.NotificationColor.Gold
                    );
                    EventsEffectsController.Instance?.ShowPositiveEffect();
                }
                else if (chance >= 34 && chance <= 66)
                {
                    AccessTools
                        .Property(typeof(ActiveHealthController), nameof(ActiveHealthController.EnergyRate))
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.EnergyRate * 0.80f);

                    AccessTools
                        .Property(typeof(ActiveHealthController), nameof(ActiveHealthController.HydrationRate))
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.HydrationRate * 0.80f);

                    NotificationHelper.Show(
                        "Metabolism Event: Your metabolism has slowed. Decreased hunger and hydration drain!",
                        NotificationHelper.NotificationLength.Long,
                        NotificationHelper.NotificationColor.White
                    );
                    EventsEffectsController.Instance?.ShowPositiveEffect();
                }
                else if (chance >= 67 && chance <= 100)
                {
                    AccessTools
                        .Property(typeof(ActiveHealthController), nameof(ActiveHealthController.EnergyRate))
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.EnergyRate * 1.20f);

                    AccessTools
                        .Property(typeof(ActiveHealthController), nameof(ActiveHealthController.HydrationRate))
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.HydrationRate * 1.20f);

                    NotificationHelper.Show(
                        "Metabolism Event: Your metabolism has fastened. Increased hunger and hydration drain!",
                        NotificationHelper.NotificationLength.Long,
                        NotificationHelper.NotificationColor.Red
                    );
                    EventsEffectsController.Instance?.ShowNegativeEffect();
                }
            }

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Metabolism Event has run");
            }
        }

        private void TryTriggerMalfunction(Player player)
        {
            var firearmController = player.HandsController as Player.FirearmController;
            if (firearmController == null)
            {
                return;
            }

            if (firearmController.Item is not Weapon weapon || !weapon.AllowMalfunction)
            {
                return;
            }

            if (weapon.MalfState.State != Weapon.EMalfunctionState.None)
            {
                return;
            }

            if (weapon.FirstLoadedChamberSlot?.ContainedItem == null)
            {
                return;
            }

            var repairable = weapon.GetItemComponent<RepairableComponent>();
            if (repairable == null)
            {
                return;
            }

            var original = repairable.Durability;
            repairable.Durability = 1f;
            RestoreDurabilityAfterDelay(repairable, original).Forget();
        }

        private async UniTaskVoid RestoreDurabilityAfterDelay(RepairableComponent repairable, float original)
        {
            await UniTask.WaitForSeconds(30f);
            repairable.Durability = original;
        }

        internal async UniTask DoMalfEvent()
        {
            if (_malfEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Malf);
            }

            _malfEventHasRun = true;

            var player = ROPlayer;
            if (player == null)
            {
                return;
            }

            NotificationHelper.Show(
                "Malfunction Event: Be careful not to jam up!",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Red
            );
            EventsEffectsController.Instance?.ShowNegativeEffect();

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Malfunction Event has started");
            }

            var elapsed = 0;
            var duration = 300;

            while (elapsed < duration)
            {
                TryTriggerMalfunction(player);
                await UniTask.WaitForSeconds(60);
                elapsed += 60;
            }

            TryTriggerMalfunction(player);

            NotificationHelper.Show(
                "Malfunction Event: Your weapon should be working properly again!",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Green
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Malfunction Event has run");
            }
        }

        internal async UniTask DoBerserkEvent()
        {
            if (_berserkEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Berserk);
            }

            _berserkEventHasRun = true;

            var player = ROPlayer;
            if (player == null)
            {
                return;
            }

            var strengthSnapshot = player.Skills.Strength.Current;
            var aimDrillsSnapshot = player.Skills.AimDrills.Current;

            player.ActiveHealthController.DoScavRegeneration(10f);
            player.ActiveHealthController.DoExternalBuff("BerserkBuff");
            player.Skills.Strength.SetCurrent(5100f);
            player.Skills.AimDrills.SetCurrent(5100f);

            NotificationHelper.Show(
                "Berserk Event: You're seeing red, I feel bad for any scavs and PMCs in your way!",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Red
            );
            EventsEffectsController.Instance?.ShowPositiveEffect();

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Berserk Event has started");
            }

            await UniTask.WaitForSeconds(180);

            player.ActiveHealthController.DoScavRegeneration(0);
            player.ActiveHealthController.PauseAllEffects();
            player.Skills.Strength.SetCurrent(strengthSnapshot, true);
            player.Skills.AimDrills.SetCurrent(aimDrillsSnapshot, true);

            NotificationHelper.Show(
                "Berserk Event: Your vision has cleared up, I guess you got all your rage out!",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Green
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Berserk Event has run");
            }
        }

        internal async UniTask DoWeightEvent()
        {
            if (_weightEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            var chance = SharedRandom.Next(0, 101);

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Weight);
            }

            _weightEventHasRun = true;

            var player = ROPlayer;
            if (player == null)
            {
                return;
            }

            if (chance >= 0 && chance <= 49)
            {
                player.Physical.BaseOverweightLimits *= 0.5f;
                player.Physical.WalkOverweightLimits *= 0.5f;
                player.Physical.SprintOverweightLimits *= 0.5f;
                player.Physical.WalkSpeedOverweightLimits *= 0.5f;
                player.Physical.OnWeightUpdated();

                NotificationHelper.Show(
                    "Weight Event: Better hunker down until you get your stamina back!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Red
                );
                EventsEffectsController.Instance?.ShowNegativeEffect();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has started");
                }

                await UniTask.WaitForSeconds(180);

                player.Physical.OnWeightLimitsUpdated();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has run");
                }

                NotificationHelper.Show(
                    "Weight Event: You're rested and ready to get back out there!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Green
                );
            }
            else if (chance >= 50 && chance <= 100)
            {
                player.Physical.BaseOverweightLimits *= 2.0f;
                player.Physical.WalkOverweightLimits *= 2.0f;
                player.Physical.SprintOverweightLimits *= 2.0f;
                player.Physical.WalkSpeedOverweightLimits *= 2.0f;
                player.Physical.OnWeightUpdated();

                NotificationHelper.Show(
                    "Weight Event: You feel light on your feet, stock up on everything you can!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Gold
                );
                EventsEffectsController.Instance?.ShowPositiveEffect();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has started");
                }

                await UniTask.WaitForSeconds(180);

                player.Physical.OnWeightLimitsUpdated();

                NotificationHelper.Show(
                    "Weight Event: You've lost your extra energy, hope you didn't fill your backpack too much!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.White
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has run");
                }
            }
        }

        public void DoLLEvent()
        {
            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.LoyaltyLevel);
            }

            int chance = SharedRandom.Next(0, 101);
            var traders = ConfigController.ServerConfig.EnableRequisitionOffice ? Utils.Traders : Utils.TradersNoReq;
            var trader = traders.RandomElement();

            string traderName = trader.Key ?? "A trader";

            var session = GetSession();
            if (chance < 50)
            {
                session?.Profile.TradersInfo[trader.Value].SetStanding(session.Profile.TradersInfo[trader.Value].Standing + 0.1);
                NotificationHelper.Show(
                    $"Trader Event: {traderName} has gained a little more respect for you.",
                    NotificationHelper.NotificationLength.Short,
                    NotificationHelper.NotificationColor.Gold
                );
                EventsEffectsController.Instance?.ShowPositiveEffect();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Trader Rep Gain Event has run");
                }
            }
            else
            {
                if (session?.Profile.TradersInfo[trader.Value].Standing >= 0.05)
                {
                    session?.Profile.TradersInfo[trader.Value].SetStanding(session.Profile.TradersInfo[trader.Value].Standing - 0.05);
                    NotificationHelper.Show(
                        $"Trader Event: {traderName} has lost a little faith in you.",
                        NotificationHelper.NotificationLength.Short,
                        NotificationHelper.NotificationColor.Red
                    );
                    EventsEffectsController.Instance?.ShowNegativeEffect();

                    if (ConfigController.DebugConfig.DebugMode)
                    {
                        Utils.LogToServerConsole("Trader Rep Loss Event has run");
                    }
                }
                else
                {
                    DoLLEvent();
                }
            }
        }

        public void DoMaxLLEvent()
        {
            var profileId = ROPlayer.ProfileId;

            if (JsonHandler.CheckFilePath(profileId, "Flags"))
            {
                ConfigController.ProfileFlags = JsonHandler.ReadFlagFile<ProfileFlags>(profileId, "Flags");

                if (FikaBridge.AmHost())
                {
                    FikaBridge.SendRandomEventPacket(Utils.MaxLoyaltyLevel);
                }

                if (!ConfigController.ProfileFlags.TraderRepFlag)
                {
                    if (_maxLLEventCount >= 1)
                    {
                        Weighting.DoRandomEvent(Weighting.WeightedEvents);
                        return;
                    }

                    var traders = ConfigController.ServerConfig.EnableRequisitionOffice ? Utils.Traders : Utils.TradersNoReq;

                    _maxLLEventCount++;

                    var session = GetSession();
                    foreach (var trader in traders)
                    {
                        session?.Profile.TradersInfo[trader.Value].SetStanding(session.Profile.TradersInfo[trader.Value].Standing + 1);
                    }

                    ConfigController.ProfileFlags.TraderRepFlag = true;
                    JsonHandler.SaveToJson(ConfigController.ProfileFlags, profileId, "Flags");

                    NotificationHelper.Show(
                        "Shopping Spree Event: All Traders have maxed out standing. Better get to them in the next ten minutes!",
                        NotificationHelper.NotificationLength.Short,
                        NotificationHelper.NotificationColor.Gold
                    );
                    EventsEffectsController.Instance?.ShowPositiveEffect();

                    if (ConfigController.DebugConfig.DebugMode)
                    {
                        Utils.LogToServerConsole("Shopping Spree Event has run");
                    }
                }
                else if (ConfigController.ProfileFlags.TraderRepFlag)
                {
                    CorrectRep();
                    ConfigController.ProfileFlags.TraderRepFlag = false;
                    JsonHandler.SaveToJson(ConfigController.ProfileFlags, profileId, "Flags");
                }
            }
            else
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
        }

        public void CorrectRep()
        {
            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.CorrectRep);
            }

            var traders = ConfigController.ServerConfig.EnableRequisitionOffice ? Utils.Traders : Utils.TradersNoReq;
            var session = GetSession();

            foreach (var trader in traders)
            {
                session?.Profile.TradersInfo[trader.Value].SetStanding(session.Profile.TradersInfo[trader.Value].Standing - 1);
            }
        }

        internal async UniTask DoLockDownEvent()
        {
            var raidTimeLeft = SPT.SinglePlayer.Utils.InRaid.RaidTimeUtil.GetRemainingRaidSeconds();

            if (_exfilEventCount >= 1)
            {
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Lockdown);
            }

            if (raidTimeLeft < 900 || ROPlayer.Location.ToLowerInvariant() == "laboratory")
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
            else
            {
                var exfils = FindObjectsOfType<ExfiltrationPoint>();

                NotificationHelper.Show(
                    "Lockdown Event: All extracts are unavailable for 15 minutes",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Blue
                );
                EventsEffectsController.Instance?.ShowNegativeEffect();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Lockdown Event has started");
                }

                _exfilLockdown = true;

                foreach (var exfil in exfils)
                {
                    if (!exfil.Settings.Name.Contains("Elevator"))
                    {
                        exfil.Disable();
                    }
                }
                _exfilEventCount++;

                await UniTask.WaitForSeconds(600);

                foreach (var exfil in exfils)
                {
                    if (!exfil.Settings.Name.Contains("Elevator"))
                    {
                        exfil.Enable();
                    }
                }

                _exfilLockdown = false;

                NotificationHelper.Show(
                    "Lockdown Event: Extracts are available again. Time to get out of there!",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Green
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Lockdown Event has run");
                }
            }
        }

        internal async UniTask DoArtyEvent()
        {
            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Artillery);
            }

            if (!_invalidArtilleryLocations.Contains(ROPlayer.Location.ToLowerInvariant()) && !_artyEventHasRun)
            {
                _artyEventHasRun = true;

                NotificationHelper.Show(
                    "Artillery Event: Get to cover. Shelling will commence in 30 seconds",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Red
                );
                EventsEffectsController.Instance?.ShowNegativeEffect();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Artillery Event has started");
                }

                await UniTask.WaitForSeconds(30);

                NotificationHelper.Show(
                    "Artillery Event: Shelling has started",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Red
                );

                ROGameWorld.ServerShellingController?.StartShellingPosition(ROPlayer.Transform.position);
            }
            else
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
        }

        internal async UniTask RunTrain()
        {
            FikaBridge.SendFlareEventPacket(Utils.Train);

            await UniTask.WaitForSeconds(3);
            var trainExfil = FindObjectOfType<Locomotive>();
            if (trainExfil == null)
            {
                return;
            }

            trainExfil.Init(System.DateTime.UtcNow);

            NotificationHelper.Show(
                "Train is arriving. Get out if you're ready!",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Blue
            );
            EventsEffectsController.Instance?.ShowPositiveEffect();

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Train is arriving");
            }

            await UniTask.WaitForSeconds(420);

            NotificationHelper.Show(
                "Train is leaving the station.",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Blue
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Train is leaving");
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            MonoBehaviourSingleton<BetterAudio>.Instance.PlayAtPoint(
                ROPlayer.Transform.position,
                clip,
                BetterAudio.AudioSourceGroupType.Environment,
                0,
                1f,
                EOcclusionTest.None,
                null,
                false
            );
        }

        private void SetPlayersHunted(bool hunted)
        {
            var gameWorld = ROGameWorld;
            if (gameWorld == null)
            {
                return;
            }

            var processedGroups = new HashSet<BotsGroup>();

            foreach (var registeredPlayer in gameWorld.RegisteredPlayers)
            {
                if (registeredPlayer == null || !registeredPlayer.IsAI || !registeredPlayer.HealthController.IsAlive)
                {
                    continue;
                }

                var botOwner = registeredPlayer.AIData?.BotOwner;
                if (botOwner == null)
                {
                    continue;
                }

                var role = botOwner.Profile?.Info?.Settings?.Role;
                if (role != null && WildSpawnTypeExtensions.IsWolf(role.Value))
                {
                    continue;
                }

                if (processedGroups.Add(botOwner.BotsGroup))
                {
                    foreach (var humanPlayer in gameWorld.RegisteredPlayers)
                    {
                        if (humanPlayer == null || humanPlayer.IsAI || !humanPlayer.HealthController.IsAlive)
                        {
                            continue;
                        }

                        if (hunted)
                        {
                            botOwner.BotsGroup.AddEnemy(humanPlayer, EBotEnemyCause.addPlayer);
                        }
                        else
                        {
                            botOwner.BotsGroup.RemoveEnemy(humanPlayer);
                        }
                    }
                }

                if (BotManagerComponent.Instance == null || !BotManagerComponent.Instance.GetSAIN(botOwner, out var sainBot))
                {
                    continue;
                }

                foreach (var humanPlayer in gameWorld.RegisteredPlayers)
                {
                    if (humanPlayer == null || humanPlayer.IsAI || !humanPlayer.HealthController.IsAlive)
                    {
                        continue;
                    }

                    if (hunted)
                    {
                        var sainEnemy = sainBot.EnemyController.CheckAddEnemy(humanPlayer);
                        sainEnemy?.KnownPlaces.UpdateSeenPlace(humanPlayer.Transform.position, Time.time);
                    }
                    else
                    {
                        sainBot.EnemyController.RemoveEnemy(humanPlayer.ProfileId);
                    }
                }
            }
        }

        private async UniTaskVoid EnforceHuntedState()
        {
            while (_huntedEventRunning)
            {
                await UniTask.WaitForSeconds(10);
                if (_huntedEventRunning)
                {
                    SetPlayersHunted(true);
                }
            }
        }

        internal async UniTask DoHuntedEvent()
        {
            if (_huntedEventRunning || _huntedEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Hunted);
            }

            _huntedEventRunning = true;
            NotificationHelper.Show(
                "You have been marked. Every hostile in the raid is now hunting you down.",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Red
            );
            EventsEffectsController.Instance?.ShowHuntedInvasionEffect();
            PlaySound(SoundHeartbeat);
            SetPlayersHunted(true);
            EnforceHuntedState().Forget();

            await UniTask.WaitForSeconds(300);

            SetPlayersHunted(false);
            _huntedEventRunning = false;
            _huntedEventHasRun = true;
        }

        internal async UniTask DoPmcExfilEvent()
        {
            if (!_pmcExfilEventRunning)
            {
                FikaBridge.SendFlareEventPacket(Utils.PmcExfil);

                _pmcExfilEventRunning = true;

                await UniTask.WaitForSeconds(3);
                NotificationHelper.Show(
                    "Extract is on it's way! Hold out for two minutes for help to arrive",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Blue
                );
                EventsEffectsController.Instance?.ShowPositiveEffect();
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Extract event has started");
                }
                NotificationHelper.Show(
                    "All hostiles are converging on your position! Survive until extraction arrives.",
                    NotificationHelper.NotificationLength.Long,
                    NotificationHelper.NotificationColor.Red
                );
                if (FikaBridge.AmHost())
                {
                    _huntedEventRunning = true;
                    SetPlayersHunted(true);
                    EnforceHuntedState().Forget();
                }
                await UniTask.WaitForSeconds(120);

                for (int i = 10; i >= 1; i--)
                {
                    NotificationHelper.Show(
                        i.ToString(),
                        NotificationHelper.NotificationLength.Tick,
                        NotificationHelper.NotificationColor.Blue
                    );
                    await UniTask.WaitForSeconds(1);
                }
                NotificationHelper.Show(
                    "Help has arrived",
                    NotificationHelper.NotificationLength.Short,
                    NotificationHelper.NotificationColor.Green
                );

                var exfilSession = Singleton<AbstractGame>.Instance as EndByExitTrigerScenario.GInterface146;
                string profileId = GamePlayerOwner.MyPlayer.ProfileId;
                string exitName = Singleton<GameWorld>.Instance.ExfiltrationController.ExfiltrationPoints.FirstOrDefault().name;

                if (FikaBridge.AmHost())
                {
                    SetPlayersHunted(false);
                    _huntedEventRunning = false;
                }
                exfilSession.StopSession(profileId, ExitStatus.Survived, exitName);

                _pmcExfilEventRunning = false;
            }
        }

        public void ExfilNow()
        {
            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.ExfilNow);
            }

            var exfilSession = Singleton<AbstractGame>.Instance as EndByExitTrigerScenario.GInterface146;
            string profileId = GamePlayerOwner.MyPlayer.ProfileId;
            string exitName = Singleton<GameWorld>.Instance.ExfiltrationController.ExfiltrationPoints.FirstOrDefault().name;

            exfilSession.StopSession(profileId, ExitStatus.Survived, exitName);
        }

        internal void StartInvasion()
        {
            if (_invasionHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            InvasionController.StartInvasion();
            _invasionHasRun = true;

            NotificationHelper.Show(
                "Invasion Event: Bosses incoming.",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Red
            );
            EventsEffectsController.Instance?.ShowHuntedInvasionEffect();
            PlaySound(SoundAlarm);

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Invasion Event has started");
            }
        }

        internal async UniTask SpawnLegionForQuest()
        {
            if (_legionSpawnedForQuest)
            {
                return;
            }

            if (!_legionQuestMapZones.TryGetValue(ROPlayer.Location.ToLowerInvariant(), out var zones))
            {
                return;
            }

            _legionSpawnedForQuest = true;

            BossInvasionConfig legionConfig = new BossInvasionConfig
            {
                BossName = "bosslegion",
                BossEscorts = "legionnaire",
                BossType = (WildSpawnType)199,
                BossEscortType = (WildSpawnType)200,
                BossEscortCount = 4,
                AdditionalSupports = null,
            };

            string zone = zones[SharedRandom.Next(zones.Length)];

            await UniTask.WaitForSeconds(60);
            if (Utils.IsInRaid())
            {
                Utils.SpawnBoss(legionConfig, zone);
                EventsEffectsController.Instance?.ShowLegionQuestEffect();
            }
        }

        public void CleanForNewEvent()
        {
            _pswitchs = null;
            _keydoor = null;
            _lamp = null;
        }

        private void CheckForFlags()
        {
            var profileId = ROPlayer.ProfileId;

            if (!JsonHandler.CheckFilePath(profileId, "Flags"))
            {
                JsonHandler.CreateFlagFile<ProfileFlags>(profileId, "Flags");
            }

            ConfigController.ProfileFlags = JsonHandler.ReadFlagFile<ProfileFlags>(profileId, "Flags");
            bool dirty = false;

            if (ConfigController.ProfileFlags.TraderRepFlag)
            {
                CorrectRep();
                ConfigController.ProfileFlags.TraderRepFlag = false;
                dirty = true;
            }

            if (ConfigController.ServerConfig.EnableCustomBoss && !ConfigController.ProfileFlags.UnlockQuestCompleted)
            {
                var quest = Utils.GetQuest(ROQuestController, "66f0eb2c12fb0ed12fbcfd46");
                if (quest != null)
                {
                    if (
                        quest.QuestStatus == EQuestStatus.Started
                        && quest.QuestStatus != EQuestStatus.AvailableForFinish
                        && quest.QuestStatus != EQuestStatus.Success
                    )
                    {
                        SpawnLegionForQuest().Forget();
                    }
                    else if (quest.QuestStatus == EQuestStatus.Success)
                    {
                        ConfigController.ProfileFlags.UnlockQuestCompleted = true;
                        dirty = true;
                    }
                }
            }

            if (dirty)
            {
                JsonHandler.SaveToJson(ConfigController.ProfileFlags, profileId, "Flags");
            }
        }

        public void DoBlackoutEventWrapper()
        {
            DoBlackoutEvent().Forget();
        }

        public void DoFunnyWrapper()
        {
            DoFunny().Forget();
        }

        public void DoMalfEventWrapper()
        {
            DoMalfEvent().Forget();
        }

        public void DoBerserkEventWrapper()
        {
            DoBerserkEvent().Forget();
        }

        public void DoWeightEventWrapper()
        {
            DoWeightEvent().Forget();
        }

        public void DoLockDownEventWrapper()
        {
            DoLockDownEvent().Forget();
        }

        public void DoArtyEventWrapper()
        {
            DoArtyEvent().Forget();
        }

        public void RunTrainWrapper()
        {
            RunTrain().Forget();
        }

        public void DoPmcExfilEventWrapper()
        {
            DoPmcExfilEvent().Forget();
        }

        public void DoHuntedEventWrapper()
        {
            DoHuntedEvent().Forget();
        }
        #endregion

        #region Body Cleanup & Corpse Airdrop

        private async UniTaskVoid RunBodyCleanupCycle()
        {
            while (Utils.IsInRaid())
            {
                await UniTask.WaitForSeconds(ROPluginConfig.TimeToClean.Value * 60f);

                if (!Utils.IsInRaid())
                {
                    break;
                }

                await ExecuteCleanupAndAirdrop();
            }
        }

        private async UniTask ExecuteCleanupAndAirdrop()
        {
            NotificationHelper.Show(
                "Commencing Body Cleanup",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.White
            );
            EventsEffectsController.Instance?.ShowCleanupEffect();

            for (int i = 10; i >= 1; i--)
            {
                NotificationHelper.Show(
                    i.ToString(),
                    NotificationHelper.NotificationLength.Tick,
                    NotificationHelper.NotificationColor.White
                );
                await UniTask.WaitForSeconds(1);
            }

            PendingCorpseItems.Clear();

            var bots = FindObjectsOfType<BotOwner>();
            if (bots == null)
            {
                return;
            }

            foreach (var bot in bots)
            {
                if (bot == null || bot.HealthController == null || bot.HealthController.IsAlive)
                {
                    continue;
                }

                var equipment = bot.GetPlayer?.Profile?.Inventory?.Equipment;
                if (equipment == null)
                {
                    continue;
                }

                foreach (var slot in _allEquipmentSlots)
                {
                    if (!ShouldCollectSlot(slot))
                    {
                        continue;
                    }

                    var item = equipment.GetSlot(slot)?.ContainedItem;
                    if (item != null)
                    {
                        PendingCorpseItems.Add(item);
                    }
                }
            }

            DeactivateDeadBots(bots);

            PendingCorpseItems.Sort((a, b) => GetItemTotalValue(b).CompareTo(GetItemTotalValue(a)));

            if (!_invalidAirdropLocations.Contains(ROPlayer.Location.ToLowerInvariant()) && PendingCorpseItems.Count > 0)
            {
                NotificationHelper.Show(
                    $"Items found: {PendingCorpseItems.Count}",
                    NotificationHelper.NotificationLength.Medium,
                    NotificationHelper.NotificationColor.Gold
                );
                Utils.LogToServerConsole($"Items found: {PendingCorpseItems.Count}");
                await DispatchCorpseAirdrops();
            }
        }

        private async UniTask DispatchCorpseAirdrops()
        {
            int capacity = AirdropCrateCapacity > 0 ? AirdropCrateCapacity : 120;
            int totalCells = 0;
            foreach (var pending in PendingCorpseItems)
            {
                totalCells += pending.Template.Width * pending.Template.Height;
            }

            int effectiveCapacity = System.Math.Max(1, (int)(capacity * 0.8));
            var mapPointCount = LocationScene.GetAll<AirdropPoint>().Count();
            int pointCap = mapPointCount > 1 ? System.Math.Min(5, mapPointCount - 1) : 1;
            int airdropCount = System.Math.Min(
                pointCap,
                System.Math.Max(1, (int)System.Math.Ceiling((double)totalCells / effectiveCapacity))
            );
            _airdropEventHasRun = true;

            NotificationHelper.Show(
                $"Corpse Cleanup: Bodies cleared. {airdropCount} airdrop(s) inbound with salvaged gear.",
                NotificationHelper.NotificationLength.Long,
                NotificationHelper.NotificationColor.Gold
            );
            Utils.LogToServerConsole($"Corpse Cleanup: Bodies cleared. {airdropCount} airdrop(s) inbound with salvaged gear.");

            for (int i = 0; i < airdropCount; i++)
            {
                ROGameWorld.InitAirdrop(null, true, ROPlayer.Transform.position);

                if (i < airdropCount - 1)
                {
                    await UniTask.WaitForSeconds(15f);
                }
            }
        }

        private static double GetItemTotalValue(Item item)
        {
            if (item?.Template == null)
            {
                return 0;
            }

            double total = (double)item.Template.CreditsPrice * item.StackObjectsCount;

            if (item is CompoundItem compound)
            {
                foreach (var slot in compound.Slots)
                {
                    if (slot.ContainedItem != null)
                    {
                        total += GetItemTotalValue(slot.ContainedItem);
                    }
                }

                foreach (var grid in compound.Grids)
                {
                    foreach (var gridItem in grid.Items)
                    {
                        total += GetItemTotalValue(gridItem);
                    }
                }
            }

            return total;
        }

        private static void DeactivateDeadBots(BotOwner[] bots)
        {
            float distSqThreshold = 100f;
            var playerPos = ROPlayer.Transform.position;

            foreach (var bot in bots)
            {
                if (bot == null || bot.HealthController == null || bot.HealthController.IsAlive)
                {
                    continue;
                }

                if ((playerPos - bot.Transform.position).sqrMagnitude >= distSqThreshold)
                {
                    if (bot.LeaveData != null)
                    {
                        bot.LeaveData.RemoveFromMap();
                    }
                    else
                    {
                        bot.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void TriggerManualCleanup()
        {
            if (!Utils.IsInRaid())
            {
                return;
            }

            ExecuteCleanupAndAirdrop().Forget();
        }

        #endregion
    }
}
