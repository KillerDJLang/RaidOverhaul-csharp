using System.Collections.Generic;
using Comfort.Common;
using EFT;
using RaidOverhaul.Models;
using SAIN.Components;
using UnityEngine;
using UnityEngine.AI;

namespace RaidOverhaul.Managers
{
    public class SupportBotManager : MonoBehaviour
    {
        public static SupportBotManager Instance { get; private set; }

        public Player ProtectedPlayer { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDespawning { get; private set; }
        public List<BotOwner> Suppressors { get; private set; } = new List<BotOwner>();
        public List<BotOwner> Assaulters { get; private set; } = new List<BotOwner>();
        public bool IsBreaching { get; private set; }
        public IPlayer ThreatTarget { get; private set; }
        public bool HasThreatTarget
        {
            get { return ThreatTarget != null && ThreatTarget.HealthController.IsAlive; }
        }

        public Vector3 ThreatTargetPosition
        {
            get { return ThreatTarget?.Transform?.position ?? Vector3.zero; }
        }

        private readonly List<BotOwner> _supportBots = new List<BotOwner>();
        private readonly List<BotOwner> _livingSupportBotsCache = new List<BotOwner>();
        private readonly HashSet<int> _preExistingBotIds = new HashSet<int>();

        private float _despawnTime;
        private float _nextScanTime;
        private float _nextFriendshipCheckTime;
        private float _spawnTimeout;
        private int _pendingSpawns;
        private bool _isSubscribedToSpawner;
        private Vector3 _spawnPosition;

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
            UnsubscribeFromSpawner();
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (!IsDespawning && Time.time >= _despawnTime)
            {
                IsDespawning = true;
            }

            if (Time.time >= _nextScanTime)
            {
                _nextScanTime = Time.time + 2f;
                ScanForSupportBots();

                if (_livingSupportBotsCache.Count > 0 && (ProtectedPlayer == null || !ProtectedPlayer.HealthController.IsAlive))
                {
                    TryReassignProtectedPlayer();
                }

                UpdateSuppressor();
                ScanForThreat();
                UpdateBreachState();

                if (_supportBots.Count > 0 && _livingSupportBotsCache.Count == 0)
                {
                    IsActive = false;
                }
            }

            if (Time.time >= _nextFriendshipCheckTime)
            {
                _nextFriendshipCheckTime = Time.time + 1f;
                CheckAndEnforceFriendship();
            }

            if (_pendingSpawns > 0 && Time.time >= _spawnTimeout)
            {
                UnsubscribeFromSpawner();
                if (_supportBots.Count == 0)
                {
                    IsActive = false;
                }
            }
        }

        public static Vector3 FindSpawnPosition(Player player)
        {
            return CalculateSpawnPosition(player);
        }

        public void Activate(Player player, Vector3 spawnPosition)
        {
            ProtectedPlayer = player;
            IsActive = true;
            IsDespawning = false;
            Suppressors.Clear();
            Assaulters.Clear();
            _despawnTime = Time.time + 900f;
            _supportBots.Clear();
            _livingSupportBotsCache.Clear();
            _pendingSpawns = 0;
            _isSubscribedToSpawner = false;
            SpawnSupportBots(spawnPosition);
        }

        private void SpawnSupportBots(Vector3 spawnPosition)
        {
            if (ProtectedPlayer == null)
            {
                return;
            }

            _spawnPosition = spawnPosition;

            var botGame = Singleton<IBotGame>.Instance;
            if (botGame == null)
            {
                IsActive = false;
                return;
            }

            var spawner = botGame.BotsController?.BotSpawner;
            if (spawner == null)
            {
                IsActive = false;
                return;
            }

            _preExistingBotIds.Clear();
            foreach (var b in spawner.Bots.BotOwners)
            {
                _preExistingBotIds.Add(b.GetInstanceID());
            }

            spawner.OnBotCreated += OnBotCreated;
            _isSubscribedToSpawner = true;
            _pendingSpawns = 2;
            _spawnTimeout = Time.time + 30f;

            spawner.SpawnBotByTypeForce(1, (WildSpawnType)201, BotDifficulty.hard, new BotSpawnParams());
            spawner.SpawnBotByTypeForce(1, (WildSpawnType)202, BotDifficulty.hard, new BotSpawnParams());
        }

