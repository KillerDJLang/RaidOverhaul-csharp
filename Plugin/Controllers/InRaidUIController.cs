using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using Newtonsoft.Json;
using RaidOverhaul.Configs;
using RaidOverhaul.Helpers;
using SPT.Common.Http;
using UnityEngine;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class InRaidUIController : MonoBehaviour
    {
        private bool _menuVisible;
        private Rect _windowRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 300, 400, 600);
        private Vector2 _scrollPosition;
        private GUIStyle _windowStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _disabledButtonStyle;
        private bool _stylesInitialized;
        private bool _hasInitialized;

        private EventController _eventController;
        private static readonly JsonConverter[] _defaultJsonConverters;

        private bool _showGearTransferUI;
        private Vector2 _gearScrollPosition;
        private List<Item> _transferableItems;
        private HashSet<string> _selectedItemIds;

        private const int TRANSFER_GRID_WIDTH = 5;
        private const int TRANSFER_GRID_HEIGHT = 5;
        private const int TRANSFER_MAX_CELLS = TRANSFER_GRID_WIDTH * TRANSFER_GRID_HEIGHT;

        private static readonly HashSet<string> _invalidTrainLocations = new HashSet<string>
        {
            "factory4_day",
            "factory4_night",
            "laboratory",
            "Sandbox",
            "Sandbox_high",
            "bigmap",
            "Interchange",
            "Labyrinth",
            "Shoreline",
            "TarkovStreets",
            "Woods",
        };

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
            if (!_hasInitialized)
            {
                _eventController = Plugin._ecScript;
                _hasInitialized = true;
            }

            // Process close input BEFORE feature-flag and raid checks
            // so the menu can always be dismissed.
            if (_menuVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Force-close when the feature is turned off or we leave the raid
                if (!DJConfig.SpecialReqFeatures.Value || !Utils.IsInRaid())
                {
                    CloseMenu();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (_showGearTransferUI)
                    {
                        _showGearTransferUI = false;
                    }
                    else
                    {
                        CloseMenu();
                    }
                    return;
                }
            }

            if (!DJConfig.SpecialReqFeatures.Value || !Utils.IsInRaid())
            {
                return;
            }

            var keybind = DJConfig.SpecialReqFeaturesBinding.Value;
            bool keyPressed = Input.GetKeyDown(keybind.MainKey);

            if (keyPressed)
            {
                if (keybind.Modifiers != null && keybind.Modifiers.Any())
                {
                    foreach (var modifier in keybind.Modifiers)
                    {
                        if (!Input.GetKey(modifier))
                        {
                            keyPressed = false;
                            break;
                        }
                    }
                }
            }

            if (keyPressed)
            {
                ToggleMenu();
            }
        }

        private bool HasReqCurrency()
        {
            if (!Utils.IsInRaid() || _session?.Profile?.Inventory == null)
            {
                return false;
            }

            var allItems = _session.Profile.Inventory.AllRealPlayerItems;

            if (allItems == null)
            {
                return false;
            }

            var reqCoins = Utils.Currency["ReqCoins"];
            var reqSlips = Utils.Currency["ReqSlips"];
            var specialForms = Utils.Currency["SpecialReqForms"];

            return allItems.Any(item => item.TemplateId == reqCoins || item.TemplateId == reqSlips || item.TemplateId == specialForms);
        }

        private int GetCurrencyCount(string currencyKey)
        {
            if (!Utils.IsInRaid() || _session?.Profile?.Inventory == null)
            {
                return 0;
            }

            var allItems = _session.Profile.Inventory.AllRealPlayerItems;

            if (allItems == null)
            {
                return 0;
            }

            var currencyId = Utils.Currency[currencyKey];
            var currencyItems = allItems.Where(item => item.TemplateId == currencyId);

            int totalCount = 0;
            foreach (var item in currencyItems)
            {
                var stackCount = item.StackObjectsCount;
                totalCount += stackCount > 0 ? stackCount : 1;
            }

            return totalCount;
        }

        private bool IsTrainAvailable()
        {
            if (!Utils.IsInRaid() || ROPlayer == null)
            {
                return false;
            }

            if (!_invalidTrainLocations.Contains(ROPlayer.Location))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool RemoveCurrency(string currencyKey, int amountToRemove)
        {
            if (!Utils.IsInRaid() || _session?.Profile?.Inventory == null)
            {
                return false;
            }

            var allItems = _session.Profile.Inventory.AllRealPlayerItems;

            if (allItems == null)
            {
                return false;
            }

            var currencyId = Utils.Currency[currencyKey];
            var currencyItems = allItems.Where(item => item.TemplateId == currencyId).ToList();

            if (currencyItems.Count == 0)
            {
                return false;
            }

            int totalAvailable = 0;
            foreach (var item in currencyItems)
            {
                var stackCount = item.StackObjectsCount;
                totalAvailable += stackCount > 0 ? stackCount : 1;
            }

            if (totalAvailable < amountToRemove)
            {
                return false;
            }

            int remainingToRemove = amountToRemove;

            foreach (var item in currencyItems.ToList())
            {
                if (remainingToRemove <= 0)
                {
                    break;
                }

                var stackCount = item.StackObjectsCount > 0 ? item.StackObjectsCount : 1;

                if (stackCount <= remainingToRemove)
                {
                    item.StackObjectsCount = 0;
                    RemoveZeroStackItem(ROPlayer, item);
                    remainingToRemove -= stackCount;
                }
                else
                {
                    item.StackObjectsCount -= remainingToRemove;
                    remainingToRemove = 0;
                }
            }

            return true;
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
            _showGearTransferUI = false;
            _selectedItemIds?.Clear();
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

            _disabledButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.5f)), textColor = new Color(0.5f, 0.5f, 0.5f) },
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

            // Fallback input handling through IMGUI event system —
            // catches key events that the game's input layer may have
            // consumed before ManualUpdate() could process them.
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    if (_showGearTransferUI)
                    {
                        _showGearTransferUI = false;
                    }
                    else
                    {
                        CloseMenu();
                    }
                    Event.current.Use();
                    return;
                }

                var keybind = DJConfig.SpecialReqFeaturesBinding.Value;
                if (Event.current.keyCode == keybind.MainKey)
                {
                    bool modifiersHeld = true;
                    if (keybind.Modifiers != null && keybind.Modifiers.Any())
                    {
                        foreach (var modifier in keybind.Modifiers)
                        {
                            if (!Input.GetKey(modifier))
                            {
                                modifiersHeld = false;
                                break;
                            }
                        }
                    }

                    if (modifiersHeld)
                    {
                        CloseMenu();
                        Event.current.Use();
                        return;
                    }
                }
            }

            InitializeStyles();

            _windowRect = GUILayout.Window(
                64195,
                _windowRect,
                DrawWindow,
                "Exfil & Train Call Menu",
                _windowStyle,
                GUILayout.MinWidth(400),
                GUILayout.MinHeight(600)
            );
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GUIUtility.ScaleAroundPivot(ScaledPivot, Vector2.zero);
            UnityInput.Current.ResetInputAxes();
        }

        private void DrawWindow(int windowID)
        {
            if (_showGearTransferUI)
            {
                DrawGearTransferWindow(windowID);
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label("Special Actions", _headerStyle);
            GUILayout.Space(10);

            if (HasReqCurrency())
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Requisition Currency", _headerStyle);
                GUILayout.Space(5);

                int reqCoins = GetCurrencyCount("ReqCoins");
                int reqSlips = GetCurrencyCount("ReqSlips");
                int reqForms = GetCurrencyCount("SpecialReqForms");

                GUILayout.Label($"Requisition Coins: {reqCoins}", _labelStyle);
                GUILayout.Label($"Requisition Slips: {reqSlips}", _labelStyle);
                GUILayout.Label($"Special Requisition Forms: {reqForms}", _labelStyle);

                GUILayout.EndVertical();
                GUILayout.Space(10);
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Emergency Extraction", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label("Call for an emergency extraction. Help will arrive in 2 minutes.", _labelStyle);
            GUILayout.Label("Cost: 15 Requisition Slips", _labelStyle);
            GUILayout.Space(5);

            int currentSlips = GetCurrencyCount("ReqSlips");
            bool canAffordExfil = currentSlips >= 15;

            GUI.enabled = canAffordExfil;
            if (
                _eventController != null
                && GUILayout.Button(
                    $"Call Emergency Exfil ({currentSlips}/15)",
                    canAffordExfil ? _buttonStyle : _disabledButtonStyle,
                    GUILayout.Height(40)
                )
            )
            {
                if (RemoveCurrency("ReqSlips", 15))
                {
                    _eventController.DoPmcExfilEventWrapper();
                    CloseMenu();
                }
            }
            GUI.enabled = true;

            if (!canAffordExfil)
            {
                GUILayout.Label("Insufficient Requisition Slips!", new GUIStyle(_labelStyle) { normal = { textColor = Color.red } });
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Train Extraction", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label("Call the train for extraction. The train will arrive shortly.", _labelStyle);
            GUILayout.Label("Cost: 250 Requisition Coins", _labelStyle);
            GUILayout.Space(5);

            int currentCoins = GetCurrencyCount("ReqCoins");
            bool isOnTrainMap = IsTrainAvailable();
            bool canAffordTrain = currentCoins >= 250 && isOnTrainMap;

            GUI.enabled = canAffordTrain;
            if (
                _eventController != null
                && GUILayout.Button(
                    $"Call Train ({currentCoins}/250)",
                    canAffordTrain ? _buttonStyle : _disabledButtonStyle,
                    GUILayout.Height(40)
                )
            )
            {
                if (RemoveCurrency("ReqCoins", 250))
                {
                    _eventController.RunTrainWrapper();
                    CloseMenu();
                }
            }
            GUI.enabled = true;

            if (!isOnTrainMap)
            {
                GUILayout.Label(
                    "Train only available on Reserve or Lighthouse!",
                    new GUIStyle(_labelStyle) { normal = { textColor = Color.red } }
                );
            }
            else if (currentCoins < 250)
            {
                GUILayout.Label("Insufficient Requisition Coins!", new GUIStyle(_labelStyle) { normal = { textColor = Color.red } });
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Instant Extraction", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label("Immediately extract from the raid. A premium service with a premium price!", _labelStyle);
            GUILayout.Label("Cost: 25 Requisition Slips", _labelStyle);
            GUILayout.Space(5);

            bool canAffordInstantExfil = currentSlips >= 25;

            GUI.enabled = canAffordInstantExfil;
            if (
                _eventController != null
                && GUILayout.Button(
                    $"Extract Now ({currentSlips}/25)",
                    canAffordInstantExfil ? _buttonStyle : _disabledButtonStyle,
                    GUILayout.Height(40)
                )
            )
            {
                if (RemoveCurrency("ReqSlips", 25))
                {
                    _eventController.ExfilNow();
                    CloseMenu();
                }
            }
            GUI.enabled = true;

            if (!canAffordInstantExfil)
            {
                GUILayout.Label("Insufficient Requisition Slips!", new GUIStyle(_labelStyle) { normal = { textColor = Color.red } });
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Extract Gear", _headerStyle);
            GUILayout.Space(5);
            GUILayout.Label("Send items to main stash. Extract your valuable gear safely!", _labelStyle);
            GUILayout.Label("Cost: 1 Special Requisition Form", _labelStyle);
            GUILayout.Space(5);

            int currentForms = GetCurrencyCount("SpecialReqForms");
            bool canAffordGearExtract = currentForms >= 1;

            GUI.enabled = canAffordGearExtract;
            if (
                _eventController != null
                && GUILayout.Button(
                    $"Extract Gear ({currentForms}/1)",
                    canAffordGearExtract ? _buttonStyle : _disabledButtonStyle,
                    GUILayout.Height(40)
                )
            )
            {
                OpenGearTransferUI();
            }
            GUI.enabled = true;

            if (!canAffordGearExtract)
            {
                GUILayout.Label(
                    "Insufficient Special Requisition Forms!",
                    new GUIStyle(_labelStyle) { normal = { textColor = Color.red } }
                );
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Press ESC or {DJConfig.SpecialReqFeaturesBinding.Value.MainKey} to close", _labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        public bool RemoveZeroStackItem(Player player, Item item)
        {
            TraderControllerClass inventoryController = player.InventoryController;

            GStruct154<GClass3408> result = InteractionsHandlerClass.Discard(item, inventoryController, simulate: false);

            if (result.Failed)
            {
                Plugin._log.LogError($"Failed to remove item: {result.Error}");
                return false;
            }

            result.Value.RaiseEvents(inventoryController, CommandStatus.Begin);
            result.Value.RaiseEvents(inventoryController, CommandStatus.Succeed);

            return true;
        }

        private void OpenGearTransferUI()
        {
            _selectedItemIds = new HashSet<string>();
            _transferableItems = GetTransferableItems();
            _showGearTransferUI = true;
            _gearScrollPosition = Vector2.zero;
        }

        private (int totalCells, int usedCells, int freeCells) CalculateTransferStashSpace()
        {
            try
            {
                if (_selectedItemIds == null || _transferableItems == null)
                {
                    return (TRANSFER_MAX_CELLS, 0, TRANSFER_MAX_CELLS);
                }

                int usedCells = 0;

                foreach (var itemId in _selectedItemIds)
                {
                    var item = _transferableItems.FirstOrDefault(i => i.Id == itemId);
                    if (item != null)
                    {
                        int itemWidth = item.Template.Width;
                        int itemHeight = item.Template.Height;
                        usedCells += itemWidth * itemHeight;
                    }
                }

                int freeCells = TRANSFER_MAX_CELLS - usedCells;

                return (TRANSFER_MAX_CELLS, usedCells, freeCells);
            }
            catch (Exception ex)
            {
                Plugin._log.LogError($"Error calculating transfer stash space: {ex.Message}");
                return (TRANSFER_MAX_CELLS, 0, TRANSFER_MAX_CELLS);
            }
        }

        private int CalculateSelectedItemsSize()
        {
            if (_selectedItemIds == null || _selectedItemIds.Count == 0 || _transferableItems == null)
            {
                return 0;
            }

            int totalSize = 0;

            foreach (var itemId in _selectedItemIds)
            {
                var item = _transferableItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    int itemWidth = item.Template.Width;
                    int itemHeight = item.Template.Height;
                    totalSize += itemWidth * itemHeight;
                }
            }

            return totalSize;
        }

        private List<Item> GetTransferableItems()
        {
            var items = new List<Item>();

            if (!Utils.IsInRaid() || ROPlayer?.Profile?.Inventory == null)
            {
                return items;
            }

            var inventory = ROPlayer.Profile.Inventory;

            var rootItems = new List<Item>();

            var backpack = inventory.Equipment.GetSlot(EquipmentSlot.Backpack).ContainedItem;
            if (backpack != null && backpack is CompoundItem backpackContainer)
            {
                AddContainerItems(backpackContainer, rootItems);
            }

            var rig = inventory.Equipment.GetSlot(EquipmentSlot.TacticalVest).ContainedItem;
            if (rig != null && rig is CompoundItem rigContainer)
            {
                AddContainerItems(rigContainer, rootItems);
            }

            var pockets = inventory.Equipment.GetSlot(EquipmentSlot.Pockets).ContainedItem;
            if (pockets != null && pockets is CompoundItem pocketsContainer)
            {
                AddContainerItems(pocketsContainer, rootItems);
            }

            return rootItems;
        }

        private void AddContainerItems(CompoundItem container, List<Item> itemList)
        {
            if (container == null || container.Grids == null)
            {
                return;
            }

            foreach (var grid in container.Grids)
            {
                if (grid?.Items == null)
                {
                    continue;
                }

                foreach (var item in grid.Items.ToList())
                {
                    if (item != null && !itemList.Contains(item))
                    {
                        itemList.Add(item);
                    }
                }
            }
        }

        private void DrawGearTransferWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Select Items to Extract", _headerStyle);
            GUILayout.Space(10);

            var (totalCells, usedCells, freeCells) = CalculateTransferStashSpace();
            int selectedItemsSize = CalculateSelectedItemsSize();

            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"Transfer Capacity: {usedCells}/{totalCells} cells ({freeCells} free)", _labelStyle);

            float usagePercent = totalCells > 0 ? (float)usedCells / totalCells : 0f;
            Color barColor = usagePercent > 0.8f ? Color.red : (usagePercent > 0.5f ? Color.yellow : Color.green);

            Rect progressRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, GUILayout.Height(20));
            GUI.Box(progressRect, "");

            Rect fillRect = new Rect(
                progressRect.x + 2,
                progressRect.y + 2,
                (progressRect.width - 4) * usagePercent,
                progressRect.height - 4
            );
            GUI.DrawTexture(fillRect, MakeTex(2, 2, barColor));

            GUILayout.Space(5);

            if (selectedItemsSize > 0)
            {
                bool willFit = selectedItemsSize <= TRANSFER_MAX_CELLS;
                Color sizeColor = willFit ? Color.green : Color.red;
                string fitMessage = willFit ? "? Will fit" : "? Too large!";

                GUILayout.Label(
                    $"Selected items size: {selectedItemsSize} cells - {fitMessage}",
                    new GUIStyle(_labelStyle) { normal = { textColor = sizeColor } }
                );

                if (!willFit)
                {
                    int overflow = selectedItemsSize - TRANSFER_MAX_CELLS;
                    GUILayout.Label(
                        $"Need to remove {overflow} cells worth of items",
                        new GUIStyle(_labelStyle) { normal = { textColor = Color.red }, fontSize = 11 }
                    );
                }
            }

            GUILayout.Label(
                $"Items will be sent to your stash (max {TRANSFER_GRID_WIDTH}x{TRANSFER_GRID_HEIGHT} grid)",
                new GUIStyle(_labelStyle) { fontSize = 11 }
            );

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.Label($"Selected: {_selectedItemIds.Count} items", _labelStyle);
            GUILayout.Space(10);

            _gearScrollPosition = GUILayout.BeginScrollView(_gearScrollPosition, GUILayout.ExpandHeight(true));

            if (_transferableItems == null || _transferableItems.Count == 0)
            {
                GUILayout.Label("No items available to transfer", _labelStyle);
            }
            else
            {
                foreach (var item in _transferableItems)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    GUILayout.BeginHorizontal(GUI.skin.box);

                    bool isSelected = _selectedItemIds.Contains(item.Id);
                    bool newSelected = GUILayout.Toggle(isSelected, "", GUILayout.Width(20));

                    if (newSelected != isSelected)
                    {
                        if (newSelected)
                        {
                            _selectedItemIds.Add(item.Id);
                        }
                        else
                        {
                            _selectedItemIds.Remove(item.Id);
                        }
                    }

                    string itemName = item.Name?.Localized() ?? item.LocalizedName() ?? "Unknown Item";
                    int stackSize = item.StackObjectsCount > 0 ? item.StackObjectsCount : 1;

                    GUILayout.Label($"{itemName} x{stackSize}", _labelStyle, GUILayout.ExpandWidth(true));

                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            var (totalCells2, usedCells2, freeCells2) = CalculateTransferStashSpace();
            bool canTransfer = usedCells2 <= TRANSFER_MAX_CELLS && _selectedItemIds.Count > 0;

            GUI.enabled = canTransfer;
            if (
                GUILayout.Button(
                    "Transfer Selected",
                    canTransfer ? _buttonStyle : _disabledButtonStyle,
                    GUILayout.Height(40),
                    GUILayout.ExpandWidth(true)
                )
            )
            {
                TransferSelectedItems();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Cancel", _buttonStyle, GUILayout.Height(40), GUILayout.Width(100)))
            {
                _showGearTransferUI = false;
                _selectedItemIds.Clear();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("Press ESC to go back", _labelStyle);

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void TransferSelectedItems()
        {
            if (_selectedItemIds == null || _selectedItemIds.Count == 0)
            {
                return;
            }

            try
            {
                int selectedItemsSize = CalculateSelectedItemsSize();
                if (selectedItemsSize > TRANSFER_MAX_CELLS)
                {
                    return;
                }

                var itemsToTransfer = new List<Item>();
                foreach (var itemId in _selectedItemIds.ToList())
                {
                    var item = _transferableItems.FirstOrDefault(i => i.Id == itemId);
                    if (item != null)
                    {
                        itemsToTransfer.Add(item);
                    }
                }

                if (itemsToTransfer.Count == 0)
                {
                    return;
                }

                if (!RemoveCurrency("SpecialReqForms", 1))
                {
                    Plugin._log.LogError("Failed to consume Special Requisition Form for gear transfer");
                    return;
                }

                var flattenedItems = Singleton<ItemFactoryClass>.Instance.TreeToFlatItems(itemsToTransfer);

                int totalItemCount = flattenedItems.Count();

                RequestHandler.PutJson(
                    "/RaidOverhaul/TransferItemRequests",
                    new
                    {
                        items = flattenedItems,
                        traderId = Utils.Traders.TryGetValue("ReqShop", out var tId) ? tId : null,
                        message = GetResponseMessage(),
                    }.ToJson(_defaultJsonConverters)
                );

                foreach (var item in itemsToTransfer)
                {
                    RemoveZeroStackItem(ROPlayer, item);
                }

                Plugin._log.LogInfo($"Successfully transferred {itemsToTransfer.Count} items ({selectedItemsSize} cells) to stash");
            }
            catch (Exception ex)
            {
                Plugin._log.LogError($"Error transferring items: {ex.Message}");
            }

            _showGearTransferUI = false;
            _selectedItemIds.Clear();
            CloseMenu();
        }

        private string GetResponseMessage()
        {
            var messages = new List<string>
            {
                "Your items have been delivered. Don't forget to leave a tip!",
                "Items received and returned to base.",
                "Holy shit, you had a good haul there. We got everything back in one piece for you.",
                "Come on, you won't even leave a tip for us? Beer? Pizza? Nothing? Stingy prick.",
                "Everything is back to your base and we definitely didn't bring any souvenirs home with us.",
                "We've received your crate and are en route to base. Remember to call us up anytime you get in a pinch.",
            };
            return messages[UnityEngine.Random.Range(0, messages.Count)];
        }
    }
}
