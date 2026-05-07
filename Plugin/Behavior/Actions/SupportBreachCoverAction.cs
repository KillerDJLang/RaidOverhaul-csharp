using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class SupportBreachCoverAction : CustomLogic
    {
        private SupportBotManager _manager;
        private float _nextMoveTime;
        private float _nextLookTime;

        public SupportBreachCoverAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = SupportBotManager.Instance;
            BotOwner.PatrollingData.Pause();
            BotOwner.Mover.Stop();
            BotOwner.SetPose(0.7f);
        }

        public override void Stop()
        {
            base.Stop();
            BotOwner.PatrollingData.Unpause();
        }

        public override void Update(CustomLayer.ActionData data)
        {
            if (_manager == null || _manager.ProtectedPlayer == null)
            {
                return;
            }

            var goalEnemy = BotOwner.Memory.GoalEnemy;

            if (Time.time >= _nextLookTime && goalEnemy != null)
            {
                _nextLookTime = Time.time + 1f;
                BotOwner.Steering.LookToPoint(goalEnemy.CurrPosition);
            }

            if (Time.time >= _nextMoveTime)
            {
                _nextMoveTime = Time.time + 2f;

                Vector3 playerPos = _manager.ProtectedPlayer.Transform.position;
                float distToPlayer = Vector3.Distance(BotOwner.Position, playerPos);

                if (distToPlayer > 8f)
                {
                    Vector3 playerFwd = _manager.ProtectedPlayer.Transform.forward;
                    Vector3 rightDir = Vector3.Cross(playerFwd, Vector3.up).normalized;
                    Vector3 coverTarget = playerPos + rightDir * 3f;

                    BotOwner.SetPose(0.7f);
                    BotOwner.SetTargetMoveSpeed(0.6f);
                    BotOwner.GoToPoint(coverTarget, mustHaveWay: false);
                }
                else
                {
                    BotOwner.Mover.Stop();
                }
            }
        }
    }
}
