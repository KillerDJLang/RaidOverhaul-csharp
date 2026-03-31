using System;
using System.Collections.Generic;
using System.Linq;
using RaidOverhaul.Configs;
using RaidOverhaul.Controllers;

namespace RaidOverhaul.Helpers
{
    public static class Weighting
    {
        public static List<(Action, int)> WeightedEvents;
        public static List<(Action, int)> WeightedDoorMethods;

        public static void InitWeightings()
        {
            InitDoorWeighting();
            InitEventWeighting();
        }

        public static void DoRandomEvent(List<(Action, int)> weighting)
        {
            weighting = weighting.OrderBy(_ => Guid.NewGuid()).ToList();
            var totalWeight = weighting.Sum(pair => pair.Item2);
            var randomNum = new Random().Next(1, totalWeight + 1);

            foreach (var (method, weight) in weighting)
            {
                randomNum -= weight;
                if (randomNum <= 0)
                {
                    method();
                    break;
                }
            }
        }

        private static void InitDoorWeighting()
        {
            var switchWeighting = DJConfig.DoorEventsToEnable.Value.HasFlag(DJConfig.DoorEvents.PowerOn)
                ? ConfigController.EventConfig.SwitchWeights
                : 0;
            var doorWeighting = DJConfig.DoorEventsToEnable.Value.HasFlag(DJConfig.DoorEvents.DoorUnlock)
                ? ConfigController.EventConfig.LockedDoorWeights
                : 0;
            var keycardWeighting = DJConfig.DoorEventsToEnable.Value.HasFlag(DJConfig.DoorEvents.KDoorUnlock)
                ? ConfigController.EventConfig.KeycardWeights
                : 0;

            WeightedDoorMethods = new List<(Action, int)>
            {
                (Plugin._dcScript.PowerOn, switchWeighting),
                (Plugin._dcScript.DoUnlock, doorWeighting),
                (Plugin._dcScript.DoKUnlock, keycardWeighting),
            };
        }

        private static void InitEventWeighting()
        {
            var damageWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Damage)
                ? ConfigController.EventConfig.DamageEventWeights
                : 0;
            var airdropWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Airdrop)
                ? ConfigController.EventConfig.AirdropEventWeights
                : 0;
            var blackoutWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Blackout)
                ? ConfigController.EventConfig.BlackoutEventWeights
                : 0;
            var jokeWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.NoJokesHere)
                ? ConfigController.EventConfig.JokeEventWeights
                : 0;
            var healWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Heal)
                ? ConfigController.EventConfig.HealEventWeights
                : 0;
            var armorWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.ArmorRepair)
                ? ConfigController.EventConfig.ArmorEventWeights
                : 0;
            var skillWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Skill)
                ? ConfigController.EventConfig.SkillEventWeights
                : 0;
            var metWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Metabolism)
                ? ConfigController.EventConfig.MetabolismEventWeights
                : 0;
            var malfWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Malfunction)
                ? ConfigController.EventConfig.MalfEventWeights
                : 0;
            var traderWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Trader)
                ? ConfigController.EventConfig.TraderEventWeights
                : 0;
            var berserkWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Berserk)
                ? ConfigController.EventConfig.BerserkEventWeights
                : 0;
            var weightWeightingLOL = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Weight)
                ? ConfigController.EventConfig.WeightEventWeights
                : 0;
            var maxLLWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.ShoppingSpree)
                ? ConfigController.EventConfig.MaxLLEventWeights
                : 0;
            var exfilWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.ExfilLockdown)
                ? ConfigController.EventConfig.ExfilEventWeights
                : 0;
            var artyWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Artillery)
                ? ConfigController.EventConfig.ArtilleryEventWeights
                : 0;
            var invasionWeighting = DJConfig.RandomEventsToEnable.Value.HasFlag(DJConfig.RaidEvents.Invasion)
                ? ConfigController.EventConfig.InvasionEventWeights
                : 0;

            WeightedEvents = new List<(Action, int)>
            {
                (Plugin._ecScript.DoDamageEvent, damageWeighting),
                (Plugin._ecScript.DoAirdropEvent, airdropWeighting),
                (Plugin._ecScript.DoBlackoutEventWrapper, blackoutWeighting),
                (Plugin._ecScript.DoFunnyWrapper, jokeWeighting),
                (Plugin._ecScript.DoHealPlayer, healWeighting),
                (Plugin._ecScript.DoArmorRepair, armorWeighting),
                (Plugin._ecScript.DoSkillEvent, skillWeighting),
                (Plugin._ecScript.DoMetabolismEvent, metWeighting),
                (Plugin._ecScript.DoMalfEventWrapper, malfWeighting),
                (Plugin._ecScript.DoLLEvent, traderWeighting),
                (Plugin._ecScript.DoBerserkEventWrapper, berserkWeighting),
                (Plugin._ecScript.DoWeightEventWrapper, weightWeightingLOL),
                (Plugin._ecScript.DoMaxLLEvent, maxLLWeighting),
                (Plugin._ecScript.DoLockDownEventWrapper, exfilWeighting),
                (Plugin._ecScript.DoArtyEventWrapper, artyWeighting),
                (Plugin._ecScript.StartInvasion, invasionWeighting),
            };
        }
    }
}
