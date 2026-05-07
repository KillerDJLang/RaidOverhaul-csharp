using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Mono.Cecil;
using MoreBotsAPI;

namespace RaidOverhaulPrepatch.Patches
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
            int baseBrainInt = 9;

            var legion = new CustomWildSpawnType(199, "bosslegion", "Legion", baseBrainInt, true, true, false);
            legion.SetCountAsBossForStatistics(true);
            legion.SetShouldUseFenceNoBossAttack(false, false);
            legion.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            SAINSettings legionSettings = new SAINSettings(legion.WildSpawnTypeValue)
            {
                Name = "Legion",
                Description = "Leader of the Legion.",
                Section = "Legion",
                BaseBrain = "PMC",
                BrainsToApply = brains,
                LayersToRemove = layers,
            };

            legion.SetSAINSettings(legionSettings);
            CustomWildSpawnTypeManager.RegisterWildSpawnType(legion, assembly);

            var legionnaire = new CustomWildSpawnType(200, "legionnaire", "Legion", baseBrainInt, true, true, false);
            legionnaire.SetCountAsBossForStatistics(true);
            legionnaire.SetShouldUseFenceNoBossAttack(false, false);
            legionnaire.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            SAINSettings legionnaireSettings = new SAINSettings(legionnaire.WildSpawnTypeValue)
            {
                Name = "Legionnaire",
                Description = "A combat member of the Legion. Fights tooth and nail to protect their leader.",
                Section = "Legion",
                BaseBrain = "PMC",
                BrainsToApply = brains,
                LayersToRemove = layers,
            };

            legionnaire.SetSAINSettings(legionnaireSettings);
            CustomWildSpawnTypeManager.RegisterWildSpawnType(legionnaire, assembly);

            CustomWildSpawnTypeManager.AddSuitableGroup(new List<int> { 199, 200 });

            var wolfLead = new CustomWildSpawnType(201, "wolflead", "Wolves", baseBrainInt, true, true, false);
            wolfLead.SetCountAsBossForStatistics(false);
            wolfLead.SetShouldUseFenceNoBossAttack(false, false);
            wolfLead.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            SAINSettings wolfLeadSettings = new SAINSettings(wolfLead.WildSpawnTypeValue)
            {
                Name = "Wolf Squad Leader",
                Description = "The senior protection operator. Leads the team in protecting the player.",
                Section = "Wolves",
                BaseBrain = "PMC",
                BrainsToApply = brains,
                LayersToRemove = layers,
            };
            wolfLead.SetSAINSettings(wolfLeadSettings);
            CustomWildSpawnTypeManager.RegisterWildSpawnType(wolfLead, assembly);

            var wolfSupport = new CustomWildSpawnType(202, "wolfsupport", "Wolves", baseBrainInt, true, true, false);
            wolfSupport.SetCountAsBossForStatistics(false);
            wolfSupport.SetShouldUseFenceNoBossAttack(false, false);
            wolfSupport.SetExcludedDifficulties(new List<int> { 0, 2, 3 });

            SAINSettings wolfSupportSettings = new SAINSettings(wolfSupport.WildSpawnTypeValue)
            {
                Name = "Wolf Squad Support",
                Description = "A hired protection operator. Defends the player at all costs.",
                Section = "Wolves",
                BaseBrain = "PMC",
                BrainsToApply = brains,
                LayersToRemove = layers,
            };
            wolfSupport.SetSAINSettings(wolfSupportSettings);
            CustomWildSpawnTypeManager.RegisterWildSpawnType(wolfSupport, assembly);

            CustomWildSpawnTypeManager.AddSuitableGroup(new List<int> { 201, 202 });
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
