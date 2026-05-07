using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionSweepAdvanceAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;
        private Vector3 _sweepDestination;
        private bool _destinationSet;

        public LegionSweepAdvanceAction(BotOwner botOwner)
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
                _sweepDestination = ComputeSweepPosition();
                _destinationSet = true;
            }

            float dist = Vector3.Distance(BotOwner.Position, _sweepDestination);
            if (dist < 5f)
            {
                Vector3 raw = _manager.LastKnownTargetPosition - _sweepDestination;
                if (raw.sqrMagnitude < 0.01f)
                {
                    raw = _manager.LastKnownTargetPosition - BotOwner.Position;
                }

                Vector3 sweepDir = raw.normalized;
                if (sweepDir.sqrMagnitude > 0.01f)
                {
                    _sweepDestination += sweepDir * 20f;
                }
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(0.6f);
            BotOwner.GoToPoint(_sweepDestination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }

        private Vector3 ComputeSweepPosition()
        {
            Vector3 lkp = _manager.LastKnownTargetPosition;
            Vector3 sweepDir = (lkp - BotOwner.Position).normalized;
            Vector3 rightDir = Vector3.Cross(sweepDir, Vector3.up).normalized;

            var escorts = _manager.GetLivingEscorts();
            int totalSlots = escorts.Count + 1;

            int slotIndex;
            if (BotOwner == _manager.Leader)
            {
                slotIndex = totalSlots / 2;
            }
            else
            {
                int escortIndex = escorts.IndexOf(BotOwner);
                slotIndex = escortIndex < 0 ? 0 : escortIndex;
            }

            float offset = (slotIndex - (totalSlots - 1) / 2f) * 5f;
            return lkp + rightDir * offset;
        }
    }
}
