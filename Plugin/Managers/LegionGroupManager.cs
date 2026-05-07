using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;
using RaidOverhaul.Models;
using SAIN.Components;
using UnityEngine;

namespace RaidOverhaul.Managers
{
    public class LegionGroupManager : MonoBehaviour
    {
        public static LegionGroupManager Instance { get; private set; }

        public BotOwner Leader { get; private set; }
        public bool IsReady { get; private set; }

        public IPlayer HuntTarget { get; private set; }
        public bool HasHuntTarget
        {
            get { return HuntTarget != null && HuntTarget.HealthController.IsAlive; }
        }

        public Vector3 HuntTargetPosition
        {
            get { return HuntTarget?.Transform?.position ?? Vector3.zero; }
        }

        public bool IsFallingBack { get; private set; }
        private float _fallbackClearTime;
        private float _fallbackRearmTime;

        public Vector3 LastKnownTargetPosition { get; private set; }
        public bool HasRecentlyLostTarget { get; private set; }
        private float _lostTargetClearTime;
        private bool _hadTargetLastFrame;

        public IPlayer NearbyUndetectedPMC { get; private set; }

        public List<BotOwner> Assaulters { get; private set; } = new List<BotOwner>();
        public List<BotOwner> Suppressors { get; private set; } = new List<BotOwner>();
        public bool IsBreaching { get; private set; }

        private readonly List<BotOwner> _legionBots = new List<BotOwner>();
        private readonly List<BotOwner> _livingEscortsCache = new List<BotOwner>();
        private float _nextBotScanTime;
        private float _nextTargetScanTime;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (Time.time >= _nextBotScanTime)
            {
                _nextBotScanTime = Time.time + 2f;
                ScanForLegionBots();
                ElectLeader();
                UpdateFallbackState();
                UpdateBreachState();
            }

            if (Time.time >= _nextTargetScanTime)
            {
                _nextTargetScanTime = Time.time + 3f;
                ScanForTarget();
                ScanForNearbyUndetectedPMC();
                AssignEscortRoles();
            }

            if (HasHuntTarget)
            {
                LastKnownTargetPosition = HuntTargetPosition;
                _hadTargetLastFrame = true;
                HasRecentlyLostTarget = false;
            }
            else if (_hadTargetLastFrame)
            {
                _hadTargetLastFrame = false;
                HasRecentlyLostTarget = true;
                _lostTargetClearTime = Time.time + 90f;
            }

            if (HasRecentlyLostTarget && Time.time >= _lostTargetClearTime)
            {
                HasRecentlyLostTarget = false;
            }
        }

        private void ScanForTarget()
        {
            if (HuntTarget != null && HuntTarget.HealthController.IsAlive)
            {
                return;
            }

            if (Leader == null)
            {
                HuntTarget = null;
                return;
            }

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null)
            {
                return;
            }

            IPlayer bestTarget = null;
            float bestDist = float.MaxValue;

            foreach (var player in gameWorld.RegisteredPlayers)
            {
                if (player == null || !player.HealthController.IsAlive)
                {
                    continue;
                }

                var role = player.Profile?.Info?.Settings?.Role;
                if (player.IsAI && role != null && WildSpawnTypeExtensions.IsLegion(role.Value))
                {
                    continue;
                }

                if (player.Side != EPlayerSide.Bear && player.Side != EPlayerSide.Usec)
                {
                    continue;
                }

                float dist = Vector3.Distance(Leader.Position, player.Transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTarget = player;
                }
            }

            bestTarget ??= Leader.Memory?.GoalEnemy?.Person;

            HuntTarget = bestTarget;

