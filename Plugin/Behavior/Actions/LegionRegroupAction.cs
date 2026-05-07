using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionRegroupAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;

        public LegionRegroupAction(BotOwner botOwner)
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

            var destination = GetRegroupDestination();
            if (destination == Vector3.zero)
            {
                return;
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(destination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }

        private Vector3 GetRegroupDestination()
        {
            var leader = _manager.Leader;
            if (leader != null && leader != BotOwner)
            {
                return leader.Position;
            }

            var ally = _manager.GetNearestAlly(BotOwner.Position, BotOwner);
            return ally?.Position ?? Vector3.zero;
        }
    }
}
