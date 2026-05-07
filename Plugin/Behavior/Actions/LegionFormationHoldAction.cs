using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionFormationHoldAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;

        public LegionFormationHoldAction(BotOwner botOwner)
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

            _nextMoveTime = Time.time + 3f;

            if (_manager == null)
            {
                return;
            }

            var leader = _manager.Leader;
            if (leader == null)
            {
                return;
            }

            if (BotOwner == leader)
            {
                BotOwner.Mover.Stop();
                return;
            }

            var escorts = _manager.GetLivingEscorts();
            int index = escorts.IndexOf(BotOwner);
            if (index < 0)
            {
                index = 0;
            }

            int total = escorts.Count > 0 ? escorts.Count : 1;

            float angle = index * (360f / total);
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * 6f;
            Vector3 destination = leader.Position + offset;

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(0.5f);
            BotOwner.GoToPoint(destination, mustHaveWay: false);

            Vector3 lookDir = (destination - leader.Position).normalized;
            BotOwner.Steering.LookToPoint(destination + lookDir * 10f);
        }
    }
}
