using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EFT.Communications;
using EFT.Interactive;
using RaidOverhaul.Configs;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using UnityEngine;

namespace RaidOverhaul.Controllers
{
    internal class DoorController : MonoBehaviour
    {
        private Switch[] _switches;
        private Door[] _doors;
        private KeycardDoor[] _keycardDoors;
        private int _switchIndex;
        private int _doorIndex;
        private int _keycardDoorIndex;

        private bool _dooreventisRunning;
        private bool _objectsCached;
        private readonly System.Random _random = new System.Random();

        private static int _doorChangedCount;
        private static int _doorNotChangedCount;
        private static int _lampCount;
        private bool _isReady;

        private static readonly HashSet<string> _switchMaps = new HashSet<string>(System.StringComparer.Ordinal)
        {
            "laboratory",
            "rezervbase",
            "bigmap",
            "interchange",
        };

        private static readonly HashSet<string> _keycardMaps = new HashSet<string>(System.StringComparer.Ordinal)
        {
            "laboratory",
            "interchange",
        };

        public void ManualUpdate()
        {
            _isReady = Utils.IsInRaid();

            if (!_isReady || !DJConfig.EnableDoorEvents.Value)
            {
                ResetEventState();
                return;
            }

            if (!_objectsCached)
            {
                _switches = FindObjectsOfType<Switch>();
                _doors = FindObjectsOfType<Door>();
                _keycardDoors = FindObjectsOfType<KeycardDoor>();
                _switchIndex = _switches.Length;
                _doorIndex = _doors.Length;
                _keycardDoorIndex = _keycardDoors.Length;
                _objectsCached = true;
            }

            if (!_dooreventisRunning && FikaBridge.AmHost())
            {
                DoorEvents().Forget();
                _dooreventisRunning = true;
            }
        }

        private void ResetEventState()
        {
            _objectsCached = false;
            _lampCount = 0;
            _doorChangedCount = 0;
            _doorNotChangedCount = 0;
        }

        private async UniTaskVoid DoorEvents()
        {
            await UniTask.WaitForSeconds(
                Random.Range(
                    ConfigController.EventConfig.DoorEventRangeMinimumServer,
                    ConfigController.EventConfig.DoorEventRangeMaximumServer
                ) * 60f
            );

            if (_isReady && FikaBridge.AmHost())
            {
                Weighting.DoRandomEvent(Weighting.WeightedDoorMethods);
            }
            else
            {
                _objectsCached = false;
            }

            _dooreventisRunning = false;
        }

        #region Door Event Controller

        public void PowerOn()
        {
            if (!_switchMaps.Contains(Plugin.ROPlayer.Location))
            {
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Plugin._log.LogInfo("No switches available on this map, returning.");
                }
                return;
            }

            if (_switches == null || _switchIndex <= 0)
            {
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Plugin._log.LogInfo("No switches left to open, returning.");
                }
                return;
            }

            var selection = _random.Next(_switchIndex);
            var _switch = _switches[selection];

