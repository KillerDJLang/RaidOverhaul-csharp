using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Mono.Cecil;
using MoreBotsAPI;

namespace LegionPrepatch.Patches
{
    public static class LegionWildSpawnTypePatch
    {
        public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

        public static void Patch(ref AssemblyDefinition assembly)
        {
            if (!ShouldPatchAssembly())
            {
                Logger
                    .CreateLogSource("Raid Overhaul PrePatch")
                    .LogWarning(
                        "Raid Overhaul plugin not detected, not patching assembly. Make sure you have installed or uninstalled the mod correctly."
                    );
                return;
            }

            var brains = new List<string>() { "PMC", "ExUsec" };
            var layers = new List<string>() { "Request", "KnightFight", "PmcBear", "PmcUsec", "ExURequest", "StationaryWS" };
            int baseBrainInt = 24;

            // lead
            var bot = new CustomWildSpawnType(199, "bosslegion", "Legion", baseBrainInt, true, true, false);

            bot.SetCountAsBossForStatistics(false);
            bot.SetShouldUseFenceNoBossAttack(false, false);
            bot.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            SAINSettings settings = new SAINSettings(bot.WildSpawnTypeValue)
            {
                Name = "Legion",
                Description = "Leader of the Legion.",
                Section = "Legion",
                BaseBrain = "PMC",
                BrainsToApply = brains,
                LayersToRemove = layers,
            };

            bot.SetSAINSettings(settings);

            CustomWildSpawnTypeManager.RegisterWildSpawnType(bot, assembly);

            // assault
            bot = new CustomWildSpawnType(200, "legionnaire", "Legion", baseBrainInt, true, true, false);

            bot.SetCountAsBossForStatistics(false);
            bot.SetShouldUseFenceNoBossAttack(false, false);
            bot.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            settings = new SAINSettings(bot.WildSpawnTypeValue)
            {
                Name = "Legionnaire",
                Description = "A combat member of the Legion. Fights tooth and nail to protect their leader.",
                Section = "Legion",
                BaseBrain = "PMC",
                BrainsToApply = brains,
                LayersToRemove = layers,
            };

            bot.SetSAINSettings(settings);

            CustomWildSpawnTypeManager.RegisterWildSpawnType(bot, assembly);
            CustomWildSpawnTypeManager.AddSuitableGroup(new List<int> { 199, 200 });
        }

        private static bool ShouldPatchAssembly()
        {
            var patcherLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var bepDir = Directory.GetParent(patcherLoc);
            var modDllLoc = Path.Combine(bepDir.FullName, "plugins", "RaidOverhaul", "RO-Plugin.dll");

            return File.Exists(modDllLoc);
        }
    }
}
