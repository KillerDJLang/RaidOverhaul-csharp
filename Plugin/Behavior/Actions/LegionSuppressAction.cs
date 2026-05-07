using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionSuppressAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextLookTime;

        public LegionSuppressAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = LegionGroupManager.Instance;
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

            _nextLookTime = Time.time + 0.5f;

            if (_manager == null)
            {
                return;
            }

            var goalEnemy = BotOwner.Memory.GoalEnemy;
            if (goalEnemy == null)
            {
                return;
            }

            BotOwner.Steering.LookToPoint(goalEnemy.CurrPosition);
        }
    }
}
