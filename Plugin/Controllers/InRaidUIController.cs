using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using RaidOverhaul.Managers;
using SPT.Common.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class InRaidUIController : MonoBehaviour
    {
        private bool _menuVisible;
        private bool _tookInputOwnership;
        private float _refreshTimer;

        private int _cachedReqCoins;
        private int _cachedReqSlips;
        private int _cachedSpecialForms;

        private EventController _eventController;

        private List<Item> _transferableItems;
        private HashSet<string> _selectedItemIds;

        private GameObject _menuPrefab;
        private GameObject _itemRowPrefab;
        private GameObject _menuInstance;
        private GameObject _gearTransferPanel;
        private GameObject _scrollView;
        private GameObject _itemContent;
        private List<GameObject> _itemRowInstances = new List<GameObject>();

        private TMP_Text _reqCoinsValue;
        private TMP_Text _reqSlipsValue;
        private TMP_Text _specialSlipsValue;

        private Button _emergencyExfilButton;
        private Button _trainButton;
        private Button _extractNowButton;
        private Button _extractGearButton;
        private Button _supportButton;
        private Button _transferButton;

        private TMP_Text _emergencyExfilError;
        private TMP_Text _trainError;
        private TMP_Text _extractNowError;
        private TMP_Text _extractGearError;
        private TMP_Text _supportError;
        private TMP_Text _transferError;

        private TMP_Text _capacityValueLabel;
        private Image _barFill;
        private AudioSource _audioSource;

        private const int TRANSFER_GRID_WIDTH = 5;
        private const int TRANSFER_GRID_HEIGHT = 10;
        private const int TRANSFER_MAX_CELLS = TRANSFER_GRID_WIDTH * TRANSFER_GRID_HEIGHT;

        private static readonly HashSet<string> _invalidTrainLocations = new HashSet<string>
        {
            "factory4_day",
            "factory4_night",
            "laboratory",
            "sandbox",
            "sandbox_high",
            "bigmap",
            "interchange",
            "labyrinth",
            "shoreline",
            "tarkovstreets",
            "woods",
        };

        private void Awake()
        {
            _menuPrefab = UICanvasPrefab;
            _itemRowPrefab = UIItemRowPrefab;
            _eventController = GetComponent<EventController>();
        }

        private void Update()
        {
            if (_menuVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (!ROPluginConfig.SpecialReqFeatures.Value || !Utils.IsInRaid())
                {
                    CloseMenu();
                    return;
                }

                _refreshTimer += Time.deltaTime;
                if (_refreshTimer >= 1f)
                {
                    _refreshTimer = 0f;
                    RefreshCurrencyCache();
                    UpdateCurrencyDisplay();
                    UpdateButtonStates();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (_gearTransferPanel != null && _gearTransferPanel.activeSelf)
                    {
                        _gearTransferPanel.SetActive(false);
                        _scrollView.SetActive(true);
                        _selectedItemIds?.Clear();
                        ClearItemList();
                    }
                    else
                    {
                        CloseMenu();
                    }
                    return;
                }
            }

            if (!ROPluginConfig.SpecialReqFeatures.Value || !Utils.IsInRaid())
            {
                return;
            }

            var keybind = ROPluginConfig.SpecialReqFeaturesBinding.Value;
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

        private int GetCurrencyCount(string currencyKey)
        {
            var session = GetSession();
            if (!Utils.IsInRaid() || session?.Profile?.Inventory == null)
            {
                return 0;
            }

            var allItems = session.Profile.Inventory.AllRealPlayerItems;

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

            if (!_invalidTrainLocations.Contains(ROPlayer.Location.ToLowerInvariant()))
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
            var session = GetSession();
            if (!Utils.IsInRaid() || session?.Profile?.Inventory == null)
            {
                return false;
            }

            var allItems = session.Profile.Inventory.AllRealPlayerItems;

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
            _refreshTimer = 0f;
            RefreshCurrencyCache();
            UpdateCurrencyDisplay();
            UpdateButtonStates();
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
            _selectedItemIds?.Clear();
            ClearItemList();
            if (_gearTransferPanel != null)
            {
                _gearTransferPanel.SetActive(false);
                _scrollView.SetActive(true);
            }
            if (_menuInstance != null)
            {
                _menuInstance.SetActive(false);
            }
        }

        public bool RemoveZeroStackItem(Player player, Item item)
        {
            TraderControllerClass inventoryController = player.InventoryController;

            GStruct154<GClass3408> result = InteractionsHandlerClass.Discard(item, inventoryController, simulate: false);

            if (result.Failed)
            {
                _log.LogError($"Failed to remove item: {result.Error}");
                return false;
            }

            result.Value.RaiseEvents(inventoryController, CommandStatus.Begin);
            result.Value.RaiseEvents(inventoryController, CommandStatus.Succeed);

            return true;
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
                _log.LogError($"Error calculating transfer stash space: {ex.Message}");
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

        private void WireUpUI()
        {
            var es = _menuInstance.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
            if (es != null && UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current != es)
            {
                es.gameObject.SetActive(false);
            }

            var bg = _menuInstance.transform.Find("Background");
            var content = bg.Find("Scroll View/Viewport/Content");

            _reqCoinsValue = bg.Find("CurrencyRow/ReqCoinsValue").GetComponent<TMP_Text>();
            _reqSlipsValue = bg.Find("CurrencyRow/ReqSlipsValue").GetComponent<TMP_Text>();
            _specialSlipsValue = bg.Find("CurrencyRow/SpecialSlipsValue").GetComponent<TMP_Text>();

            _emergencyExfilButton = content.Find("EmergencyExfilSection/ExfilButton").GetComponent<Button>();
            _emergencyExfilError = content.Find("EmergencyExfilSection/ErrorLabel").GetComponent<TMP_Text>();
            _trainButton = content.Find("TrainSection/ExfilButton").GetComponent<Button>();
            _trainError = content.Find("TrainSection/ErrorLabel").GetComponent<TMP_Text>();
            _extractNowButton = content.Find("ExtractNowSection/ExfilButton").GetComponent<Button>();
            _extractNowError = content.Find("ExtractNowSection/ErrorLabel").GetComponent<TMP_Text>();
            _extractGearButton = content.Find("ExtractGearSection/ExfilButton").GetComponent<Button>();
            _extractGearError = content.Find("ExtractGearSection/ErrorLabel").GetComponent<TMP_Text>();
            _supportButton = content.Find("SupportTeamSection/ExfilButton").GetComponent<Button>();
            _supportError = content.Find("SupportTeamSection/ErrorLabel").GetComponent<TMP_Text>();

            _gearTransferPanel = bg.Find("GearTransferPanel").gameObject;
            _scrollView = bg.Find("Scroll View").gameObject;
            _transferButton = _gearTransferPanel.transform.Find("TransferButton").GetComponent<Button>();
            _transferError = _gearTransferPanel.transform.Find("TransferErrorLabel").GetComponent<TMP_Text>();
            _capacityValueLabel = _gearTransferPanel.transform.Find("CapacityRow/CapacityValueLabel").GetComponent<TMP_Text>();
            _barFill = _gearTransferPanel.transform.Find("CapacityBar/BarFill").GetComponent<Image>();
            _itemContent = _gearTransferPanel.transform.Find("ItemScrollView/Viewport/Content").gameObject;

            _emergencyExfilButton.onClick.AddListener(() =>
            {
                PlayClick();
                OnEmergencyExfilClicked();
            });
            _trainButton.onClick.AddListener(() =>
            {
                PlayClick();
                OnTrainClicked();
            });
            _extractNowButton.onClick.AddListener(() =>
            {
                PlayClick();
                OnExtractNowClicked();
            });
            _extractGearButton.onClick.AddListener(() =>
            {
                PlayClick();
                OnExtractGearClicked();
            });
            _supportButton.onClick.AddListener(() =>
            {
                PlayClick();
                OnSupportClicked();
            });
            _transferButton.onClick.AddListener(() =>
            {
                PlayClick();
                OnTransferClicked();
            });
        }

        private void RefreshCurrencyCache()
        {
            _cachedReqCoins = GetCurrencyCount("ReqCoins");
            _cachedReqSlips = GetCurrencyCount("ReqSlips");
            _cachedSpecialForms = GetCurrencyCount("SpecialReqForms");
        }

        private void UpdateCurrencyDisplay()
        {
            _reqCoinsValue.text = _cachedReqCoins.ToString();
            _reqSlipsValue.text = _cachedReqSlips.ToString();
            _specialSlipsValue.text = _cachedSpecialForms.ToString();
        }

        private void SetButtonState(Button btn, TMP_Text errorLabel, bool canUse, string errorMsg = "")
        {
            btn.interactable = canUse;
            errorLabel.text = errorMsg;
            errorLabel.gameObject.SetActive(!canUse);
        }

        private void UpdateButtonStates()
        {
            bool onTrainMap = IsTrainAvailable();
            bool supportActive = SupportBotManager.Instance?.IsActive ?? false;

            SetButtonState(
                _emergencyExfilButton,
                _emergencyExfilError,
                _cachedReqSlips >= 10,
                $"Need 10 Req Slips (have {_cachedReqSlips})"
            );
            SetButtonState(
                _trainButton,
                _trainError,
                _cachedReqCoins >= 250 && onTrainMap,
                !onTrainMap ? "Train not available on this map" : $"Need 250 Req Coins (have {_cachedReqCoins})"
            );
            SetButtonState(_extractNowButton, _extractNowError, _cachedReqSlips >= 25, $"Need 25 Req Slips (have {_cachedReqSlips})");
            SetButtonState(_extractGearButton, _extractGearError, _cachedReqSlips >= 15, $"Need 15 Req Slips (have {_cachedReqSlips})");
            SetButtonState(
                _supportButton,
                _supportError,
                _cachedSpecialForms >= 1 && !supportActive,
                supportActive ? "Support team already active" : $"Need 1 Special Req Form (have {_cachedSpecialForms})"
            );
        }

        private void OnEmergencyExfilClicked()
        {
            if (!RemoveCurrency("ReqSlips", 10))
            {
                return;
            }
            _eventController.DoPmcExfilEventWrapper();
            CloseMenu();
        }

        private void OnTrainClicked()
        {
            if (!RemoveCurrency("ReqCoins", 250))
            {
                return;
            }
            _eventController.RunTrainWrapper();
            CloseMenu();
        }

        private void OnExtractNowClicked()
        {
            if (!RemoveCurrency("ReqSlips", 25))
            {
                return;
            }
            _eventController.ExfilNow();
            CloseMenu();
        }

        private void OnExtractGearClicked()
        {
            _selectedItemIds = [];
            _transferableItems = GetTransferableItems();
            PopulateItemList();
            _scrollView.SetActive(false);
            _gearTransferPanel.SetActive(true);
            UpdateCapacityDisplay();
        }

        private void OnSupportClicked()
        {
            var spawnPos = SupportBotManager.FindSpawnPosition(ROPlayer);
            if (spawnPos == Vector3.zero)
            {
                _supportError.text = "No valid spawn point nearby — move to a more open area.";
                _supportError.gameObject.SetActive(true);
                _supportButton.interactable = false;
                return;
            }
            if (!RemoveCurrency("SpecialReqForms", 1))
            {
                return;
            }

            if (FikaBridge.AmHost())
            {
                SupportBotManager.Instance?.Activate(ROPlayer, spawnPos);
            }
            else
            {
                FikaBridge.RequestSupportBotsPacket(ROPlayer.ProfileId, spawnPos.x, spawnPos.y, spawnPos.z);
            }

            CloseMenu();
        }

        private void OnTransferClicked()
        {
            TransferSelectedItems();
        }

        private void PopulateItemList()
        {
            ClearItemList();
            if (_transferableItems == null)
            {
                return;
            }
            foreach (var item in _transferableItems)
            {
                if (item == null)
                {
                    continue;
                }
                var row = Instantiate(_itemRowPrefab, _itemContent.transform);
                var toggle = row.transform.Find("ItemToggle").GetComponent<Toggle>();
                var nameLabel = row.transform.Find("ItemNameLabel").GetComponent<TMP_Text>();
                var stackLabel = row.transform.Find("ItemStackLabel").GetComponent<TMP_Text>();

                nameLabel.text = item.Name?.Localized() ?? item.LocalizedName() ?? "Unknown Item";
                stackLabel.text = $"x{(item.StackObjectsCount > 0 ? item.StackObjectsCount : 1)}";

                toggle.isOn = false;
                string capturedId = item.Id;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        _selectedItemIds.Add(capturedId);
                    }
                    else
                    {
                        _selectedItemIds.Remove(capturedId);
                    }
                    UpdateCapacityDisplay();
                });
                _itemRowInstances.Add(row);
            }
        }

        private void ClearItemList()
        {
            foreach (var row in _itemRowInstances)
            {
                if (row != null)
                {
                    Destroy(row);
                }
            }
            _itemRowInstances.Clear();
        }

        private void UpdateCapacityDisplay()
        {
            var (totalCells, usedCells, _) = CalculateTransferStashSpace();
            _capacityValueLabel.text = $"{usedCells} / {totalCells}";
            float fill = totalCells > 0 ? (float)usedCells / totalCells : 0f;
            _barFill.fillAmount = fill;
            _barFill.color =
                fill > 0.8f ? new Color(0.8f, 0.1f, 0.1f)
                : fill > 0.5f ? new Color(0.8f, 0.8f, 0.1f)
                : new Color(0.18f, 0.48f, 0.18f);

            bool canTransfer = usedCells <= totalCells && _selectedItemIds.Count > 0;
            SetButtonState(
                _transferButton,
                _transferError,
                canTransfer,
                usedCells > totalCells ? $"Too large — remove {usedCells - totalCells} cells worth of items" : "No items selected"
            );
        }

        private void OnDestroy()
        {
            ClearItemList();
            if (_menuInstance != null)
            {
                Destroy(_menuInstance);
            }
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

                var flattenedItems = Singleton<ItemFactoryClass>.Instance.TreeToFlatItems(itemsToTransfer);

                RequestHandler.PutJson(
                    "/RaidOverhaul/TransferItemRequests",
                    new
                    {
                        items = flattenedItems,
                        traderId = Utils.Traders.TryGetValue("ReqShop", out var tId) ? tId : null,
                        message = GetResponseMessage(),
                    }.ToJson(null)
                );

                foreach (var item in itemsToTransfer)
                {
                    RemoveZeroStackItem(ROPlayer, item);
                }

                if (!RemoveCurrency("ReqSlips", 15))
                {
                    _log.LogError("Failed to consume Requisition Slips for gear transfer after successful transfer");
                }

                _log.LogInfo($"Successfully transferred {itemsToTransfer.Count} items ({selectedItemsSize} cells) to stash");
                _audioSource?.PlayOneShot(SoundBeepGreen);
            }
            catch (Exception ex)
            {
                _log.LogError($"Error transferring items: {ex.Message}");
            }

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
