using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;
using UnityEngine.AI;

namespace RaidOverhaul.Behavior.Actions
{
    internal class SupportFlankAction : CustomLogic
    {
        private SupportBotManager _manager;
        private float _nextMoveTime;

        public SupportFlankAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = SupportBotManager.Instance;
            BotOwner.PatrollingData.Pause();
        }

        public override void Stop()
        {
            base.Stop();
            BotOwner.PatrollingData.Unpause();
        }

        public override void Update(CustomLayer.ActionData data)
        {
            if (Time.time < _nextMoveTime)
            {
                return;
            }

            _nextMoveTime = Time.time + 2f;

            if (_manager == null)
            {
                return;
            }

            if (_manager.Suppressors.Contains(BotOwner))
            {
                return;
            }

            var goalEnemy = BotOwner.Memory.GoalEnemy;
            if (goalEnemy == null)
            {
                return;
            }

            Vector3 enemyPos = goalEnemy.CurrPosition;
            Vector3 toEnemy = (enemyPos - BotOwner.Position).normalized;
            Vector3 rightDir = Vector3.Cross(toEnemy, Vector3.up).normalized;

            int index = _manager.Assaulters.IndexOf(BotOwner);
            if (index < 0)
            {
                index = 0;
            }

            float flankSign = (index % 2 == 0) ? 1f : -1f;

            Vector3 chosenDestination = Vector3.zero;
            float[] offsets = new float[] { 15f, 8f, 5f };
            foreach (float offset in offsets)
            {
                Vector3 candidate = enemyPos + rightDir * (flankSign * offset);
                if (NavMesh.SamplePosition(candidate, out var hit, 3f, NavMesh.AllAreas) && Vector3.Distance(hit.position, candidate) <= 2f)
                {
                    chosenDestination = hit.position;
                    break;
                }
            }

            if (chosenDestination == Vector3.zero)
            {
                BotOwner.SetPose(0f);
                BotOwner.SetTargetMoveSpeed(0.3f);
                BotOwner.GoToPoint(enemyPos, mustHaveWay: false);
                BotOwner.Steering.LookToMovingDirection();
                return;
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(chosenDestination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }
    }
}
