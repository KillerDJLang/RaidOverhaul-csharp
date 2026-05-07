using DrakiaXYZ.BigBrain.Brains;
using EFT;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class SupportBreachAssaultAction : CustomLogic
    {
        private float _nextMoveTime;

        public SupportBreachAssaultAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
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

            var goalEnemy = BotOwner.Memory.GoalEnemy;
            if (goalEnemy == null)
            {
                return;
            }

            BotOwner.SetPose(0.3f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(goalEnemy.CurrPosition, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }
    }
}