            if (_switch.DoorState == EDoorState.Shut)
            {
                FikaBridge.SendSwitchStateChangePacket(_switch.Id);
                _switch.Open();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("A random switch has been thrown.");
                }
            }

            _switchIndex--;
            if (selection < _switchIndex)
            {
                _switches[selection] = _switches[_switchIndex];
            }
        }

        public void DoUnlock()
        {
            if (_doors == null || _doorIndex <= 0)
            {
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Plugin._log.LogInfo("No locked doors available, returning.");
                }
                return;
            }

            var selection = _random.Next(_doorIndex);
            var door = _doors[selection];

            if (door.gameObject.layer != LayerMaskClass.InteractiveLayer)
            {
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Plugin._log.LogInfo("Chosen door isn't on the interactive layer, returning.");
                }
                return;
            }

            if (door.DoorState == EDoorState.Locked && door.Operatable && door.enabled)
            {
                FikaBridge.SendDoorStateChangePacket(door.Id);
                door.Unlock();
                door.Open();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("A random door has been unlocked.");
                }
            }

            _doorIndex--;
            if (selection < _doorIndex)
            {
                _doors[selection] = _doors[_doorIndex];
            }
        }

        public void DoKUnlock()
        {
            if (!_keycardMaps.Contains(Plugin.ROPlayer.Location))
            {
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Plugin._log.LogInfo("No keycard doors available on this map, returning.");
                }
                return;
            }

            if (_keycardDoors == null || _keycardDoorIndex <= 0)
            {
                if (ConfigController.DebugConfig.DebugMode)
                {
                    Plugin._log.LogInfo("No keycard doors left to open, returning.");
                }
                return;
            }

            var selection = _random.Next(_keycardDoorIndex);
            var kdoor = _keycardDoors[selection];

            if (kdoor.DoorState == EDoorState.Locked)
            {
                FikaBridge.SendKeycardDoorStateChangePacket(kdoor.Id);
                kdoor.Unlock();
                kdoor.Open();

                if (ConfigController.DebugConfig.DebugMode)
                {
                    Utils.LogToServerConsole("A random keycard door has been unlocked.");
                }
            }

            _keycardDoorIndex--;
            if (selection < _keycardDoorIndex)
            {
                _keycardDoors[selection] = _keycardDoors[_keycardDoorIndex];
            }
        }

        #endregion

        #region Random Raid Start Events

        public static void RandomizeDefaultDoors()
        {
            if (!DJConfig.EnableRaidStartEvents.Value && Plugin.ROPlayer.Location == "laboratory")
            {
                return;
            }

            var doors = FindObjectsOfType<Door>();
            var interactiveLayer = LayerMaskClass.InteractiveLayer;

            foreach (var door in doors)
            {
                if (!door.Operatable || !door.enabled)
                {
                    _doorNotChangedCount++;
                    continue;
                }

                var doorState = door.DoorState;
                if (doorState != EDoorState.Shut && doorState != EDoorState.Open)
                {
                    _doorNotChangedCount++;
                    continue;
                }

                if (doorState == EDoorState.Locked)
                {
                    _doorNotChangedCount++;
                    continue;
                }

                if (door.gameObject.layer != interactiveLayer)
                {
                    _doorNotChangedCount++;
                    continue;
                }

                var randomValue = Random.Range(0, 100);

                if (randomValue < 50)
                {
                    if (doorState == EDoorState.Shut)
                    {
                        FikaBridge.SendRaidStartDoorStateChangePacket(door.Id);
                        door.Open();
                        _doorChangedCount++;
                    }
                    else if (doorState == EDoorState.Open)
                    {
                        FikaBridge.SendRaidStartDoorStateChangePacket(door.Id);
                        door.Close();
                        _doorChangedCount++;
                    }
                }
            }

            if (!ConfigController.DebugConfig.DebugMode)
            {
                return;
            }

            NotificationManagerClass.DisplayMessageNotification(
                $"[{_doorChangedCount}] total Doors have had their states changed. [{_doorNotChangedCount}] haven't been modified.",
                ENotificationDurationType.Long
            );
            Utils.LogToServerConsole(
                $"[{_doorChangedCount}] total Doors have had their states changed. [{_doorNotChangedCount}] haven't been modified."
            );
        }

        public static void RandomizeLampState()
        {
            if (!DJConfig.EnableRaidStartEvents.Value && Plugin.ROPlayer.Location == "laboratory")
            {
                return;
            }

            var lamps = FindObjectsOfType<LampController>();

            foreach (var lamp in lamps)
            {
                if (Random.Range(0, 100) < 25)
                {
                    FikaBridge.SendRaidStartLampStateChangePacket(lamp.Id);
                    lamp.Switch(Turnable.EState.Off);
                    lamp.enabled = false;
                    _lampCount++;
                }
            }

            if (!ConfigController.DebugConfig.DebugMode)
            {
                return;
            }

            NotificationManagerClass.DisplayMessageNotification(
                $"[{_lampCount}] total Lamps have been modified.",
                ENotificationDurationType.Long
            );
            Utils.LogToServerConsole($"[{_lampCount}] total Lamps have been modified.");
        }
        #endregion
    }
}
