using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using RaidOverhaul.Helpers;
using UnityEngine;

namespace RaidOverhaul.Controllers
{
    public class DebugUIController : MonoBehaviour
    {
        private bool _menuVisible;
        private Rect _windowRect = new Rect(Screen.width / 2 - 225, Screen.height / 2 - 350, 450, 700);
        private Vector2 _scrollPosition;
        private GUIStyle _windowStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _dropdownButtonStyle;
        private GUIStyle _dropdownItemStyle;
        private bool _stylesInitialized;
        private bool _hasInitialized;

        private EventController _eventController;

        private int _selectedBossIndex;
        private bool _bossDropdownOpen;
        private Vector2 _bossDropdownScroll;

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
                        BossEscortDifficult = new[] { "normal", "normal", "normal", "normal" },
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
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharSecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharScout,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
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
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontaySecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
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

        public static Vector2 ScaledPivot
        {
            get { return GetScaling(); }
        }

        private static Vector2 GetScaling()
        {
            float scaling = Mathf.Min(Screen.width / 1920, Screen.height / 1080);
            return new Vector2(scaling, scaling);
        }

        public void ManualUpdate()
        {
            if (!ConfigController.DebugConfig.DebugMode)
            {
                if (_menuVisible)
                {
                    CloseMenu();
                }
                return;
            }

            if (!_hasInitialized)
            {
                _eventController = Plugin._ecScript;
                _hasInitialized = true;
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
                _menuVisible = true;
            }
        }

        private void CloseMenu()
        {
            _menuVisible = false;
        }

