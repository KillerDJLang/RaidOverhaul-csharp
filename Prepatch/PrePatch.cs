using System.Reflection;
using BepInEx;
using RaidOverhaulPrepatch.Helpers;

[assembly: AssemblyTitle(ClientInfo.ROPrepatchName)]
[assembly: AssemblyDescription(ClientInfo.ROPrepatchDescription)]
[assembly: AssemblyCopyright(ClientInfo.ROCopyright)]
[assembly: AssemblyFileVersion(ClientInfo.PluginVersion)]

namespace RaidOverhaulPrepatch
{
    [BepInDependency("com.morebotsapiprepatch.tacticaltoaster", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ClientInfo.ROPrepatchGUID, ClientInfo.ROPrepatchName, ClientInfo.PluginVersion)]
    public class RaidOverhaulPrepatch : BaseUnityPlugin
    {
        public static RaidOverhaulPrepatch Instance;

        public void Awake()
        {
            Instance = this;
        }
    }
}
