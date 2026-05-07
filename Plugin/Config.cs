using System;
using BepInEx.Configuration;
using UnityEngine;

namespace RaidOverhaul
{
    public static class ROPluginConfig
    {
        [Flags]
        public enum RaidEvents
        {
            Damage = 1,
            Blackout = 2,
            ArmorRepair = 4,
            Heal = 8,
            Airdrop = 16,
            Skill = 32,
            Metabolism = 64,
            Malfunction = 128,
            Trader = 256,
            Berserk = 512,
            Weight = 1024,
            NoJokesHere = 2048,
            ShoppingSpree = 4096,
            ExfilLockdown = 8192,
            Artillery = 16384,
            Invasion = 32768,
            Hunted = 65536,

            All =
                Damage
                | Blackout
                | ArmorRepair
                | Heal
                | Airdrop
                | Skill
                | Metabolism
                | Malfunction
                | Trader
                | Berserk
                | Weight
                | NoJokesHere
                | ShoppingSpree
                | ExfilLockdown
                | Artillery
                | Invasion
                | Hunted,
        }

        [Flags]
        public enum DoorEvents
        {
            PowerOn = 1,
            DoorUnlock = 2,
            KDoorUnlock = 4,

            All = PowerOn | DoorUnlock | KDoorUnlock,
        }

        public static ConfigEntry<bool> DropBackPack;
        public static ConfigEntry<bool> EnableClean;
        public static ConfigEntry<int> TimeToClean;
        public static ConfigEntry<float> DropBackPackChance;

        public static ConfigEntry<bool> Deafness;
        public static ConfigEntry<bool> Concussion;

        public static ConfigEntry<bool> EnableEvents;
        public static ConfigEntry<bool> EnableDoorEvents;
        public static ConfigEntry<bool> EnableRaidStartEvents;

        public static ConfigEntry<RaidEvents> RandomEventsToEnable;

        public static ConfigEntry<DoorEvents> DoorEventsToEnable;

        public static ConfigEntry<bool> SpecialReqFeatures;

        public static ConfigEntry<KeyboardShortcut> SpecialReqFeaturesBinding;

        public static void BindConfig(ConfigFile cfg)
        {
            #region Core Events

            SpecialReqFeaturesBinding = cfg.Bind(
                "1. Core Events  (Changing Any Of These Options Requires Restart)",
                "Exfil and Train Call Menu Keybind",
                new KeyboardShortcut(KeyCode.F7),
                new ConfigDescription(
                    "Set the keybind for opening the Exfil and Train Call Menu.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 5,
                    }
                )
            );

            SpecialReqFeatures = cfg.Bind(
                "1. Core Events  (Changing Any Of These Options Requires Restart)",
                "Exfil and Train Call Menu",
                true,
                new ConfigDescription(
                    "Enable the Exfil and Train Call Menu.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 4,
                    }
                )
            );

            EnableRaidStartEvents = cfg.Bind(
                "1. Core Events  (Changing Any Of These Options Requires Restart)",
                "Raid Start Events",
                true,
                new ConfigDescription(
                    "Dictates whether you allow the Door and Light randomization events to run on raid start or not.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 3,
                    }
                )
            );

            EnableEvents = cfg.Bind(
                "1. Core Events  (Changing Any Of These Options Requires Restart)",
                "Dynamic Events",
                true,
                new ConfigDescription(
                    "Dictates whether the dynamic event timer should increment and allow events to run or not.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 2,
                    }
                )
            );

            EnableDoorEvents = cfg.Bind(
                "1. Core Events  (Changing Any Of These Options Requires Restart)",
                "Dynamic Door Events",
                true,
                new ConfigDescription(
                    "Dictates whether the dynamic event timer should increment and allow door events to run or not.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 1,
                    }
                )
            );

            #endregion

            #region Random Events

            RandomEventsToEnable = cfg.Bind(
                "2. Random Events",
                "Events List",
                RaidEvents.All,
                new ConfigDescription(
                    "Disable/Enable any of the random events that occur throughout your raids.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 1,
                    }
                )
            );

            #endregion

            #region Door Events

            DoorEventsToEnable = cfg.Bind(
                "3. Door Events",
                "Door Events List",
                DoorEvents.All,
                new ConfigDescription(
                    "Disable/Enable any of the door/power switch events that occur throughout your raids.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 1,
                    }
                )
            );

            #endregion

            #region Body Clean Up

            EnableClean = cfg.Bind(
                "4. Body Cleanup Configs",
                "Enable Clean",
                true,
                new ConfigDescription(
                    "Enable body cleanup event.\nThis requires a restart to take effect after enabling or disabling!",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 2,
                    }
                )
            );

            TimeToClean = cfg.Bind(
                "4. Body Cleanup Configs",
                "Time to Clean",
                60,
                new ConfigDescription(
                    "The time to clean bodies calculated in minutes.",
                    new AcceptableValueRange<int>(30, 90),
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 1,
                    }
                )
            );

            #endregion

            #region Bot Drop

            DropBackPack = cfg.Bind(
                "5. Bot Drop Configs",
                "Drop Backpack",
                true,
                new ConfigDescription(
                    "Enable bots dropping backpacks on death.\nThis requires a restart to take effect after enabling or disabling!",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = true,
                        Order = 2,
                    }
                )
            );

            DropBackPackChance = cfg.Bind(
                "5. Bot Drop Configs",
                "Backpack Drop Chance",
                0.2f,
                new ConfigDescription(
                    "Chance of bots dropping a backpack on death.",
                    new AcceptableValueRange<float>(0f, 1f),
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = true,
                        Order = 1,
                    }
                )
            );

            #endregion

            #region Shot Effects

            Deafness = cfg.Bind(
                "6. Shot Effects",
                "Deafness",
                false,
                new ConfigDescription(
                    "Enable deafness changes. Make sure you have your ear protection on.\nThis requires a restart to take effect after enabling or disabling!",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 2,
                    }
                )
            );

            Concussion = cfg.Bind(
                "6. Shot Effects",
                "Concussion",
                false,
                new ConfigDescription(
                    "Enable concussion effect. Getting shot in the head while wearing protection will result in a concussion.\nThis requires a restart to take effect after enabling or disabling!",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        IsAdvanced = false,
                        ShowRangeAsPercent = false,
                        Order = 1,
                    }
                )
            );

            #endregion
        }
    }
}
