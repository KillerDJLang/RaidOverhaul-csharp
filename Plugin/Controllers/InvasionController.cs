using System;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;

namespace RaidOverhaul.Controllers
{
    internal static class InvasionController
    {
        internal static void StartInvasion()
        {
            var spawner = Singleton<IBotGame>.Instance?.BotsController?.BotSpawner;
            if (spawner == null)
            {
                return;
            }

            var bossZones = spawner.SpawnZones(false).Where(z => z.CanSpawnBoss).ToList();
            if (bossZones.Count == 0)
            {
                return;
            }

            var random = new System.Random();
            var zone = bossZones[random.Next(bossZones.Count)];

            var bossConfig = ConfigController.ServerConfig.EnableCustomBoss
                ? _bossDataPool[random.Next(_bossDataPool.Count)]
                : _bossDataPoolNoLegion[random.Next(_bossDataPoolNoLegion.Count)];

            var wave = new BossLocationSpawn
            {
                BossName = bossConfig.BossName,
                BossType = bossConfig.BossType,
                BossChance = 100f,
                BossPlayer = false,
                BossDifficult = "normal",
                BossDif = BotDifficulty.normal,
                BossZone = zone.NameZone,
                BornZone = zone.NameZone,
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

        private static readonly List<BossInvasionConfig> _bossDataPool = new List<BossInvasionConfig>
        {
            new BossInvasionConfig
            {
                BossName = "bosslegion",
                BossEscorts = "legionnaire",
                BossType = (WildSpawnType)199,
                BossEscortType = (WildSpawnType)200,
                BossEscortCount = 2,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "legionnaire",
                BossEscorts = "legionnaire",
                BossType = (WildSpawnType)200,
                BossEscortType = (WildSpawnType)200,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossTagilla",
                BossEscorts = "followerBully",
                BossType = WildSpawnType.bossTagilla,
                BossEscortType = WildSpawnType.followerBully,
                BossEscortCount = 0,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKilla",
                BossEscorts = "followerTagilla",
                BossType = WildSpawnType.bossKilla,
                BossEscortType = WildSpawnType.followerTagilla,
                BossEscortCount = 0,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossGluhar",
                BossEscorts = "followerGluharSecurity",
                BossType = WildSpawnType.bossGluhar,
                BossEscortType = WildSpawnType.followerGluharSecurity,
                BossEscortCount = 2,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharAssault,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharSecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharScout,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossKnight",
                BossEscorts = "exUsec",
                BossType = WildSpawnType.bossKnight,
                BossEscortType = WildSpawnType.exUsec,
                BossEscortCount = 2,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBigPipe,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBirdEye,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossZryachiy",
                BossEscorts = "followerZryachiy",
                BossType = WildSpawnType.bossZryachiy,
                BossEscortType = WildSpawnType.followerZryachiy,
                BossEscortCount = 2,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossSanitar",
                BossEscorts = "followerSanitar",
                BossType = WildSpawnType.bossSanitar,
                BossEscortType = WildSpawnType.followerSanitar,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKolontay",
                BossEscorts = "followerKolontaySecurity",
                BossType = WildSpawnType.bossKolontay,
                BossEscortType = WildSpawnType.followerKolontaySecurity,
                BossEscortCount = 2,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontayAssault,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontaySecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossBoar",
                BossEscorts = "followerBoar",
                BossType = WildSpawnType.bossBoar,
                BossEscortType = WildSpawnType.followerBoar,
                BossEscortCount = 6,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoar,
                        BossEscortAmount = 4,
                        BossEscortDifficult = new[] { "normal", "normal", "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoarClose1,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoarClose2,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossBully",
                BossEscorts = "followerBully",
                BossType = WildSpawnType.bossBully,
                BossEscortType = WildSpawnType.followerBully,
                BossEscortCount = 4,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKojaniy",
                BossEscorts = "followerKojaniy",
                BossType = WildSpawnType.bossKojaniy,
                BossEscortType = WildSpawnType.followerKojaniy,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "exUsec",
                BossEscorts = "exUsec",
                BossType = WildSpawnType.exUsec,
                BossEscortType = WildSpawnType.exUsec,
                BossEscortCount = 4,
                AdditionalSupports = null,
            },
        };

        private static readonly List<BossInvasionConfig> _bossDataPoolNoLegion = new List<BossInvasionConfig>
        {
            new BossInvasionConfig
            {
                BossName = "bossTagilla",
                BossEscorts = "followerBully",
                BossType = WildSpawnType.bossTagilla,
                BossEscortType = WildSpawnType.followerBully,
                BossEscortCount = 0,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKilla",
                BossEscorts = "followerTagilla",
                BossType = WildSpawnType.bossKilla,
                BossEscortType = WildSpawnType.followerTagilla,
                BossEscortCount = 0,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossGluhar",
                BossEscorts = "followerGluharSecurity",
                BossType = WildSpawnType.bossGluhar,
                BossEscortType = WildSpawnType.followerGluharSecurity,
                BossEscortCount = 2,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharAssault,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharSecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharScout,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossKnight",
                BossEscorts = "exUsec",
                BossType = WildSpawnType.bossKnight,
                BossEscortType = WildSpawnType.exUsec,
                BossEscortCount = 2,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBigPipe,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBirdEye,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossZryachiy",
                BossEscorts = "followerZryachiy",
                BossType = WildSpawnType.bossZryachiy,
                BossEscortType = WildSpawnType.followerZryachiy,
                BossEscortCount = 2,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossSanitar",
                BossEscorts = "followerSanitar",
                BossType = WildSpawnType.bossSanitar,
                BossEscortType = WildSpawnType.followerSanitar,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKolontay",
                BossEscorts = "followerKolontaySecurity",
                BossType = WildSpawnType.bossKolontay,
                BossEscortType = WildSpawnType.followerKolontaySecurity,
                BossEscortCount = 2,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontayAssault,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontaySecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal", "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossBoar",
                BossEscorts = "followerBoar",
                BossType = WildSpawnType.bossBoar,
                BossEscortType = WildSpawnType.followerBoar,
                BossEscortCount = 6,
                AdditionalSupports =
                [
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoar,
                        BossEscortAmount = 4,
                        BossEscortDifficult = new[] { "normal", "normal", "normal", "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoarClose1,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerBoarClose2,
                        BossEscortAmount = 1,
                        BossEscortDifficult = new[] { "normal" },
                    },
                ],
            },
            new BossInvasionConfig
            {
                BossName = "bossBully",
                BossEscorts = "followerBully",
                BossType = WildSpawnType.bossBully,
                BossEscortType = WildSpawnType.followerBully,
                BossEscortCount = 4,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "bossKojaniy",
                BossEscorts = "followerKojaniy",
                BossType = WildSpawnType.bossKojaniy,
                BossEscortType = WildSpawnType.followerKojaniy,
                BossEscortCount = 3,
                AdditionalSupports = null,
            },
            new BossInvasionConfig
            {
                BossName = "exUsec",
                BossEscorts = "exUsec",
                BossType = WildSpawnType.exUsec,
                BossEscortType = WildSpawnType.exUsec,
                BossEscortCount = 4,
                AdditionalSupports = null,
            },
        };
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
