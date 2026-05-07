using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class SupportAdvanceAction : CustomLogic
    {
        private SupportBotManager _manager;
        private float _nextMoveTime;

        public SupportAdvanceAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = SupportBotManager.Instance;
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

            if (_manager == null || _manager.ProtectedPlayer == null)
            {
                return;
            }

            Vector3 playerPos = _manager.ProtectedPlayer.Transform.position;
            float distToPlayer = Vector3.Distance(BotOwner.Position, playerPos);

            var bots = _manager.GetLivingBots();
            int index = bots.IndexOf(BotOwner);
            if (index < 0)
            {
                index = 0;
            }

            Vector3 playerFwd = _manager.ProtectedPlayer.Transform.forward;
            Vector3 rightDir = Vector3.Cross(playerFwd, Vector3.up).normalized;
            float lateralSign = (index % 2 == 0) ? 1f : -1f;
            Vector3 guardTarget = playerPos + rightDir * (lateralSign * 5f);

            if (distToPlayer > 12f)
            {
                BotOwner.SetPose(1f);
                BotOwner.SetTargetMoveSpeed(0.7f);
                BotOwner.GoToPoint(guardTarget, mustHaveWay: false);
                BotOwner.Steering.LookToMovingDirection();
            }
            else if (distToPlayer < 5f)
            {
                Vector3 stepBack = playerPos + (BotOwner.Position - playerPos).normalized * 7f;
                BotOwner.SetPose(1f);
                BotOwner.SetTargetMoveSpeed(0.5f);
                BotOwner.GoToPoint(stepBack, mustHaveWay: false);
                BotOwner.Steering.LookToMovingDirection();
            }
            else
            {
                BotOwner.Mover.Stop();
                Vector3 outwardDir = guardTarget - playerPos;
                if (outwardDir.sqrMagnitude > 0.01f)
                {
                    BotOwner.Steering.LookToPoint(guardTarget + outwardDir.normalized * 10f);
                }
            }
        }
    }
}
