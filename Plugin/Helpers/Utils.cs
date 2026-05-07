using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using Newtonsoft.Json;
using SPT.Common.Http;

namespace RaidOverhaul.Helpers
{
    public static class Utils
    {
        public static readonly Dictionary<string, MongoID> Traders = new()
        {
            { "Prapor", "54cb50c76803fa8b248b4571" },
            { "Therapist", "54cb57776803fa99248b456e" },
            { "Fence", "579dc571d53a0658a154fbec" },
            { "Skier", "58330581ace78e27b8b10cee" },
            { "Peacekeeper", "5935c25fb3acc3127c3d8cd9" },
            { "Mechanic", "5a7c2eca46aef81a7ca2145d" },
            { "Ragman", "5ac3b934156ae10c4430e83c" },
            { "Jaeger", "5c0647fdd443bc2504c2d371" },
            { "Lightkeeper", "638f541a29ffd1183d187f57" },
            { "Btr", "656f0f98d80a697f855d34b1" },
            { "Ref", "6617beeaa9cfa777ca915b7c" },
            { "ReqShop", "66f0eaa93f6cc015bc1f3acb" },
        };

        public static readonly Dictionary<string, MongoID> TradersNoReq = new()
        {
            { "Prapor", "54cb50c76803fa8b248b4571" },
            { "Therapist", "54cb57776803fa99248b456e" },
            { "Fence", "579dc571d53a0658a154fbec" },
            { "Skier", "58330581ace78e27b8b10cee" },
            { "Peacekeeper", "5935c25fb3acc3127c3d8cd9" },
            { "Mechanic", "5a7c2eca46aef81a7ca2145d" },
            { "Ragman", "5ac3b934156ae10c4430e83c" },
            { "Jaeger", "5c0647fdd443bc2504c2d371" },
            { "Lightkeeper", "638f541a29ffd1183d187f57" },
            { "Btr", "656f0f98d80a697f855d34b1" },
            { "Ref", "6617beeaa9cfa777ca915b7c" },
        };

        public static readonly Dictionary<string, MongoID> Currency = new()
        {
            { "Roubles", "5449016a4bdc2d6f028b456f" },
            { "USD", "5696686a4bdc2da3298b456a" },
            { "Euros", "569668774bdc2da2298b4568" },
            { "GPCoins", "5d235b4d86f7742e017bc88a" },
            { "ReqCoins", "66292e79a4d9da25e683ab55" },
            { "ReqSlips", "668b3c71042c73c6f9b00704" },
            { "SpecialReqForms", "67c95a09708ee99e7a575da5" },
        };

        public const string SkeletonKey = "66a2fc926af26cc365283f23";
        public const string VipKeycard = "66a2fc9886fbd5d38c5ca2a6";
        public const string RealismKey = "RealismMod";
        public const string ROStandaloneKey = "nameless.raidoverhaul.standalone";
        public const string UnityToolkitKey = "com.arys.unitytoolkit";
        public const string FikaCoreKey = "com.fika.core";
        public const string BigBrainKey = "xyz.drakia.bigbrain";
        public const string SAINKey = "me.sol.sain";
        public const string Heal = "Heal";
        public const string Damage = "Damage";
        public const string Repair = "Repair";
        public const string Airdrop = "Airdrop";
        public const string Jokes = "Jokes";
        public const string Blackout = "Blackout";
        public const string Skill = "Skill";
        public const string Metabolism = "Metabolism";
        public const string Malf = "Malf";
        public const string LoyaltyLevel = "LoyaltyLevel";
        public const string Berserk = "Berserk";
        public const string Weight = "Weight";
        public const string MaxLoyaltyLevel = "MaxLoyaltyLevel";
        public const string CorrectRep = "CorrectRep";
        public const string Lockdown = "Lockdown";
        public const string GearExfilEvent = "GearExfilEvent";
        public const string Train = "Train";
        public const string PmcExfil = "PmcExfil";
        public const string Artillery = "Artillery";
        public const string Hunted = "Hunted";
        public const string ExfilNow = "ExfilNow";

