using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionLeaderFollowEscortsAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;

        public LegionLeaderFollowEscortsAction(BotOwner botOwner)
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

            var escorts = _manager.GetLivingEscorts();
            Vector3 destination;

            if (escorts.Count > 0)
            {
                Vector3 centroid = Vector3.zero;
                foreach (var escort in escorts)
                {
                    centroid += escort.Position;
                }

                centroid /= escorts.Count;

                Vector3 targetPos = _manager.HuntTargetPosition;
                Vector3 approachDir = (targetPos - centroid).normalized;
                destination = centroid - approachDir * 10f;
            }
            else
            {
                destination = _manager.HuntTargetPosition;
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(destination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }
    }
}
