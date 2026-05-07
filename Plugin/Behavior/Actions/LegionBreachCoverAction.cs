using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionBreachCoverAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;
        private float _nextLookTime;

        public LegionBreachCoverAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = LegionGroupManager.Instance;
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
            if (_manager == null)
            {
                return;
            }

            if (Time.time >= _nextLookTime && _manager.HasHuntTarget)
            {
                _nextLookTime = Time.time + 1f;
                BotOwner.Steering.LookToPoint(_manager.HuntTargetPosition);
            }

            if (Time.time >= _nextMoveTime)
            {
                _nextMoveTime = Time.time + 2f;

                var leader = _manager.Leader;
                if (leader == null)
                {
                    return;
                }

                float distToLeader = Vector3.Distance(BotOwner.Position, leader.Position);
                if (distToLeader > 8f)
                {
                    Vector3 toTarget = (_manager.HuntTargetPosition - leader.Position).normalized;
                    Vector3 rightDir = Vector3.Cross(toTarget, Vector3.up).normalized;
                    Vector3 coverTarget = leader.Position + rightDir * 3f;

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
