using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionEscortAdvanceAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;

        public LegionEscortAdvanceAction(BotOwner botOwner)
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

            _nextMoveTime = Time.time + 2f;

            if (_manager == null || !_manager.HasHuntTarget)
            {
                return;
            }

            var targetPos = _manager.HuntTargetPosition;

            var escorts = _manager.GetLivingEscorts();
            int escortIndex = escorts.IndexOf(BotOwner);
            if (escortIndex < 0)
            {
                escortIndex = 0;
            }

            Vector3 approachDir = (targetPos - BotOwner.Position).normalized;
            Vector3 rightDir = Vector3.Cross(approachDir, Vector3.up).normalized;
            float lateralSign = (escortIndex % 2 == 0) ? 1f : -1f;
            Vector3 destination = targetPos + rightDir * (lateralSign * 4f);

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(destination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }
    }
}
