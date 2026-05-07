using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionFallbackRegroupAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;

        public LegionFallbackRegroupAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = LegionGroupManager.Instance;
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

            _nextMoveTime = Time.time + 1.5f;

            if (_manager == null)
            {
                return;
            }

            var leader = _manager.Leader;
            if (leader == null || leader == BotOwner)
            {
                BotOwner.Mover.Stop();
                return;
            }

            float dist = Vector3.Distance(BotOwner.Position, leader.Position);
            if (dist < 5f)
            {
                BotOwner.Mover.Stop();
                return;
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(leader.Position, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }
    }
}
