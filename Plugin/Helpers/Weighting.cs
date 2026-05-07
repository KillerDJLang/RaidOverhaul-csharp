using System;
using System.Collections.Generic;
using System.Linq;
using RaidOverhaul.Controllers;

namespace RaidOverhaul.Helpers
{
    public static class Weighting
    {
        public static List<(Action, int)> WeightedEvents;
        public static List<(Action, int)> WeightedDoorMethods;

        internal static void InitWeightings(EventController eventController, DoorController doorController)
        {
            InitDoorWeighting(doorController);
            InitEventWeighting(eventController);
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

        private static void InitDoorWeighting(DoorController doorController)
        {
            var switchWeighting = ROPluginConfig.DoorEventsToEnable.Value.HasFlag(ROPluginConfig.DoorEvents.PowerOn)
                ? ConfigController.EventConfig.SwitchWeights
                : 0;
            var doorWeighting = ROPluginConfig.DoorEventsToEnable.Value.HasFlag(ROPluginConfig.DoorEvents.DoorUnlock)
                ? ConfigController.EventConfig.LockedDoorWeights
                : 0;
            var keycardWeighting = ROPluginConfig.DoorEventsToEnable.Value.HasFlag(ROPluginConfig.DoorEvents.KDoorUnlock)
                ? ConfigController.EventConfig.KeycardWeights
                : 0;

            WeightedDoorMethods = new List<(Action, int)>
            {
                (doorController.PowerOn, switchWeighting),
                (doorController.DoUnlock, doorWeighting),
                (doorController.DoKUnlock, keycardWeighting),
            };
        }

        private static void InitEventWeighting(EventController eventController)
        {
            var damageWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Damage)
                ? ConfigController.EventConfig.DamageEventWeights
                : 0;
            var airdropWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Airdrop)
                ? ConfigController.EventConfig.AirdropEventWeights
                : 0;
            var blackoutWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Blackout)
                ? ConfigController.EventConfig.BlackoutEventWeights
                : 0;
            var jokeWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.NoJokesHere)
                ? ConfigController.EventConfig.JokeEventWeights
                : 0;
            var healWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Heal)
                ? ConfigController.EventConfig.HealEventWeights
                : 0;
            var armorWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.ArmorRepair)
                ? ConfigController.EventConfig.ArmorEventWeights
                : 0;
            var skillWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Skill)
                ? ConfigController.EventConfig.SkillEventWeights
                : 0;
            var metWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Metabolism)
                ? ConfigController.EventConfig.MetabolismEventWeights
                : 0;
            var malfWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Malfunction)
                ? ConfigController.EventConfig.MalfEventWeights
                : 0;
            var traderWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Trader)
                ? ConfigController.EventConfig.TraderEventWeights
                : 0;
            var berserkWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Berserk)
                ? ConfigController.EventConfig.BerserkEventWeights
                : 0;
            var weightWeightingLOL = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Weight)
                ? ConfigController.EventConfig.WeightEventWeights
                : 0;
            var maxLLWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.ShoppingSpree)
                ? ConfigController.EventConfig.MaxLLEventWeights
                : 0;
            var exfilWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.ExfilLockdown)
                ? ConfigController.EventConfig.ExfilEventWeights
                : 0;
            var artyWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Artillery)
                ? ConfigController.EventConfig.ArtilleryEventWeights
                : 0;
            var invasionWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Invasion)
                ? ConfigController.EventConfig.InvasionEventWeights
                : 0;
            var huntedWeighting = ROPluginConfig.RandomEventsToEnable.Value.HasFlag(ROPluginConfig.RaidEvents.Hunted)
                ? ConfigController.EventConfig.HuntedEventWeights
                : 0;

            WeightedEvents = new List<(Action, int)>
            {
                (eventController.DoDamageEvent, damageWeighting),
                (eventController.DoAirdropEvent, airdropWeighting),
                (eventController.DoBlackoutEventWrapper, blackoutWeighting),
                (eventController.DoFunnyWrapper, jokeWeighting),
                (eventController.DoHealPlayer, healWeighting),
                (eventController.DoArmorRepair, armorWeighting),
                (eventController.DoSkillEvent, skillWeighting),
                (eventController.DoMetabolismEvent, metWeighting),
                (eventController.DoMalfEventWrapper, malfWeighting),
                (eventController.DoLLEvent, traderWeighting),
                (eventController.DoBerserkEventWrapper, berserkWeighting),
                (eventController.DoWeightEventWrapper, weightWeightingLOL),
                (eventController.DoMaxLLEvent, maxLLWeighting),
                (eventController.DoLockDownEventWrapper, exfilWeighting),
                (eventController.DoArtyEventWrapper, artyWeighting),
                (eventController.StartInvasion, invasionWeighting),
                (eventController.DoHuntedEventWrapper, huntedWeighting),
            };
        }
    }
}