        public static T Get<T>(string url)
        {
            var req = RequestHandler.GetJson(url);

            if (string.IsNullOrEmpty(req))
            {
                throw new InvalidOperationException("The response from the server is null or empty.");
            }

            return JsonConvert.DeserializeObject<T>(req);
        }

        public static void LogToServerConsole(string message)
        {
            Plugin._log.Log(LogLevel.Info, message);
            RequestHandler.PutJson("/RaidOverhaul/LogToServer", new { message = message }.ToJson(null));
        }

        public static bool IsInRaid()
        {
            if (!Singleton<GameWorld>.Instantiated)
            {
                return false;
            }

            var gameWorld = Singleton<GameWorld>.Instance;

            return gameWorld != null
                && gameWorld.AllAlivePlayersList != null
                && gameWorld.AllAlivePlayersList.Count > 0
                && gameWorld.MainPlayer != null
                && gameWorld.MainPlayer is not HideoutPlayer;
        }

        internal static void SpawnBoss(BossInvasionConfig bossConfig, string zoneName = null)
        {
            var spawner = Singleton<IBotGame>.Instance?.BotsController?.BotSpawner;
            if (spawner == null)
            {
                return;
            }

            string spawnZone;
            if (zoneName != null)
            {
                spawnZone = zoneName;
            }
            else
            {
                var bossZones = spawner.SpawnZones(false).Where(z => z.CanSpawnBoss).ToList();
                if (bossZones.Count == 0)
                {
                    return;
                }
                spawnZone = bossZones[new Random().Next(bossZones.Count)].NameZone;
            }

            var wave = new BossLocationSpawn
            {
                BossName = bossConfig.BossName,
                BossType = bossConfig.BossType,
                BossChance = 100f,
                BossPlayer = false,
                BossDifficult = "normal",
                BossDif = BotDifficulty.normal,
                BossZone = spawnZone,
                BornZone = spawnZone,
                BossEscortType = bossConfig.BossEscorts,
                EscortType = bossConfig.BossEscortType,
                BossEscortAmount = bossConfig.BossEscortCount.ToString(),
                EscortCount = bossConfig.BossEscortCount,
                BossEscortDifficult = "normal",
                EscortDif = BotDifficulty.normal,
                Supports = bossConfig.AdditionalSupports,
                ForceSpawn = true,
                IgnoreMaxBots = true,
                ShallSpawn = true,
                Time = -1f,
                TriggerType = SpawnTriggerType.none,
                TriggerId = "",
                TriggerName = "",
            };

            if (bossConfig.AdditionalSupports != null && bossConfig.AdditionalSupports.Length > 0)
            {
                wave.SubDatas = new List<BossLocationSpawnSubData>();
                int totalEscorts = bossConfig.BossEscortCount;

                foreach (var support in bossConfig.AdditionalSupports)
                {
                    var difficulty = (BotDifficulty)Enum.Parse(typeof(BotDifficulty), support.BossEscortDifficult[0]);
                    var subData = new BossLocationSpawnSubData(support.BossEscortAmount, support.BossEscortType, difficulty);
                    wave.SubDatas.Add(subData);
                    totalEscorts += subData.BossEscortAmount;
                }

                wave.EscortCount = totalEscorts;
            }

            spawner.ActivateBotsByWave(wave);
        }

        public static QuestClass GetQuest(AbstractQuestControllerClass questController, string questId)
        {
            object quests = Plugin._abstractQuestControllerQuestsProp.GetValue(questController);
            return Plugin._abstractQuestControllerGetMethod.Invoke(quests, new object[] { questId }) as QuestClass;
        }
    }

    internal class BossInvasionConfig
    {
        public string BossName;
        public string BossEscorts;
        public WildSpawnType BossType;
        public WildSpawnType BossEscortType;
        public int BossEscortCount;
        public WildSpawnSupports[] AdditionalSupports;
    }
}
