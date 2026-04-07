using System;
using System.Linq;
using BepInEx;
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
        private bool _stylesInitialized;
        private bool _hasInitialized;

        private EventController _eventController;

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
    }
}