        private void InitializeStyles()
        {
            if (_stylesInitialized)
            {
                return;
            }

            _windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.95f)) },
                padding = new RectOffset(10, 10, 20, 10),
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.9f)), textColor = Color.white },
                hover = { background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.9f)), textColor = Color.white },
                active = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.9f)), textColor = Color.white },
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(10, 10, 5, 5),
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = Color.white },
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = new Color(1f, 0.84f, 0f) },
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };

            _dropdownButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTex(2, 2, new Color(0.25f, 0.25f, 0.25f, 0.9f)), textColor = Color.white },
                hover = { background = MakeTex(2, 2, new Color(0.35f, 0.35f, 0.35f, 0.9f)), textColor = Color.white },
                active = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.9f)), textColor = Color.white },
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(10, 10, 5, 5),
            };

            _dropdownItemStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.95f)), textColor = Color.white },
                hover = { background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.8f)), textColor = Color.white },
                active = { background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.95f)), textColor = Color.white },
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(15, 10, 4, 4),
            };

            _stylesInitialized = true;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void OnGUI()
        {
            if (!_menuVisible)
            {
                return;
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    CloseMenu();
                    Event.current.Use();
                    return;
                }

                if (Event.current.keyCode == TOGGLE_KEY)
                {
                    CloseMenu();
                    Event.current.Use();
                    return;
                }
            }

            InitializeStyles();

            _windowRect = GUILayout.Window(
                64196,
                _windowRect,
                DrawWindow,
                "Debug Menu",
                _windowStyle,
                GUILayout.MinWidth(450),
                GUILayout.MinHeight(700)
            );
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GUIUtility.ScaleAroundPivot(ScaledPivot, Vector2.zero);
            UnityInput.Current.ResetInputAxes();
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Debug Actions", _headerStyle);
            GUILayout.Space(10);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            DrawSection(
                "Player Events",
                () =>
                {
                    DrawButton("Heal Player", () => _eventController.DoHealPlayer());
                    DrawButton("Damage Event", () => _eventController.DoDamageEvent());
                    DrawButton("Metabolism Event", () => _eventController.DoMetabolismEvent());
                    DrawButton("Berserk Event", () => _eventController.DoBerserkEventWrapper());
                    DrawButton("Weight Event", () => _eventController.DoWeightEventWrapper());
                    DrawButton("Skill Event", () => _eventController.DoSkillEvent());
                }
            );

            GUILayout.Space(10);

            DrawSection(
                "Equipment Events",
                () =>
                {
                    DrawButton("Armor Repair", () => _eventController.DoArmorRepair());
                    DrawButton("Malfunction Event", () => _eventController.DoMalfEventWrapper());
                }
            );

            GUILayout.Space(10);

            DrawSection(
                "World Events",
                () =>
                {
                    DrawButton("Airdrop Event", () => _eventController.DoAirdropEvent());
                    DrawButton("Blackout Event", () => _eventController.DoBlackoutEventWrapper());
                    DrawButton("Lockdown Event", () => _eventController.DoLockDownEventWrapper());
                    DrawButton("Artillery Event", () => _eventController.DoArtyEventWrapper());
                    DrawButton("Invasion Event", () => _eventController.StartInvasion());
                    DrawButton("Funny Event", () => _eventController.DoFunnyWrapper());
                }
            );

            GUILayout.Space(10);

            DrawSection(
                "Trader & Extraction",
                () =>
                {
                    DrawButton("Loyalty Level Event", () => _eventController.DoLLEvent());
                    DrawButton("Max Loyalty Level", () => _eventController.DoMaxLLEvent());
                    DrawButton("Correct Rep", () => _eventController.CorrectRep());
                    DrawButton("PMC Exfil Event", () => _eventController.DoPmcExfilEventWrapper());
                    DrawButton("Exfil Now", () => _eventController.ExfilNow());
                    DrawButton("Run Train", () => _eventController.RunTrainWrapper());
                }
            );

            GUILayout.Space(10);

            DrawSection(
                "Utilities",
                () =>
                {
                    DrawButton("Log All Weapon IDs", GetAllWeaponIDs);
                    DrawButton("Log All Item IDs", GetAllItemIDs);
                }
            );

            GUILayout.Space(10);

            DrawSection(
                "Spawn Bosses",
                () =>
                {
                    GUILayout.Label("Select Boss:", _labelStyle);
                    GUILayout.Space(3);

                    if (GUILayout.Button(_bossDisplayNames[_selectedBossIndex] + "  ▼", _dropdownButtonStyle, GUILayout.Height(30)))
                    {
                        _bossDropdownOpen = !_bossDropdownOpen;
                    }

                    if (_bossDropdownOpen)
                    {
                        _bossDropdownScroll = GUILayout.BeginScrollView(_bossDropdownScroll, GUILayout.Height(150));
                        for (int i = 0; i < _bossDisplayNames.Length; i++)
                        {
                            string label = _selectedBossIndex == i ? "► " + _bossDisplayNames[i] : "   " + _bossDisplayNames[i];
                            if (GUILayout.Button(label, _dropdownItemStyle, GUILayout.Height(26)))
                            {
                                _selectedBossIndex = i;
                                _bossDropdownOpen = false;
                            }
                        }
                        GUILayout.EndScrollView();
                    }

                    GUILayout.Space(5);
                    DrawButton("Spawn Selected Boss", () => SpawnBoss(_bossSpawnConfigs[_selectedBossIndex]));
                }
            );

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Press ESC or {TOGGLE_KEY} to close", _labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawSection(string title, Action drawContent)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(title, _headerStyle);
            GUILayout.Space(5);
            drawContent();
            GUILayout.EndVertical();
        }

        private void DrawButton(string label, Action onClick)
        {
            if (_eventController != null && GUILayout.Button(label, _buttonStyle, GUILayout.Height(35)))
            {
                onClick();
            }
        }

        private static void GetAllWeaponIDs()
        {
            var weapons = Plugin._session.Profile.Inventory?.AllRealPlayerItems;
            weapons = weapons.Where(x => x is Weapon);

            foreach (var weapon in weapons)
            {
                Plugin._log.LogInfo($"Template ID: {weapon.TemplateId}, locale name: {weapon.LocalizedName()}");
                Utils.LogToServerConsole($"Template ID: {weapon.TemplateId}, locale name: {weapon.LocalizedName()}");
            }
        }

        private static void GetAllItemIDs()
        {
            var items = Plugin._session.Profile.Inventory?.AllRealPlayerItems;
            items = items.Where(x => x is Item);

            foreach (var item in items)
            {
                Plugin._log.LogInfo($"Template ID: {item.TemplateId}, locale name: {item.LocalizedName()}");
                Utils.LogToServerConsole($"Template ID: {item.TemplateId}, locale name: {item.LocalizedName()}");
            }
        }

        private static void SpawnBoss(BossInvasionConfig bossConfig)
        {
            var spawner = Singleton<IBotGame>.Instance?.BotsController?.BotSpawner;
            if (spawner == null)
            {
                return;
            }

            var bossZones = spawner.SpawnZones(false).Where(z => z.CanSpawnBoss).ToList();
            if (bossZones.Count == 0)
            {
                return;
            }

            var zone = bossZones[new System.Random().Next(bossZones.Count)];

            var wave = new BossLocationSpawn
            {
                BossName = bossConfig.BossName,
                BossType = bossConfig.BossType,
                BossChance = 100f,
                BossPlayer = false,
                BossDifficult = "normal",
                BossDif = BotDifficulty.normal,
                BossZone = zone.NameZone,
                BornZone = zone.NameZone,
                BossEscortType = bossConfig.BossEscorts,
                EscortType = bossConfig.BossEscortType,
                BossEscortAmount = bossConfig.BossEscortCount.ToString(),
                EscortCount = bossConfig.BossEscortCount,
                BossEscortDifficult = "normal",
                EscortDif = BotDifficulty.normal,
                Supports = bossConfig.AdditionalSupports,
                ForceSpawn = true,
                IgnoreMaxBots = true,
                ShallSpawn = true,
                Time = -1f,
                TriggerType = SpawnTriggerType.none,
                TriggerId = "",
                TriggerName = "",
            };

            if (bossConfig.AdditionalSupports != null && bossConfig.AdditionalSupports.Length > 0)
            {
                wave.SubDatas = new List<BossLocationSpawnSubData>();
                int totalEscorts = bossConfig.BossEscortCount;

                foreach (var support in bossConfig.AdditionalSupports)
                {
                    var difficulty = (BotDifficulty)Enum.Parse(typeof(BotDifficulty), support.BossEscortDifficult[0]);
                    var subData = new BossLocationSpawnSubData(support.BossEscortAmount, support.BossEscortType, difficulty);
                    wave.SubDatas.Add(subData);
                    totalEscorts += subData.BossEscortAmount;
                }

                wave.EscortCount = totalEscorts;
            }

            spawner.ActivateBotsByWave(wave);
        }
    }
}
