using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Comfort.Common;
using CommonAssets.Scripts.Game;
using Cysharp.Threading.Tasks;
using EFT;
using EFT.Communications;
using EFT.HealthSystem;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.MovingPlatforms;
using EFT.UI;
using EFT.UI.BattleTimer;
using EFT.UI.Matchmaker;
using HarmonyLib;
using JsonType;
using RaidOverhaul.Configs;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using RaidOverhaul.Patches;
using UnityEngine;
using UnityEngine.UI;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class EventController : MonoBehaviour
    {
        private static bool _pmcExfilEventRunning;
        private static bool _eventIsRunning;
        public static bool ExfilLockdown;
        public static bool GearExfil;
        private bool _metabolismDisabled;
        private bool _jokeEventHasRun;
        private bool _airdropEventHasRun;
        private bool _berserkEventHasRun;
        private bool _malfEventHasRun;
        private bool _weightEventHasRun;
        private bool _artyEventHasRun;
        private bool _blackoutEventHasRun;
        internal bool _invasionHasRun;
        private bool _hasInitializedReflection;
        private bool _isReady;
        private bool _exfilUIChanged;

        private int _skillEventCount;
        private int _repairEventCount;
        private int _healthEventCount;
        private int _damageEventCount;
        private int _maxLLEventCount;
        private int _exfilEventCount;
        public static int TimeStart;

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

        private class OriginalWeaponStatsBers
        {
            public float MalfChance;
            public float DuraBurn;
            public float Ergo;
            public float RecoilBack;
            public float RecoilUp;
        }

        private class OriginalWeaponStatsMalf
        {
            public float MalfChance;
            public float DuraBurn;
            public float Ergo;
        }

        private static readonly HashSet<string> InvalidAirdropLocations = new HashSet<string>
        {
            "factory4_day",
            "factory4_night",
            "laboratory",
            "sandbox",
            "sandbox_high",
        };

        private static readonly HashSet<string> InvalidArtilleryLocations = new HashSet<string>
        {
            "factory4_day",
            "factory4_night",
            "laboratory",
        };

        private Dictionary<string, OriginalWeaponStatsBers> _originalWSBers = new Dictionary<string, OriginalWeaponStatsBers>();
        private Dictionary<string, OriginalWeaponStatsMalf> _originalWSMalf = new Dictionary<string, OriginalWeaponStatsMalf>();

        private void Awake()
        {
            if (!_hasInitializedReflection)
            {
                InitializeDoorReflection();
                InitializeTimeChangeReflection();
                _hasInitializedReflection = true;
            }

            CheckForFlag();
        }

        private void InitializeDoorReflection()
        {
            _switchCloseMethod = typeof(Switch).GetMethod("Close", BindingFlags.Instance | BindingFlags.Public);
            _switchLockMethod = typeof(Switch).GetMethod("Lock", BindingFlags.Instance | BindingFlags.Public);
            _switchUnlockMethod = typeof(Switch).GetMethod("Unlock", BindingFlags.Instance | BindingFlags.Public);
            _keycardUnlockMethod = typeof(KeycardDoor).GetMethod("Unlock", BindingFlags.Instance | BindingFlags.Public);
            _keycardOpenMethod = typeof(KeycardDoor).GetMethod("Open", BindingFlags.Instance | BindingFlags.Public);
        }

        private void InitializeTimeChangeReflection()
        {
            _edateTimeField = typeof(MatchMakerSelectionLocationScreen).GetField(
                "edateTime_0",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
        }

        public void ManualUpdate()
        {
            _isReady = Utils.IsInRaid();

            if (!_isReady || !DJConfig.EnableEvents.Value)
            {
                ResetEventState();
                return;
            }

            if (DJConfig.TimeChanges.Value)
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

                if (!_eventIsRunning)
                {
                    StartEvents().Forget();
                    _eventIsRunning = true;
                }
            }

            if (EventExfilPatch._isLockdown && !_exfilUIChanged)
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
            _skillEventCount = 0;
            _repairEventCount = 0;
            _healthEventCount = 0;
            _damageEventCount = 0;
            _maxLLEventCount = 0;
            _exfilEventCount = 0;
            _cachedExtractionPanel = null;
            _cachedExitPanels = null;
            _blackoutEventHasRun = false;
            _invasionHasRun = false;
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
            if (EventExfilPatch._isLockdown)
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
                    typeof(ExtractionTimersPanel)
                        .GetField("_mainDescription", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(_cachedExtractionPanel);

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

                await UniTask.WaitUntil(() => !EventExfilPatch._isLockdown);

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

            NotificationManagerClass.DisplayMessageNotification(
                "Heal Event: On your feet you ain't dead yet.",
                ENotificationDurationType.Long
            );
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

            NotificationManagerClass.DisplayMessageNotification(
                "Heart Attack Event: Better get to a medic quick, you don't have long left.",
                ENotificationDurationType.Long,
                ENotificationIconType.Alert
            );
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

            NotificationManagerClass.DisplayMessageNotification(
                "Armor Repair Event: All equipped armor repaired... nice!",
                ENotificationDurationType.Long
            );
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
                        _repairEventCount++;
                    }
                );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Armor Repair Event has run");
            }
        }

        public void DoAirdropEvent()
        {
            if (!InvalidAirdropLocations.Contains(ROPlayer.Location) && !_airdropEventHasRun)
            {
                if (Utils.FindTemplates(Utils.RedFlare).FirstOrDefault() is not AmmoTemplate ammoTemplate)
                {
                    return;
                }

                ROPlayer.HandleFlareSuccessEvent(ROPlayer.Transform.position, ammoTemplate);

                NotificationManagerClass.DisplayMessageNotification(
                    "Aidrop Event: Incoming Airdrop!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Quest
                );

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

                NotificationManagerClass.DisplayMessageNotification(
                    "Heart Attack Event: Nice knowing ya, you've got 10 seconds",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Alert
                );

                await UniTask.WaitForSeconds(10);

                NotificationManagerClass.DisplayMessageNotification("jk", ENotificationDurationType.Long, ENotificationIconType.Quest);

                await UniTask.WaitForSeconds(2);

                DoHealPlayer();

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

            NotificationManagerClass.DisplayMessageNotification(
                "Blackout Event: All power switches and lights disabled for 10 minutes",
                ENotificationDurationType.Long,
                ENotificationIconType.Alert
            );

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

            NotificationManagerClass.DisplayMessageNotification(
                "Blackout Event over",
                ENotificationDurationType.Long,
                ENotificationIconType.Quest
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
            }

            if (chance >= 0 && chance <= 55)
            {
                if (level > 50 || level < 0)
                {
                    return;
                }

                selectedSkill.SetLevel(level + 1);
                _skillEventCount++;
                NotificationManagerClass.DisplayMessageNotification(
                    "Skill Event: You've advanced a skill to the next level!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Quest
                );
            }
            else
            {
                if (level <= 0)
                {
                    return;
                }

                selectedSkill.SetLevel(level - 1);
                _skillEventCount++;
                NotificationManagerClass.DisplayMessageNotification(
                    "Skill Event: You've lost a skill level, unlucky!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Quest
                );
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
                    NotificationManagerClass.DisplayMessageNotification(
                        "Metabolism Event: You've got an iron stomach, No hunger or hydration drain!",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                }
                else if (chance >= 34 && chance <= 66)
                {
                    AccessTools
                        .Property(typeof(ActiveHealthController), "EnergyRate")
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.EnergyRate * 0.80f);

                    AccessTools
                        .Property(typeof(ActiveHealthController), "HydrationRate")
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.HydrationRate * 0.80f);

                    NotificationManagerClass.DisplayMessageNotification(
                        "Metabolism Event: Your metabolism has slowed. Decreased hunger and hydration drain!",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                }
                else if (chance >= 67 && chance <= 100)
                {
                    AccessTools
                        .Property(typeof(ActiveHealthController), "EnergyRate")
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.EnergyRate * 1.20f);

                    AccessTools
                        .Property(typeof(ActiveHealthController), "HydrationRate")
                        .SetValue(ROPlayer.ActiveHealthController, ROPlayer.ActiveHealthController.HydrationRate * 1.20f);

                    NotificationManagerClass.DisplayMessageNotification(
                        "Metabolism Event: Your metabolism has fastened. Increased hunger and hydration drain!",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                }
            }

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Metabolism Event has run");
            }
        }

        internal async UniTask DoMalfEvent()
        {
            if (_malfEventHasRun)
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
                return;
            }

            var items = _session.Profile.Inventory.GetItemsInSlots(
                new EquipmentSlot[] { EquipmentSlot.FirstPrimaryWeapon, EquipmentSlot.SecondPrimaryWeapon }
            );

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Malf);
            }

            _malfEventHasRun = true;
            _originalWSMalf.Clear();

            foreach (var item in items)
            {
                if (item is not Weapon weapon)
                {
                    continue;
                }

                var template = weapon.Template;
                var templateId = item.TemplateId;

                if (!_originalWSMalf.ContainsKey(templateId))
                {
                    _originalWSMalf[templateId] = new OriginalWeaponStatsMalf
                    {
                        MalfChance = template.BaseMalfunctionChance,
                        DuraBurn = template.DurabilityBurnRatio,
                        Ergo = template.Ergonomics,
                    };
                }

                var origStats = _originalWSMalf[templateId];
                template.BaseMalfunctionChance = origStats.MalfChance * 3f;
                template.DurabilityBurnRatio = origStats.DuraBurn * 2f;
                template.Ergonomics = origStats.Ergo * 0.5f;
            }

            NotificationManagerClass.DisplayMessageNotification(
                "Malfunction Event: Be careful not to jam up!",
                ENotificationDurationType.Long,
                ENotificationIconType.Alert
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Malfunction Event has started");
            }

            await UniTask.WaitForSeconds(300);

            foreach (var item in items)
            {
                if (item is not Weapon weapon)
                {
                    continue;
                }

                if (_originalWSMalf.TryGetValue(item.TemplateId, out var origStats))
                {
                    var template = weapon.Template;
                    template.BaseMalfunctionChance = origStats.MalfChance;
                    template.DurabilityBurnRatio = origStats.DuraBurn;
                    template.Ergonomics = origStats.Ergo;
                }
            }

            NotificationManagerClass.DisplayMessageNotification(
                "Malfunction Event: Your weapon has had time to cool off, shouldn't have any more troubles!",
                ENotificationDurationType.Long
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

            var items = _session.Profile.Inventory.GetItemsInSlots(
                new EquipmentSlot[] { EquipmentSlot.FirstPrimaryWeapon, EquipmentSlot.SecondPrimaryWeapon }
            );

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Berserk);
            }

            _berserkEventHasRun = true;
            _originalWSBers.Clear();

            ROPlayer.ActiveHealthController.DoScavRegeneration(10f);

            foreach (var item in items)
            {
                if (item is not Weapon weapon)
                {
                    continue;
                }

                var template = weapon.Template;
                var templateId = item.TemplateId;

                if (!_originalWSBers.ContainsKey(templateId))
                {
                    _originalWSBers[templateId] = new OriginalWeaponStatsBers
                    {
                        Ergo = template.Ergonomics,
                        DuraBurn = template.DurabilityBurnRatio,
                        MalfChance = template.BaseMalfunctionChance,
                        RecoilBack = template.RecoilForceBack,
                        RecoilUp = template.RecoilForceUp,
                    };
                }

                var origStats = _originalWSBers[templateId];
                template.BaseMalfunctionChance = origStats.MalfChance * 0.25f;
                template.DurabilityBurnRatio = origStats.DuraBurn * 0.5f;
                template.Ergonomics = origStats.Ergo * 2f;
                template.RecoilForceBack = origStats.RecoilBack * 0.5f;
                template.RecoilForceUp = origStats.RecoilUp * 0.5f;
            }

            NotificationManagerClass.DisplayMessageNotification(
                "Berserk Event: You're seeing red, I feel bad for any scavs and PMCs in your way!",
                ENotificationDurationType.Long,
                ENotificationIconType.Alert
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Berserk Event has started");
            }

            await UniTask.WaitForSeconds(180);

            ROPlayer.ActiveHealthController.DoScavRegeneration(0);
            ROPlayer.ActiveHealthController.PauseAllEffects();

            foreach (var item in items)
            {
                if (item is not Weapon weapon)
                {
                    continue;
                }

                if (_originalWSBers.TryGetValue(item.TemplateId, out var origStats))
                {
                    var template = weapon.Template;
                    template.BaseMalfunctionChance = origStats.MalfChance;
                    template.DurabilityBurnRatio = origStats.DuraBurn;
                    template.Ergonomics = origStats.Ergo;
                    template.RecoilForceBack = origStats.RecoilBack;
                    template.RecoilForceUp = origStats.RecoilUp;
                }
            }

            NotificationManagerClass.DisplayMessageNotification(
                "Berserk Event: Your vision has cleared up, I guess you got all your rage out!",
                ENotificationDurationType.Long,
                ENotificationIconType.Alert
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

            var items = _session.Profile.Inventory.GetItemsInSlots(
                new EquipmentSlot[]
                {
                    EquipmentSlot.FirstPrimaryWeapon,
                    EquipmentSlot.SecondPrimaryWeapon,
                    EquipmentSlot.Holster,
                    EquipmentSlot.Scabbard,
                    EquipmentSlot.ArmorVest,
                    EquipmentSlot.TacticalVest,
                    EquipmentSlot.Backpack,
                    EquipmentSlot.Earpiece,
                    EquipmentSlot.Headwear,
                }
            );

            var chance = SharedRandom.Next(0, 101);

            if (FikaBridge.AmHost())
            {
                FikaBridge.SendRandomEventPacket(Utils.Weight);
            }

            _weightEventHasRun = true;

            if (chance >= 0 && chance <= 49)
            {
                foreach (var item in items)
                {
                    if (item is null)
                    {
                        continue;
                    }
                    item.Template.Weight *= 2f;
                }
                _session.Profile.Inventory.UpdateTotalWeight();

                NotificationManagerClass.DisplayMessageNotification(
                    "Weight Event: Better hunker down until you get your stamina back!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Alert
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has started");
                }

                await UniTask.WaitForSeconds(180);

                foreach (var item in items)
                {
                    if (item is null)
                    {
                        continue;
                    }
                    item.Template.Weight *= 0.5f;
                }
                _session.Profile.Inventory.UpdateTotalWeight();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has run");
                }

                NotificationManagerClass.DisplayMessageNotification(
                    "Weight Event: You're rested and ready to get back out there!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Alert
                );
            }
            else if (chance >= 50 && chance <= 100)
            {
                foreach (var item in items)
                {
                    if (item is null)
                    {
                        continue;
                    }
                    item.Template.Weight *= 0.5f;
                }
                _session.Profile.Inventory.UpdateTotalWeight();

                NotificationManagerClass.DisplayMessageNotification(
                    "Weight Event: You feel light on your feet, stock up on everything you can!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Alert
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Weight Event has started");
                }

                await UniTask.WaitForSeconds(180);

                foreach (var item in items)
                {
                    if (item is null)
                    {
                        continue;
                    }
                    item.Template.Weight *= 2f;
                }
                _session.Profile.Inventory.UpdateTotalWeight();

                NotificationManagerClass.DisplayMessageNotification(
                    "Weight Event: You've lost your extra energy, hope you didn't fill your backpack too much!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.Alert
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

            if (chance < 50)
            {
                _session.Profile.TradersInfo[trader.Value].SetStanding(_session.Profile.TradersInfo[trader.Value].Standing + 0.1);
                NotificationManagerClass.DisplayMessageNotification(
                    $"Trader Event: {traderName} has gained a little more respect for you.",
                    ENotificationDurationType.Default,
                    ENotificationIconType.Mail
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Trader Rep Gain Event has run");
                }
            }
            else
            {
                if (_session.Profile.TradersInfo[trader.Value].Standing >= 0.05)
                {
                    _session.Profile.TradersInfo[trader.Value].SetStanding(_session.Profile.TradersInfo[trader.Value].Standing - 0.05);
                    NotificationManagerClass.DisplayMessageNotification(
                        $"Trader Event: {traderName} has lost a little faith in you.",
                        ENotificationDurationType.Default,
                        ENotificationIconType.Mail
                    );

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
            if (JsonHandler.CheckFilePath("TraderRep", "Flags"))
            {
                JsonHandler.ReadFlagFile("TraderRep", "Flags");

                if (FikaBridge.AmHost())
                {
                    FikaBridge.SendRandomEventPacket(Utils.MaxLoyaltyLevel);
                }

                if (!ConfigController.Flags.TraderRepFlag)
                {
                    if (_maxLLEventCount >= 1)
                    {
                        Weighting.DoRandomEvent(Weighting.WeightedEvents);
                        return;
                    }

                    var traders = ConfigController.ServerConfig.EnableRequisitionOffice ? Utils.Traders : Utils.TradersNoReq;

                    _maxLLEventCount++;

                    foreach (var trader in traders)
                    {
                        _session.Profile.TradersInfo[trader.Value].SetStanding(_session.Profile.TradersInfo[trader.Value].Standing + 1);
                    }

                    ConfigController.Flags.TraderRepFlag = true;
                    JsonHandler.SaveToJson(ConfigController.Flags, "TraderRep", "Flags");

                    NotificationManagerClass.DisplayMessageNotification(
                        "Shopping Spree Event: All Traders have maxed out standing. Better get to them in the next ten minutes!",
                        ENotificationDurationType.Default,
                        ENotificationIconType.Mail
                    );

                    if (ConfigController.DebugConfig.DebugMode)
                    {
                        Utils.LogToServerConsole("Shopping Spree Event has run");
                    }
                }
                else if (ConfigController.Flags.TraderRepFlag)
                {
                    CorrectRep();
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

            if (JsonHandler.CheckFilePath("TraderRep", "Flags"))
            {
                JsonHandler.ReadFlagFile("TraderRep", "Flags");

                if (ConfigController.Flags.TraderRepFlag)
                {
                    var traders = ConfigController.ServerConfig.EnableRequisitionOffice ? Utils.Traders : Utils.TradersNoReq;

                    foreach (var trader in traders)
                    {
                        _session.Profile.TradersInfo[trader.Value].SetStanding(_session.Profile.TradersInfo[trader.Value].Standing - 1);
                    }

                    ConfigController.Flags.TraderRepFlag = false;
                    JsonHandler.SaveToJson(ConfigController.Flags, "TraderRep", "Flags");
                    Weighting.DoRandomEvent(Weighting.WeightedEvents);
                }
            }
            else
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
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

            if (raidTimeLeft < 900 || ROPlayer.Location == "laboratory")
            {
                Weighting.DoRandomEvent(Weighting.WeightedEvents);
            }
            else
            {
                var exfils = FindObjectsOfType<ExfiltrationPoint>();

                NotificationManagerClass.DisplayMessageNotification(
                    "Lockdown Event: All extracts are unavailable for 15 minutes",
                    ENotificationDurationType.Long,
                    ENotificationIconType.EntryPoint
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Lockdown Event has started");
                }

                EventExfilPatch._isLockdown = true;
                ExfilLockdown = true;

                foreach (var exfil in exfils)
                {
                    if (!exfil.Settings.Name.Contains("Elevator"))
                    {
                        exfil.Disable();
                    }
                }
                _exfilEventCount++;

                TimeStart = System.DateTime.UtcNow.Second;

                await UniTask.WaitForSeconds(600);

                foreach (var exfil in exfils)
                {
                    if (!exfil.Settings.Name.Contains("Elevator"))
                    {
                        exfil.Enable();
                    }
                }

                EventExfilPatch._isLockdown = false;
                ExfilLockdown = false;

                NotificationManagerClass.DisplayMessageNotification(
                    "Lockdown Event: Extracts are available again. Time to get out of there!",
                    ENotificationDurationType.Long,
                    ENotificationIconType.EntryPoint
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

            if (!InvalidArtilleryLocations.Contains(ROPlayer.Location) && !_artyEventHasRun)
            {
                NotificationManagerClass.DisplayMessageNotification(
                    "Artillery Event: Get to cover. Shelling will commence in 30 seconds",
                    ENotificationDurationType.Long,
                    ENotificationIconType.EntryPoint
                );

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Artillery Event has started");
                }

                await UniTask.WaitForSeconds(30);

                NotificationManagerClass.DisplayMessageNotification(
                    "Artillery Event: Shelling has started",
                    ENotificationDurationType.Long,
                    ENotificationIconType.EntryPoint
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

            NotificationManagerClass.DisplayMessageNotification(
                "Train is arriving. Get out if you're ready!",
                ENotificationDurationType.Long,
                ENotificationIconType.EntryPoint
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Train is arriving");
            }

            await UniTask.WaitForSeconds(420);

            NotificationManagerClass.DisplayMessageNotification(
                "Train is leaving the station.",
                ENotificationDurationType.Long,
                ENotificationIconType.EntryPoint
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Train is leaving");
            }
        }

        internal async UniTask DoPmcExfilEvent()
        {
            if (!_pmcExfilEventRunning)
            {
                FikaBridge.SendFlareEventPacket(Utils.PmcExfil);

                _pmcExfilEventRunning = true;

                await UniTask.WaitForSeconds(3);
                NotificationManagerClass.DisplayMessageNotification(
                    "Extract is on it's way! Hold out for two minutes for help to arrive",
                    ENotificationDurationType.Long,
                    ENotificationIconType.EntryPoint
                );
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("Extract event has started");
                }
                await UniTask.WaitForSeconds(120);
                NotificationManagerClass.DisplayMessageNotification(
                    "10",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "9",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "8",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "7",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "6",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "5",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "4",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "3",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "2",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "1",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );
                await UniTask.WaitForSeconds(1);
                NotificationManagerClass.DisplayMessageNotification(
                    "Help has arrived",
                    ENotificationDurationType.Default,
                    ENotificationIconType.EntryPoint
                );

                var exfilSession = Singleton<AbstractGame>.Instance as EndByExitTrigerScenario.GInterface146;
                exfilSession.StopSession(
                    GamePlayerOwner.MyPlayer.ProfileId,
                    ExitStatus.Survived,
                    Singleton<GameWorld>.Instance.ExfiltrationController.ExfiltrationPoints.FirstOrDefault().name
                );

                _pmcExfilEventRunning = false;
            }
        }

        internal void ExfilNow()
        {
            var exfilSession = Singleton<AbstractGame>.Instance as EndByExitTrigerScenario.GInterface146;
            exfilSession.StopSession(
                GamePlayerOwner.MyPlayer.ProfileId,
                ExitStatus.Survived,
                Singleton<GameWorld>.Instance.ExfiltrationController.ExfiltrationPoints.FirstOrDefault().name
            );
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

            NotificationManagerClass.DisplayMessageNotification(
                "Invasion Event: Bosses incoming.",
                ENotificationDurationType.Long,
                ENotificationIconType.Alert
            );

            if (ConfigController.DebugConfig.DebugMode)
            {
                Utils.LogToServerConsole("Invasion Event has started");
            }
        }

        public void CleanForNewEvent()
        {
            _pswitchs = null;
            _keydoor = null;
            _lamp = null;
        }

        private void CheckForFlag()
        {
            if (JsonHandler.CheckFilePath("TraderRep", "Flags"))
            {
                JsonHandler.ReadFlagFile("TraderRep", "Flags");

                if (ConfigController.Flags.TraderRepFlag)
                {
                    CorrectRep();
                }
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
        #endregion
    }
}
