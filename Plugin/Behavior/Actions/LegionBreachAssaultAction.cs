using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionBreachAssaultAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextMoveTime;

        public LegionBreachAssaultAction(BotOwner botOwner)
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

            var goalEnemy = BotOwner.Memory.GoalEnemy;
            Vector3 destination = goalEnemy != null ? goalEnemy.CurrPosition : _manager.HuntTargetPosition;

            if (destination == Vector3.zero)
            {
                return;
            }

            BotOwner.SetPose(0f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(destination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }
    }
}
