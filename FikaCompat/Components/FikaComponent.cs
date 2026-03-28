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
using RaidOverhaul.FikaModule.Packets;
using RaidOverhaul.Helpers;

namespace RaidOverhaul.FikaModule.Components
{
    internal class FikaComponent
    {
        private static readonly MethodInfo DoorUnlockMethod = typeof(Door).GetMethod("Unlock", BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo DoorOpenMethod = typeof(Door).GetMethod("Open", BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo DoorCloseMethod = typeof(Door).GetMethod("Close", BindingFlags.Instance | BindingFlags.Public);
        private static readonly MethodInfo KeycardDoorUnlockMethod = typeof(KeycardDoor).GetMethod(
            "Unlock",
            BindingFlags.Instance | BindingFlags.Public
        );
        private static readonly MethodInfo KeycardDoorOpenMethod = typeof(KeycardDoor).GetMethod(
            "Open",
            BindingFlags.Instance | BindingFlags.Public
        );
        private static readonly MethodInfo SwitchOpenMethod = typeof(Switch).GetMethod("Open", BindingFlags.Instance | BindingFlags.Public);

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

        private static void ReceiveRandomEventPacket(RandomEventSyncPacket packet, NetPeer peer)
        {
            switch (packet.EventToRun)
            {
                case Utils.Heal:
                    Plugin._ecScript.DoHealPlayer();
                    break;
                case Utils.Damage:
                    Plugin._ecScript.DoDamageEvent();
                    break;
                case Utils.Repair:
                    Plugin._ecScript.DoArmorRepair();
                    break;
                case Utils.Airdrop:
                    NotificationManagerClass.DisplayMessageNotification(
                        "Aidrop Event: Incoming Airdrop!",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                    break;
                case Utils.Jokes:
                    Plugin._ecScript.DoFunnyWrapper();
                    break;
                case Utils.Blackout:
                    Plugin._ecScript.DoBlackoutEventWrapper();
                    break;
                case Utils.Skill:
                    Plugin._ecScript.DoSkillEvent();
                    break;
                case Utils.Metabolism:
                    Plugin._ecScript.DoMetabolismEvent();
                    break;
                case Utils.Malf:
                    Plugin._ecScript.DoMalfEventWrapper();
                    break;
                case Utils.LoyaltyLevel:
                    Plugin._ecScript.DoLLEvent();
                    break;
                case Utils.Berserk:
                    Plugin._ecScript.DoBerserkEventWrapper();
                    break;
                case Utils.Weight:
                    Plugin._ecScript.DoWeightEventWrapper();
                    break;
                case Utils.MaxLoyaltyLevel:
                    Plugin._ecScript.DoMaxLLEvent();
                    break;
                case Utils.CorrectRep:
                    Plugin._ecScript.CorrectRep();
                    break;
                case Utils.Lockdown:
                    Plugin._ecScript.DoLockDownEventWrapper();
                    break;
                case Utils.GearExfilEvent:
                    NotificationManagerClass.DisplayMessageNotification(
                        "Gear Exfil Event: Host has activated the gear exfil event. \nHunker down and protect them until their gear is safely locked away.",
                        ENotificationDurationType.Long,
                        ENotificationIconType.Quest
                    );
                    break;
                case Utils.Train:
                    Plugin._ecScript.RunTrainWrapper();
                    break;
                case Utils.PmcExfil:
                    Plugin._ecScript.DoPmcExfilEventWrapper();
                    break;
                case Utils.Artillery:
                    Plugin._ecScript.DoArtyEventWrapper();
                    break;
            }

            if (
                Plugin.ROGameWorld != null
                && Plugin.ROGameWorld.AllAlivePlayersList != null
                && Plugin.ROGameWorld.AllAlivePlayersList.Count > 0
                && (Plugin.ROPlayer is not HideoutPlayer)
            )
            {
                Plugin._ecScript.CleanForNewEvent();
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
        }

        public static void InitOnPluginEnabled()
        {
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnFikaNetManagerCreated);
        }
    }
}
