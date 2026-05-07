using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.InventoryLogic;
using RaidOverhaul.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class DebugUIController : MonoBehaviour
    {
        private bool _menuVisible;
        private bool _tookInputOwnership;

        private EventController _eventController;

        private GameObject _menuPrefab;
        private GameObject _menuInstance;
        private AudioSource _audioSource;

        private Button _healPlayerButton;
        private Button _damageEventButton;
        private Button _metabolismEventButton;
        private Button _berserkEventButton;
        private Button _weightEventButton;
        private Button _skillEventButton;

        private Button _armorRepairButton;
        private Button _malfunctionButton;

        private Button _airdropButton;
        private Button _triggerCleanupButton;
        private Button _blackoutButton;
        private Button _lockdownButton;
        private Button _artilleryButton;
        private Button _invasionButton;
        private Button _huntedButton;
        private Button _funnyButton;

        private Button _loyaltyLevelButton;
        private Button _maxLoyaltyButton;
        private Button _correctRepButton;
        private Button _pmcExfilButton;
        private Button _exfilNowButton;
        private Button _runTrainButton;

        private Button _logWeaponIDsButton;
        private Button _logItemIDsButton;

        private TMP_Dropdown _bossDropdown;
        private Button _spawnBossButton;

        // Matches the order of the BossInvasionConfig list so the indexes align when selecting from the dropdown menu
        // This ensures that the correct boss config is applied when a boss is selected from the debug UI
        // When adding new entries in the future, DON'T FORGET TO KEEP THIS IN SYNC BAKAAAAA
        private static readonly string[] _bossDisplayNames =
        {
            "Kaban (bossBoar)",
            "Reshala (bossBully)",
            "Gluhar (bossGluhar)",
            "Killa (bossKilla)",
            "Knight & Goons (bossKnight)",
            "Shturman (bossKojaniy)",
            "Kolontay (bossKolontay)",
            "Sanitar (bossSanitar)",
            "Tagilla (bossTagilla)",
            "Zryachiy (bossZryachiy)",
            "Legion (bossLegion)",
            "Legionnaire Squad",
            "Rogue Squad (exUsec)",
        };

        private static readonly List<BossInvasionConfig> _bossSpawnConfigs = new List<BossInvasionConfig>
        {
            new BossInvasionConfig
            {
                BossName = "bossBoar",
                BossEscorts = "followerBoar",
                BossType = WildSpawnType.bossBoar,
                BossEscortType = WildSpawnType.followerBoar,
                BossEscortCount = 6,
                AdditionalSupports = new[]
                {
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoar,
                        BossEscortAmount = 4,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoarClose1,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoarClose2,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                },
            },
            new BossInvasionConfig
            {
                BossName = "bossBully",
                BossEscorts = "followerBully",
                BossType = WildSpawnType.bossBully,
                BossEscortType = WildSpawnType.followerBully,
                BossEscortCount = 4,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossGluhar",
                BossEscorts = "followerGluharSecurity",
                BossType = WildSpawnType.bossGluhar,
                BossEscortType = WildSpawnType.followerGluharSecurity,
                BossEscortCount = 2,
                AdditionalSupports = new[]
                {
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharAssault,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharSecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharScout,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                },
            },
            new BossInvasionConfig
            {
                BossName = "bossKilla",
                BossEscorts = "followerTagilla",
                BossType = WildSpawnType.bossKilla,
                BossEscortType = WildSpawnType.followerTagilla,
                BossEscortCount = 0,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKnight",
                BossEscorts = "exUsec",
                BossType = WildSpawnType.bossKnight,
                BossEscortType = WildSpawnType.exUsec,
                BossEscortCount = 2,
                AdditionalSupports = new[]
                {
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBigPipe,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBirdEye,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                },
            },
            new BossInvasionConfig
            {
                BossName = "bossKojaniy",
                BossEscorts = "followerKojaniy",
                BossType = WildSpawnType.bossKojaniy,
                BossEscortType = WildSpawnType.followerKojaniy,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKolontay",
                BossEscorts = "followerKolontaySecurity",
                BossType = WildSpawnType.bossKolontay,
                BossEscortType = WildSpawnType.followerKolontaySecurity,
                BossEscortCount = 2,
                AdditionalSupports = new[]
                {
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontayAssault,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontaySecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                },
            },
            new BossInvasionConfig
            {
                BossName = "bossSanitar",
                BossEscorts = "followerSanitar",
                BossType = WildSpawnType.bossSanitar,
                BossEscortType = WildSpawnType.followerSanitar,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossTagilla",
                BossEscorts = "followerBully",
                BossType = WildSpawnType.bossTagilla,
                BossEscortType = WildSpawnType.followerBully,
                BossEscortCount = 0,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossZryachiy",
                BossEscorts = "followerZryachiy",
                BossType = WildSpawnType.bossZryachiy,
                BossEscortType = WildSpawnType.followerZryachiy,
                BossEscortCount = 2,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bosslegion",
                BossEscorts = "legionnaire",
                BossType = (WildSpawnType)199,
                BossEscortType = (WildSpawnType)200,
                BossEscortCount = 2,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "legionnaire",
                BossEscorts = "legionnaire",
                BossType = (WildSpawnType)200,
                BossEscortType = (WildSpawnType)200,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "exUsec",
                BossEscorts = "exUsec",
                BossType = WildSpawnType.exUsec,
                BossEscortType = WildSpawnType.exUsec,
                BossEscortCount = 4,
                AdditionalSupports = null,
            },
        };

        private const KeyCode TOGGLE_KEY = KeyCode.F8;

        private void Awake()
        {
            _menuPrefab = DebugCanvasPrefab;
            _eventController = GetComponent<EventController>();
        }

        private void Update()
        {
            if (!ConfigController.DebugConfig.IsDev)
            {
                if (_menuVisible)
                {
                    CloseMenu();
                }
                return;
            }

            if (_menuVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (!Utils.IsInRaid())
                {
                    CloseMenu();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseMenu();
                    return;
                }
            }

            if (!Utils.IsInRaid())
            {
                return;
            }

            if (Input.GetKeyDown(TOGGLE_KEY))
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            if (_menuVisible)
            {
                CloseMenu();
            }
            else
            {
                ShowMenu();
            }
        }

        private void PlayClick()
        {
            _audioSource?.PlayOneShot(SoundButtonClick);
        }

        private void ShowMenu()
        {
            if (_menuInstance == null)
            {
                _menuInstance = Instantiate(_menuPrefab);
                DontDestroyOnLoad(_menuInstance);
                _audioSource = _menuInstance.GetComponent<AudioSource>();
                WireUpUI();
            }
            EventsEffectsController.Instance?.ShowCornerGlowEffect();
            _menuInstance.SetActive(true);
            _menuVisible = true;
            _tookInputOwnership = !GamePlayerOwner.IgnoreInputInNPCDialog;
            if (_tookInputOwnership)
            {
                GamePlayerOwner.SetIgnoreInputInNPCDialog(true);
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void CloseMenu()
        {
            _menuVisible = false;
            if (_tookInputOwnership)
            {
                GamePlayerOwner.SetIgnoreInputInNPCDialog(false);
            }
            _tookInputOwnership = false;
            if (_menuInstance != null)
            {
                _menuInstance.SetActive(false);
            }
        }

        private void WireUpUI()
        {
            var es = _menuInstance.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
            if (es != null && UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current != es)
            {
                es.gameObject.SetActive(false);
            }

            var bg = _menuInstance.transform.Find("Background");
            var content = bg.Find("Scroll View/Viewport/Content");

            var playerSection = content.Find("PlayerEventsSection");
            var equipSection = content.Find("EquipmentEventsSection");
            var worldSection = content.Find("WorldEventsSection");
            var traderSection = content.Find("TraderExtractionSection");
            var utilSection = content.Find("UtilitiesSection");
            var spawnSection = content.Find("SpawnBossesSection");

            _healPlayerButton = playerSection.Find("HealPlayerButton").GetComponent<Button>();
            _damageEventButton = playerSection.Find("DamageEventButton").GetComponent<Button>();
            _metabolismEventButton = playerSection.Find("MetabolismEventButton").GetComponent<Button>();
            _berserkEventButton = playerSection.Find("BerserkEventButton").GetComponent<Button>();
            _weightEventButton = playerSection.Find("WeightEventButton").GetComponent<Button>();
            _skillEventButton = playerSection.Find("SkillEventButton").GetComponent<Button>();

            _armorRepairButton = equipSection.Find("ArmorRepairButton").GetComponent<Button>();
            _malfunctionButton = equipSection.Find("MalfunctionButton").GetComponent<Button>();

            _airdropButton = worldSection.Find("AirdropButton").GetComponent<Button>();
            _triggerCleanupButton = worldSection.Find("TriggerCleanupButton").GetComponent<Button>();
            _blackoutButton = worldSection.Find("BlackoutButton").GetComponent<Button>();
            _lockdownButton = worldSection.Find("LockdownButton").GetComponent<Button>();
            _artilleryButton = worldSection.Find("ArtilleryButton").GetComponent<Button>();
            _invasionButton = worldSection.Find("InvasionButton").GetComponent<Button>();
            _huntedButton = worldSection.Find("HuntedButton").GetComponent<Button>();
            _funnyButton = worldSection.Find("FunnyButton").GetComponent<Button>();

            _loyaltyLevelButton = traderSection.Find("LoyaltyLevelButton").GetComponent<Button>();
            _maxLoyaltyButton = traderSection.Find("MaxLoyaltyButton").GetComponent<Button>();
            _correctRepButton = traderSection.Find("CorrectRepButton").GetComponent<Button>();
            _pmcExfilButton = traderSection.Find("PmcExfilButton").GetComponent<Button>();
            _exfilNowButton = traderSection.Find("ExfilNowButton").GetComponent<Button>();
            _runTrainButton = traderSection.Find("RunTrainButton").GetComponent<Button>();

            _logWeaponIDsButton = utilSection.Find("LogWeaponIDsButton").GetComponent<Button>();
            _logItemIDsButton = utilSection.Find("LogItemIDsButton").GetComponent<Button>();

            _spawnBossButton = spawnSection.Find("SpawnBossButton").GetComponent<Button>();
            _bossDropdown = spawnSection.Find("BossDropdown").GetComponent<TMP_Dropdown>();
            _bossDropdown.ClearOptions();
            _bossDropdown.AddOptions(new List<string>(_bossDisplayNames));
            _bossDropdown.value = 0;

            var captionText = _bossDropdown.captionText;
            if (captionText != null)
            {
                float maxWidth = 0f;
                foreach (var name in _bossDisplayNames)
                {
                    float w = captionText.GetPreferredValues(name).x;
                    if (w > maxWidth)
                    {
                        maxWidth = w;
                    }
                }
                var dropdownRect = _bossDropdown.GetComponent<RectTransform>();
                dropdownRect.sizeDelta = new Vector2(maxWidth + 50f, dropdownRect.sizeDelta.y);
            }

            _healPlayerButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoHealPlayer();
            });
            _damageEventButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoDamageEvent();
            });
            _metabolismEventButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoMetabolismEvent();
            });
            _berserkEventButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoBerserkEventWrapper();
            });
            _weightEventButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoWeightEventWrapper();
            });
            _skillEventButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoSkillEvent();
            });
            _armorRepairButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoArmorRepair();
            });
            _malfunctionButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoMalfEventWrapper();
            });
            _airdropButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoAirdropEvent();
            });
            _triggerCleanupButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.TriggerManualCleanup();
            });
            _blackoutButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoBlackoutEventWrapper();
            });
            _lockdownButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoLockDownEventWrapper();
            });
            _artilleryButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoArtyEventWrapper();
            });
            _invasionButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.StartInvasion();
            });
            _huntedButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoHuntedEventWrapper();
            });
            _funnyButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoFunnyWrapper();
            });
            _loyaltyLevelButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoLLEvent();
            });
            _maxLoyaltyButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoMaxLLEvent();
            });
            _correctRepButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.CorrectRep();
            });
            _pmcExfilButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.DoPmcExfilEventWrapper();
            });
            _exfilNowButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.ExfilNow();
            });
            _runTrainButton.onClick.AddListener(() =>
            {
                PlayClick();
                _eventController.RunTrainWrapper();
            });
            _logWeaponIDsButton.onClick.AddListener(() =>
            {
                PlayClick();
                GetAllWeaponIDs();
            });
            _logItemIDsButton.onClick.AddListener(() =>
            {
                PlayClick();
                GetAllItemIDs();
            });
            _spawnBossButton.onClick.AddListener(() =>
            {
                PlayClick();
                Utils.SpawnBoss(_bossSpawnConfigs[_bossDropdown.value]);
            });
        }

        private void OnDestroy()
        {
            if (_menuInstance != null)
            {
                Destroy(_menuInstance);
            }
        }

        private static void GetAllWeaponIDs()
        {
            var weapons = GetSession()?.Profile.Inventory?.AllRealPlayerItems;
            weapons = weapons.Where(x => x is Weapon);

            foreach (var weapon in weapons)
            {
                _log.LogInfo($"Template ID: {weapon.TemplateId}, locale name: {weapon.LocalizedName()}");
                Utils.LogToServerConsole($"Template ID: {weapon.TemplateId}, locale name: {weapon.LocalizedName()}");
            }
        }

        private static void GetAllItemIDs()
        {
            var items = GetSession()?.Profile.Inventory?.AllRealPlayerItems;
            items = items.Where(x => x is Item);

            foreach (var item in items)
            {
                _log.LogInfo($"Template ID: {item.TemplateId}, locale name: {item.LocalizedName()}");
                Utils.LogToServerConsole($"Template ID: {item.TemplateId}, locale name: {item.LocalizedName()}");
            }
        }
    }
}