        private void OnBotCreated(BotOwner bot)
        {
            if (!_isSubscribedToSpawner || _pendingSpawns <= 0)
            {
                return;
            }

            if (_preExistingBotIds.Contains(bot.GetInstanceID()))
            {
                return;
            }

            var role = bot.Profile?.Info?.Settings?.Role;
            if (role == null || !WildSpawnTypeExtensions.IsWolf(role.Value))
            {
                return;
            }

            int teamCount = _supportBots.Count;
            float spreadAngle = teamCount * 90f;
            Vector3 spreadDir = Quaternion.Euler(0, spreadAngle, 0) * Vector3.forward;
            Vector3 targetPos = _spawnPosition + spreadDir * 3f;

            if (NavMesh.SamplePosition(targetPos, out var hit, 10f, NavMesh.AllAreas) && Mathf.Abs(hit.position.y - _spawnPosition.y) <= 3f)
            {
                bot.Transform.position = hit.position;
            }
            else
            {
                bot.Transform.position = _spawnPosition;
            }

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld != null)
            {
                foreach (var registeredPlayer in gameWorld.RegisteredPlayers)
                {
                    if (registeredPlayer == null || registeredPlayer.IsAI || !registeredPlayer.HealthController.IsAlive)
                    {
                        continue;
                    }

                    var humanPlayer = registeredPlayer as Player;
                    if (humanPlayer == null)
                    {
                        continue;
                    }

                    bot.BotsGroup.AddAlly(humanPlayer);
                    if (bot.BotsGroup.IsEnemy(humanPlayer))
                    {
                        bot.BotsGroup.RemoveEnemy(humanPlayer);
                    }
                    TryClearSAINEnemy(bot, humanPlayer);
                }
            }

            foreach (var existing in _supportBots)
            {
                if (existing == null || existing.IsDead)
                {
                    continue;
                }

                existing.BotsGroup.AddAlly(bot.GetPlayer);
                bot.BotsGroup.AddAlly(existing.GetPlayer);
            }

            _supportBots.Add(bot);
            _pendingSpawns--;

            if (_pendingSpawns <= 0)
            {
                UnsubscribeFromSpawner();
                FinalizeTeamFriendship();
                RefreshLivingCache();
            }
        }

        private void FinalizeTeamFriendship()
        {
            for (int i = 0; i < _supportBots.Count; i++)
            {
                for (int j = 0; j < _supportBots.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var a = _supportBots[i];
                    var b = _supportBots[j];
                    if (a == null || b == null || a.IsDead || b.IsDead)
                    {
                        continue;
                    }

                    a.BotsGroup.AddAlly(b.GetPlayer);
                }
            }
        }

        private void ScanForThreat()
        {
            IPlayer bestThreat = null;
            float bestDist = float.MaxValue;
            foreach (var bot in _livingSupportBotsCache)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                var enemy = bot.Memory?.GoalEnemy?.Person;
                if (enemy == null || !enemy.HealthController.IsAlive)
                {
                    continue;
                }

                float dist = Vector3.Distance(bot.Position, enemy.Transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestThreat = enemy;
                }
            }
            ThreatTarget = bestThreat;

