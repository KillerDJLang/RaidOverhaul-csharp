using System.Reflection;
using Comfort.Common;
using Fika.Core.Main.Utils;
using Fika.Core.Networking;
using RaidOverhaul.Fika;
using RaidOverhaul.FikaModule.Components;

[assembly: AssemblyTitle("Raid Overhaul Fika Addon")]
[assembly: AssemblyDescription("Fika Packets for Raid Overhaul.")]
[assembly: AssemblyCopyright("Copyright © 2025 nameless")]
[assembly: AssemblyFileVersion("3.0.2")]

namespace RaidOverhaul.FikaModule
{
    internal class FikaMain
    {
        public static void Init()
        {
            PluginAwake();
            FikaBridge.PluginEnableEmitted += PluginEnable;

            FikaBridge.AmHostEmitted += AmHost;
            FikaBridge.GetRaidIdEmitted += GetRaidId;

            FikaBridge.SendFlareEventRunPacketEmitted += FikaComponent.SendFlareEventPacket;
            FikaBridge.SendRandomEventRunPacketEmitted += FikaComponent.SendRandomEventPacket;
            FikaBridge.SendDoorStateChangePacketEmitted += FikaComponent.SendDoorStateChangePacket;
            FikaBridge.SendKeycardDoorStateChangePacketEmitted += FikaComponent.SendSwitchStateChangePacket;
            FikaBridge.SendSwitchStateChangePacketEmitted += FikaComponent.SendKeycardDoorStateChangePacket;
            FikaBridge.SendRaidStartDoorStateChangePacketEmitted += FikaComponent.SendRaidStartDoorStateChangePacket;
            FikaBridge.SendRaidStartLampStateChangePacketEmitted += FikaComponent.SendRaidStartLampStateChangePacket;
        }

        private static void PluginAwake() { }

        private static void PluginEnable()
        {
            FikaComponent.InitOnPluginEnabled();
        }

        private static bool AmHost()
        {
            return Singleton<FikaServer>.Instantiated;
        }

        private static string GetRaidId()
        {
            return FikaBackendUtils.GroupId;
        }
    }
}
