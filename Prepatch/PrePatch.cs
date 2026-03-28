using System.Reflection;
using BepInEx;
using LegionPrepatch.Helpers;

[assembly: AssemblyTitle(ClientInfo.ROPrepatchName)]
[assembly: AssemblyDescription(ClientInfo.ROPrepatchDescription)]
[assembly: AssemblyCopyright(ClientInfo.ROCopyright)]
[assembly: AssemblyFileVersion(ClientInfo.PluginVersion)]

namespace LegionPrepatch
{
    [BepInDependency("com.morebotsapiprepatch.tacticaltoaster", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ClientInfo.ROPrepatchGUID, ClientInfo.ROPrepatchName, ClientInfo.PluginVersion)]
    public class LegionPrepatch : BaseUnityPlugin
    {
        public static LegionPrepatch Instance;

        public void Awake()
        {
            Instance = this;
        }
    }
}