            if (bestThreat != null && bestThreat.IsAI)
            {
                AcquireEnemyOnAllBots(bestThreat);
            }
        }

        private void AcquireEnemyOnAllBots(IPlayer target)
        {
            foreach (var bot in _livingSupportBotsCache)
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

        private void UpdateBreachState()
        {
            if (!HasThreatTarget)
            {
                IsBreaching = false;
                return;
            }

            foreach (var bot in _livingSupportBotsCache)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                if (Vector3.Distance(bot.Position, ThreatTargetPosition) <= 20f)
                {
                    IsBreaching = true;
                    return;
                }
            }

            bool anyClose = false;
            foreach (var bot in _livingSupportBotsCache)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                if (Vector3.Distance(bot.Position, ThreatTargetPosition) <= 25f)
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

        private void ScanForSupportBots()
        {
            _supportBots.RemoveAll(b => b == null || b.IsDead);
            RefreshLivingCache();
        }

        private void RefreshLivingCache()
        {
            _livingSupportBotsCache.Clear();
            foreach (var b in _supportBots)
            {
                if (b != null && !b.IsDead)
                {
                    _livingSupportBotsCache.Add(b);
                }
            }
        }

        private void UpdateSuppressor()
        {
            Suppressors.Clear();
            Assaulters.Clear();

            if (_livingSupportBotsCache.Count >= 1)
            {
                Suppressors.Add(_livingSupportBotsCache[0]);
            }

            if (_livingSupportBotsCache.Count >= 2)
            {
                Assaulters.Add(_livingSupportBotsCache[1]);
            }
        }

        private void CheckAndEnforceFriendship()
        {
            if (_livingSupportBotsCache.Count == 0)
            {
                return;
            }

            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null)
            {
                return;
            }

            foreach (var bot in _livingSupportBotsCache)
            {
                if (bot == null || bot.IsDead)
                {
                    continue;
                }

                foreach (var registeredPlayer in gameWorld.RegisteredPlayers)
                {
                    if (registeredPlayer == null || registeredPlayer.IsAI || !registeredPlayer.HealthController.IsAlive)
                    {
                        continue;
                    }

                    var humanPlayer = registeredPlayer as Player;
                    if (humanPlayer == null)
                    {
                        continue;
                    }

                    bot.BotsGroup.AddAlly(humanPlayer);
                    if (bot.BotsGroup.IsEnemy(humanPlayer))
                    {
                        bot.BotsGroup.RemoveEnemy(humanPlayer);
                    }
                    TryClearSAINEnemy(bot, humanPlayer);

                    if (ReferenceEquals(bot.Memory?.GoalEnemy?.Person, humanPlayer))
                    {
                        bot.Memory.GoalEnemy = null;
                    }
                }
            }
        }

        private void TryReassignProtectedPlayer()
        {
            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null)
            {
                return;
            }

            foreach (var registeredPlayer in gameWorld.RegisteredPlayers)
            {
                if (registeredPlayer == null || registeredPlayer.IsAI || !registeredPlayer.HealthController.IsAlive)
                {
                    continue;
                }

                ProtectedPlayer = registeredPlayer as Player;
                if (ProtectedPlayer != null)
                {
                    return;
                }
            }
        }

        private static void TryClearSAINEnemy(BotOwner bot, IPlayer humanPlayer)
        {
            if (BotManagerComponent.Instance == null)
            {
                return;
            }
            if (!BotManagerComponent.Instance.GetSAIN(bot, out var sainBot))
            {
                return;
            }
            sainBot.EnemyController.RemoveEnemy(humanPlayer.ProfileId);
        }

        private void UnsubscribeFromSpawner()
        {
            if (!_isSubscribedToSpawner)
            {
                return;
            }

            _isSubscribedToSpawner = false;

            var botGame = Singleton<IBotGame>.Instance;
            var spawner = botGame?.BotsController?.BotSpawner;
            if (spawner != null)
            {
                spawner.OnBotCreated -= OnBotCreated;
            }
        }

        public List<BotOwner> GetLivingBots()
        {
            return _livingSupportBotsCache;
        }

        private static Vector3 CalculateSpawnPosition(Player player)
        {
            Vector3 playerPos = player.Transform.position;
            Vector3 playerFwd = player.Transform.forward;
            const float halfFov = 63f;

            float[] angles = new float[] { 180f, 160f, 200f, 140f, 220f, 120f, 240f, 90f, 270f };
            float[] distances = new float[] { 20f, 15f, 25f, 12f };
            foreach (float dist in distances)
            {
                foreach (float angle in angles)
                {
                    Vector3 candidate = playerPos + Quaternion.Euler(0, angle, 0) * playerFwd * dist;
                    if (TryValidatePosition(candidate, playerPos, playerFwd, halfFov, 3f, out Vector3 result))
                    {
                        return result;
                    }
                }
            }

            float[] fallbackDist = new float[] { 10f, 8f, 5f };
            for (int deg = 0; deg < 360; deg += 30)
            {
                foreach (float dist in fallbackDist)
                {
                    Vector3 candidate = playerPos + Quaternion.Euler(0, deg, 0) * Vector3.forward * dist;
                    if (TryValidatePosition(candidate, playerPos, Vector3.zero, 0f, 3f, out Vector3 result))
                    {
                        return result;
                    }
                }
            }

            float[] pass3Dist = new float[] { 8f, 5f, 3f };
            for (int deg = 0; deg < 360; deg += 30)
            {
                foreach (float dist in pass3Dist)
                {
                    Vector3 candidate = playerPos + Quaternion.Euler(0, deg, 0) * Vector3.forward * dist;
                    if (TryValidatePosition(candidate, playerPos, Vector3.zero, 0f, 15f, out Vector3 result))
                    {
                        return result;
                    }
                }
            }

            return Vector3.zero;
        }

        private static bool TryValidatePosition(
            Vector3 candidate,
            Vector3 playerPos,
            Vector3 playerFwd,
            float halfFov,
            float maxYDelta,
            out Vector3 result
        )
        {
            result = Vector3.zero;

            if (!NavMesh.SamplePosition(candidate, out var hit, 10f, NavMesh.AllAreas))
            {
                return false;
            }

            if (Mathf.Abs(hit.position.y - playerPos.y) > maxYDelta)
            {
                return false;
            }

            if (halfFov > 0f)
            {
                float dot = Vector3.Dot(playerFwd.normalized, (hit.position - playerPos).normalized);
                if (Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg < halfFov)
                {
                    return false;
                }
            }

            var path = new NavMeshPath();
            if (!NavMesh.CalculatePath(hit.position, playerPos, NavMesh.AllAreas, path))
            {
                return false;
            }

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            result = hit.position;
            return true;
        }
    }
}