            if (bestTarget != null)
            {
                AcquireTargetOnAllBots(bestTarget);
            }
        }

        private void AcquireTargetOnAllBots(IPlayer target)
        {
            foreach (var bot in _legionBots)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                bot.BotsGroup.AddEnemy(target, EBotEnemyCause.addPlayer);

                if (BotManagerComponent.Instance == null || !BotManagerComponent.Instance.GetSAIN(bot, out var sainBot))
                {
                    continue;
                }

                var sainEnemy = sainBot.EnemyController.CheckAddEnemy(target);
                sainEnemy?.KnownPlaces.UpdateSeenPlace(target.Transform.position, Time.time);
            }
        }

        private void ScanForNearbyUndetectedPMC()
        {
            if (!IsReady)
            {
                return;
            }

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null)
            {
                NearbyUndetectedPMC = null;
                return;
            }

            foreach (var player in gameWorld.RegisteredPlayers)
            {
                if (player == null || !player.HealthController.IsAlive)
                {
                    continue;
                }

                var role = player.Profile?.Info?.Settings?.Role;
                if (player.IsAI && role != null && WildSpawnTypeExtensions.IsLegion(role.Value))
                {
                    continue;
                }

                if (player.Side != EPlayerSide.Bear && player.Side != EPlayerSide.Usec)
                {
                    continue;
                }

                bool detected = false;
                bool nearby = false;
                foreach (var b in _legionBots)
                {
                    if (b == null || b.IsDead)
                    {
                        continue;
                    }

                    if (b.Memory?.GoalEnemy?.Person == player)
                    {
                        detected = true;
                        break;
                    }
                    if (!nearby && Vector3.Distance(b.Position, player.Transform.position) < 50f)
                    {
                        nearby = true;
                    }
                }

                if (detected || !nearby)
                {
                    continue;
                }

                NearbyUndetectedPMC = player;
                return;
            }

            NearbyUndetectedPMC = null;
        }

        public void ClearNearbyUndetectedPMC()
        {
            NearbyUndetectedPMC = null;
        }

        private void AssignEscortRoles()
        {
            Assaulters.Clear();
            Suppressors.Clear();

            var escorts = GetLivingEscorts();
            for (int i = 0; i < escorts.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Assaulters.Add(escorts[i]);
                }
                else
                {
                    Suppressors.Add(escorts[i]);
                }
            }
        }

        private void UpdateFallbackState()
        {
            if (IsFallingBack)
            {
                if (Time.time >= _fallbackClearTime)
                {
                    bool allClose =
                        Leader == null
                        || _legionBots
                            .Where(b => b != null && !b.IsDead && b != Leader)
                            .All(b => Vector3.Distance(b.Position, Leader.Position) <= 10f);

                    if (allClose)
                    {
                        IsFallingBack = false;
                        _fallbackRearmTime = Time.time + 30f;
                    }
                }
                return;
            }

            if (Leader == null)
            {
                return;
            }

            if (Time.time < _fallbackRearmTime)
            {
                return;
            }

            var hp = Leader.HealthController;
            var chest = hp.GetBodyPartHealth(EBodyPart.Chest);
            if (chest.Maximum <= 0)
            {
                return;
            }

            if (chest.Current / chest.Maximum < 0.5f)
            {
                IsFallingBack = true;
                _fallbackClearTime = Time.time + 30f;
            }
        }

        public List<BotOwner> GetLivingEscorts()
        {
            return _livingEscortsCache;
        }

        public int TotalLivingCount
        {
            get { return _legionBots.Count(b => b != null && !b.IsDead); }
        }

        public BotOwner GetNearestAlly(Vector3 fromPosition, BotOwner exclude)
        {
            BotOwner nearest = null;
            float nearestDist = float.MaxValue;
            foreach (var bot in _legionBots)
            {
                if (bot == null || bot.IsDead || bot == exclude)
                {
                    continue;
                }

                float dist = Vector3.Distance(fromPosition, bot.Position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = bot;
                }
            }
            return nearest;
        }

        private void ScanForLegionBots()
        {
            _legionBots.RemoveAll(b => b == null || b.IsDead);

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null)
            {
                return;
            }

            foreach (var player in gameWorld.RegisteredPlayers)
            {
                if (player == null || !player.IsAI)
                {
                    continue;
                }

                var botOwner = player.AIData?.BotOwner;
                if (botOwner == null || botOwner.IsDead)
                {
                    continue;
                }

                var role = botOwner.Profile?.Info?.Settings?.Role;
                if (role == null || !WildSpawnTypeExtensions.IsLegion(role.Value))
                {
                    continue;
                }

                if (!_legionBots.Contains(botOwner))
                {
                    _legionBots.Add(botOwner);
                }
            }

            IsReady = _legionBots.Count > 0;

            _livingEscortsCache.Clear();
            foreach (var b in _legionBots)
            {
                if (b != null && !b.IsDead && b != Leader)
                {
                    _livingEscortsCache.Add(b);
                }
            }
        }

        private void UpdateBreachState()
        {
            if (!HasHuntTarget)
            {
                IsBreaching = false;
                return;
            }

            foreach (var bot in _livingEscortsCache)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                if (Vector3.Distance(bot.Position, HuntTargetPosition) <= 20f)
                {
                    IsBreaching = true;
                    return;
                }
            }

            bool anyClose = false;
            foreach (var bot in _livingEscortsCache)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                if (Vector3.Distance(bot.Position, HuntTargetPosition) <= 25f)
                {
                    anyClose = true;
                    break;
                }
            }
            if (!anyClose)
            {
                IsBreaching = false;
            }
        }

        private void ElectLeader()
        {
            var alive = _legionBots.Where(b => b != null && !b.IsDead).ToList();
            Leader = alive.FirstOrDefault(b => (int?)b.Profile?.Info?.Settings?.Role == 199) ?? alive.FirstOrDefault();
        }
    }
}
