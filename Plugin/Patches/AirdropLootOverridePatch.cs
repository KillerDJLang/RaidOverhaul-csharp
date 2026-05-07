using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFT.SynchronizableObjects;
using HarmonyLib;
using RaidOverhaul.Controllers;
using RaidOverhaul.Helpers;
using SPT.Reflection.Patching;

namespace RaidOverhaul.Patches
{
    public class AirdropLootOverridePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LootItem), nameof(LootItem.CreateLootContainer));
        }

        [PatchPostfix]
        private static void Postfix(LootableContainer lc, Item item)
        {
            if (lc.GetComponentInParent<AirdropSynchronizableObject>() == null)
            {
                return;
            }

            if (EventController.PendingCorpseItems == null || EventController.PendingCorpseItems.Count == 0)
            {
                Utils.LogToServerConsole("Corpse Cleanup: PendingCorpseItems is null or Count == 0.");
                return;
            }

            if (item is not CompoundItem root || root.Grids == null)
            {
                Utils.LogToServerConsole("Corpse Cleanup: airdrop item has no grids.");
                return;
            }

            if (EventController.AirdropCrateCapacity < 0)
            {
                int measured = 0;
                foreach (var g in root.Grids)
                {
                    if (!g.CanStretchVertically)
                    {
                        measured += g.GridWidth * g.GridHeight;
                    }
                }

                if (measured > 0)
                {
                    EventController.AirdropCrateCapacity = measured;
                }
            }

            foreach (var grid in root.Grids)
            {
                foreach (var gridItem in grid.Items.ToList())
                {
                    grid.RemoveWithoutRestrictions(gridItem);
                }
            }

            var remaining = new List<Item>();
            foreach (var pending in EventController.PendingCorpseItems)
            {
                bool placed = false;
                foreach (var grid in root.Grids)
                {
                    var result = grid.AddAnywhere(pending, EErrorHandlingType.Ignore);
                    if (!result.Failed)
                    {
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    remaining.Add(pending);
                }
            }

            Utils.LogToServerConsole("Corpse Cleanup: AirdropLootOverridePatch endpoint hit.");
            EventController.PendingCorpseItems = remaining;
        }
    }
}
