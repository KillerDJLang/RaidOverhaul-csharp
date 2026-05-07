using System;
using System.Collections.Generic;
using EFT;
using RaidOverhaul.Helpers;

namespace RaidOverhaul.Controllers
{
    internal static class InvasionController
    {
        internal static void StartInvasion()
        {
            var random = new Random();
            var bossConfig = ConfigController.ServerConfig.EnableCustomBoss
                ? _bossDataPool[random.Next(_bossDataPool.Count)]
                : _bossDataPoolNoLegion[random.Next(_bossDataPoolNoLegion.Count)];

            Utils.SpawnBoss(bossConfig);
        }

        private static readonly List<BossInvasionConfig> _bossDataPool = new List<BossInvasionConfig>
        {
            new BossInvasionConfig
            {
                BossName = "bosslegion",
                BossEscorts = "legionnaire",
                BossType = (WildSpawnType)199,
                BossEscortType = (WildSpawnType)200,
                BossEscortCount = 4,
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
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharSecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharScout,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
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
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontaySecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
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
                        BossEscortDifficult = new[] { "normal" },
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
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharSecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerGluharScout,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
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
                        BossEscortDifficult = new[] { "normal" },
                    },
                    new WildSpawnSupports
                    {
                        BossEscortType = WildSpawnType.followerKolontaySecurity,
                        BossEscortAmount = 2,
                        BossEscortDifficult = new[] { "normal" },
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
                        BossEscortDifficult = new[] { "normal" },
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
}
