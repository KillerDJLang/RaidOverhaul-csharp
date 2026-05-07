using DrakiaXYZ.BigBrain.Brains;
using EFT;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class SupportSuppressAction : CustomLogic
    {
        private float _nextLookTime;

        public SupportSuppressAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            BotOwner.PatrollingData.Pause();
            BotOwner.Mover.Stop();
            BotOwner.SetPose(1f);
        }

        public override void Stop()
        {
            base.Stop();
            BotOwner.PatrollingData.Unpause();
        }

        public override void Update(CustomLayer.ActionData data)
        {
            if (Time.time < _nextLookTime)
            {
                return;
            }

            _nextLookTime = Time.time + 1f;

            var goalEnemy = BotOwner.Memory.GoalEnemy;
            if (goalEnemy == null)
            {
                return;
            }

            BotOwner.Steering.LookToPoint(goalEnemy.CurrPosition);
        }
    }
}
