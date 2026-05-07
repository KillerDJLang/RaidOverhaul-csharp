using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.Interactive;
using Fika.Core.Main.Utils;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using HarmonyLib;
using RaidOverhaul.FikaModule.Packets;
using RaidOverhaul.Helpers;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.FikaModule.Components
{
    internal class FikaComponent
    {
        private static readonly MethodInfo DoorUnlockMethod = AccessTools.Method(typeof(Door), nameof(Door.Unlock));
        private static readonly MethodInfo DoorOpenMethod = AccessTools.Method(typeof(Door), nameof(Door.Open));
        private static readonly MethodInfo DoorCloseMethod = AccessTools.Method(typeof(Door), nameof(Door.Close));
        private static readonly MethodInfo KeycardDoorUnlockMethod = AccessTools.Method(typeof(KeycardDoor), nameof(KeycardDoor.Unlock));
        private static readonly MethodInfo KeycardDoorOpenMethod = AccessTools.Method(typeof(KeycardDoor), nameof(KeycardDoor.Open));
        private static readonly MethodInfo SwitchOpenMethod = AccessTools.Method(typeof(Switch), nameof(Switch.Open));

        public static bool AmHost()
        {
            return Singleton<FikaServer>.Instantiated;
        }

        public static string GetRaidId()
        {
            return FikaBackendUtils.GroupId;
        }

        public static void SendRandomEventPacket(string eventToSend)
        {
            if (Singleton<FikaServer>.Instantiated)
            {
                var packet = new RandomEventSyncPacket { EventToRun = eventToSend };
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendFlareEventPacket(string flareEventToSend)
        {
            var packet = new RandomEventSyncPacket { EventToRun = flareEventToSend };

            if (Singleton<FikaServer>.Instantiated)
            {
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }

            if (Singleton<FikaClient>.Instantiated)
            {
                Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendDoorStateChangePacket(string doorId)
        {
            if (Singleton<FikaServer>.Instantiated)
            {
                var packet = new DoorEventSyncPacket { DoorId = doorId };
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendSwitchStateChangePacket(string switchId)
        {
            if (Singleton<FikaServer>.Instantiated)
            {
                var packet = new SwitchEventSyncPacket { SwitchId = switchId };
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendKeycardDoorStateChangePacket(string keycardDoorId)
        {
            if (Singleton<FikaServer>.Instantiated)
            {
                var packet = new KeycardDoorEventSyncPacket { KeycardDoorId = keycardDoorId };
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendRaidStartDoorStateChangePacket(string doorId)
        {
            if (Singleton<FikaServer>.Instantiated)
            {
                var packet = new RaidStartDoorStateSyncPacket { DoorId = doorId };
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendRaidStartLampStateChangePacket(string lampId)
        {
            if (Singleton<FikaServer>.Instantiated)
            {
                var packet = new RaidStartLampStateSyncPacket { LampId = lampId };
                Singleton<FikaServer>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void SendRequestSupportBotsPacket(string callerProfileId, float spawnX, float spawnY, float spawnZ)
        {
            if (Singleton<FikaClient>.Instantiated)
            {
                var packet = new RequestSupportBotsPacket
                {
                    CallerProfileId = callerProfileId,
                    SpawnX = spawnX,
                    SpawnY = spawnY,
                    SpawnZ = spawnZ,
                };
                Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        private static void ReceiveRequestSupportBotsPacket(RequestSupportBotsPacket packet, NetPeer peer)
        {
            if (!Singleton<FikaServer>.Instantiated)
            {
                return;
            }

            var gameWorld = Plugin.ROGameWorld;
            if (gameWorld == null)
            {
                return;
            }

            Player callerPlayer = null;
            foreach (var registeredPlayer in gameWorld.RegisteredPlayers)
            {
                if (registeredPlayer == null || registeredPlayer.IsAI)
                {
                    continue;
                }

                if (registeredPlayer.ProfileId == packet.CallerProfileId)
                {
                    callerPlayer = registeredPlayer as Player;
                    break;
                }
            }

            if (callerPlayer == null)
            {
                return;
            }

            var spawnPos = new Vector3(packet.SpawnX, packet.SpawnY, packet.SpawnZ);
            SupportBotManager.Instance?.Activate(callerPlayer, spawnPos);
        }

        private static void ReceiveRandomEventPacket(RandomEventSyncPacket packet, NetPeer peer)
        {
            switch (packet.EventToRun)
            {
                case Utils.Heal:
                    Plugin.GetEventController()?.DoHealPlayer();
                    break;
                case Utils.Damage:
                    Plugin.GetEventController()?.DoDamageEvent();
                    break;
                case Utils.Repair:
                    Plugin.GetEventController()?.DoArmorRepair();
                    break;
                case Utils.Airdrop:
                    NotificationManagerClass.DisplayMessageNotification(
                        "Aidrop Event: Incoming Airdrop!",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                    break;
                case Utils.Jokes:
                    Plugin.GetEventController()?.DoFunnyWrapper();
                    break;
                case Utils.Blackout:
                    Plugin.GetEventController()?.DoBlackoutEventWrapper();
                    break;
                case Utils.Skill:
                    Plugin.GetEventController()?.DoSkillEvent();
                    break;
                case Utils.Metabolism:
                    Plugin.GetEventController()?.DoMetabolismEvent();
                    break;
                case Utils.Malf:
                    Plugin.GetEventController()?.DoMalfEventWrapper();
                    break;
                case Utils.LoyaltyLevel:
                    Plugin.GetEventController()?.DoLLEvent();
                    break;
                case Utils.Berserk:
                    Plugin.GetEventController()?.DoBerserkEventWrapper();
                    break;
                case Utils.Weight:
                    Plugin.GetEventController()?.DoWeightEventWrapper();
                    break;
                case Utils.MaxLoyaltyLevel:
                    Plugin.GetEventController()?.DoMaxLLEvent();
                    break;
                case Utils.CorrectRep:
                    Plugin.GetEventController()?.CorrectRep();
                    break;
                case Utils.Lockdown:
                    Plugin.GetEventController()?.DoLockDownEventWrapper();
                    break;
                case Utils.GearExfilEvent:
                    NotificationManagerClass.DisplayMessageNotification(
                        "Gear Exfil Event: Host has activated the gear exfil event. \nHunker down and protect them until their gear is safely locked away.",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                    break;
                case Utils.Train:
                    Plugin.GetEventController()?.RunTrainWrapper();
                    break;
                case Utils.PmcExfil:
                    Plugin.GetEventController()?.DoPmcExfilEventWrapper();
                    break;
                case Utils.Artillery:
                    Plugin.GetEventController()?.DoArtyEventWrapper();
                    break;
                case Utils.Hunted:
                    NotificationManagerClass.DisplayMessageNotification(
                        "You have been marked. Every hostile in the raid is now hunting you down.",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Alert
                    );
                    break;
                case Utils.ExfilNow:
                    Plugin.GetEventController()?.ExfilNow();
                    break;
            }

            if (
                Plugin.ROGameWorld != null
                && Plugin.ROGameWorld.AllAlivePlayersList != null
                && Plugin.ROGameWorld.AllAlivePlayersList.Count > 0
                && (Plugin.ROPlayer is not HideoutPlayer)
            )
            {
                Plugin.GetEventController()?.CleanForNewEvent();
            }
        }

        private static void ReceiveDoorStateChangePacket(DoorEventSyncPacket packet, NetPeer peer)
        {
            var door = ROSession.GetDoorById(packet.DoorId);

            if (door != null && door.DoorState == EDoorState.Locked && door.Operatable && door.enabled)
            {
                DoorUnlockMethod.Invoke(door, null);
                DoorOpenMethod.Invoke(door, null);
            }
        }

        private static void ReceiveKeycardDoorStateChangePacket(KeycardDoorEventSyncPacket packet, NetPeer peer)
        {
            var kDoor = ROSession.GetKeycardDoorById(packet.KeycardDoorId);

            if (kDoor != null && kDoor.DoorState == EDoorState.Locked && kDoor.Operatable && kDoor.enabled)
            {
                KeycardDoorUnlockMethod.Invoke(kDoor, null);
                KeycardDoorOpenMethod.Invoke(kDoor, null);
            }
        }

        private static void ReceiveSwitchStateChangePacket(SwitchEventSyncPacket packet, NetPeer peer)
        {
            var pSwitch = ROSession.GetSwitchById(packet.SwitchId);

            if (pSwitch != null && pSwitch.DoorState == EDoorState.Shut)
            {
                SwitchOpenMethod.Invoke(pSwitch, null);
            }
        }

        private static void ReceiveRaidStartDoorStateChangePacket(RaidStartDoorStateSyncPacket packet, NetPeer peer)
        {
            var door = ROSession.GetDoorById(packet.DoorId);

            if (door == null)
            {
                return;
            }

            if (door.DoorState == EDoorState.Shut)
            {
                DoorOpenMethod.Invoke(door, null);
            }
            else if (door.DoorState == EDoorState.Open)
            {
                DoorCloseMethod.Invoke(door, null);
            }
        }

        private static void ReceiveRaidStartLampStateChangePacket(RaidStartLampStateSyncPacket packet, NetPeer peer)
        {
            var lamp = ROSession.GetLampById(packet.LampId);

            if (lamp != null)
            {
                lamp.Switch(Turnable.EState.Off);
                lamp.enabled = false;
            }
        }

        private static void OnFikaNetManagerCreated(FikaNetworkManagerCreatedEvent managerCreatedEvent)
        {
            managerCreatedEvent.Manager.RegisterPacket<RandomEventSyncPacket, NetPeer>(ReceiveRandomEventPacket);
            managerCreatedEvent.Manager.RegisterPacket<DoorEventSyncPacket, NetPeer>(ReceiveDoorStateChangePacket);
            managerCreatedEvent.Manager.RegisterPacket<KeycardDoorEventSyncPacket, NetPeer>(ReceiveKeycardDoorStateChangePacket);
            managerCreatedEvent.Manager.RegisterPacket<SwitchEventSyncPacket, NetPeer>(ReceiveSwitchStateChangePacket);
            managerCreatedEvent.Manager.RegisterPacket<RaidStartDoorStateSyncPacket, NetPeer>(ReceiveRaidStartDoorStateChangePacket);
            managerCreatedEvent.Manager.RegisterPacket<RaidStartLampStateSyncPacket, NetPeer>(ReceiveRaidStartLampStateChangePacket);
            managerCreatedEvent.Manager.RegisterPacket<RequestSupportBotsPacket, NetPeer>(ReceiveRequestSupportBotsPacket);
        }

        public static void InitOnPluginEnabled()
        {
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnFikaNetManagerCreated);
        }
    }
}
