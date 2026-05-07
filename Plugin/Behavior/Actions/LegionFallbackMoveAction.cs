using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionFallbackMoveAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;
        private Vector3 _retreatDestination;
        private bool _destinationSet;

        public LegionFallbackMoveAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = LegionGroupManager.Instance;
            _destinationSet = false;
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

            if (!_destinationSet)
            {
                _retreatDestination = ComputeRetreatPoint();
                _destinationSet = true;
            }

            float distToRetreat = Vector3.Distance(BotOwner.Position, _retreatDestination);
            if (distToRetreat < 5f)
            {
                BotOwner.Mover.Stop();
                return;
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(_retreatDestination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }

        private Vector3 ComputeRetreatPoint()
        {
            if (_manager.HasHuntTarget)
            {
                Vector3 toTarget = (_manager.HuntTargetPosition - BotOwner.Position).normalized;
                return BotOwner.Position - toTarget * 30f;
            }

            var ally = _manager.GetNearestAlly(BotOwner.Position, BotOwner);
            if (ally != null)
            {
                return ally.Position;
            }

            return BotOwner.Position - BotOwner.Transform.forward * 30f;
        }
    }
}
