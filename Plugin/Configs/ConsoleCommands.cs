using System;
using System.Linq;
using EFT.InventoryLogic;
using EFT.UI;
using RaidOverhaul.Helpers;

namespace RaidOverhaul.Configs
{
    internal static class ConsoleCommands
    {
        public static void RegisterCC()
        {
            ConsoleScreen.Processor.RegisterCommand("ROHeal", new Action(Plugin._ecScript.DoHealPlayer));
            ConsoleScreen.Processor.RegisterCommand("RODamage", new Action(Plugin._ecScript.DoDamageEvent));
            ConsoleScreen.Processor.RegisterCommand("ROArmor", new Action(Plugin._ecScript.DoArmorRepair));
            ConsoleScreen.Processor.RegisterCommand("ROAirdrop", new Action(Plugin._ecScript.DoAirdropEvent));
            ConsoleScreen.Processor.RegisterCommand("ROFunny", new Action(Plugin._ecScript.DoFunnyWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROBlackout", new Action(Plugin._ecScript.DoBlackoutEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROSkill", new Action(Plugin._ecScript.DoSkillEvent));
            ConsoleScreen.Processor.RegisterCommand("ROMetabolism", new Action(Plugin._ecScript.DoMetabolismEvent));
            ConsoleScreen.Processor.RegisterCommand("ROMalf", new Action(Plugin._ecScript.DoMalfEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROLL", new Action(Plugin._ecScript.DoLLEvent));
            ConsoleScreen.Processor.RegisterCommand("ROBerserk", new Action(Plugin._ecScript.DoBerserkEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROWeight", new Action(Plugin._ecScript.DoWeightEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROMaxLL", new Action(Plugin._ecScript.DoMaxLLEvent));
            ConsoleScreen.Processor.RegisterCommand("RORepCorrect", new Action(Plugin._ecScript.CorrectRep));
            ConsoleScreen.Processor.RegisterCommand("ROLockdown", new Action(Plugin._ecScript.DoLockDownEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROArtillery", new Action(Plugin._ecScript.DoArtyEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("RORunTrain", new Action(Plugin._ecScript.RunTrainWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROPmcExfil", new Action(Plugin._ecScript.DoPmcExfilEventWrapper));
            ConsoleScreen.Processor.RegisterCommand("ROExfilNow", new Action(Plugin._ecScript.ExfilNow));
            ConsoleScreen.Processor.RegisterCommand("ROInvasion", new Action(Plugin._ecScript.StartInvasion));
            ConsoleScreen.Processor.RegisterCommand("GetWeaponIds", new Action(GetAllWeaponIDs));
            ConsoleScreen.Processor.RegisterCommand("GetAllIds", new Action(GetAllItemIDs));
        }

        private static void GetAllWeaponIDs()
        {
            var weapons = Plugin._session.Profile.Inventory?.AllRealPlayerItems;
            weapons = weapons.Where(x => x is Weapon);

            foreach (var weapon in weapons)
            {
                Plugin._log.LogInfo($"Template ID: {weapon.TemplateId}, locale name: {weapon.LocalizedName()}");
                Utils.LogToServerConsole($"Template ID: {weapon.TemplateId}, locale name: {weapon.LocalizedName()}");
            }
        }

        private static void GetAllItemIDs()
        {
            var items = Plugin._session.Profile.Inventory?.AllRealPlayerItems;
            items = items.Where(x => x is Item);

            foreach (var item in items)
            {
                Plugin._log.LogInfo($"Template ID: {item.TemplateId}, locale name: {item.LocalizedName()}");
                Utils.LogToServerConsole($"Template ID: {item.TemplateId}, locale name: {item.LocalizedName()}");
            }
        }
    }
}
