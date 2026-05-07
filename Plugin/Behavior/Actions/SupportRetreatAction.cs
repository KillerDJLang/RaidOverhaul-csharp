using System;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class SupportRetreatAction : CustomLogic
    {
        private SupportBotManager _manager;
        private float _nextMoveTime;
        private float _retreatStartTime;
        private Vector3 _retreatDestination;
        private bool _destinationSet;

        public SupportRetreatAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = SupportBotManager.Instance;
            _destinationSet = false;
            _retreatStartTime = Time.time;
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
                TryDeactivate();
                return;
            }

            if (!_destinationSet)
            {
                Vector3 awayDir = (BotOwner.Position - _manager.ProtectedPlayer.Transform.position).normalized;
                if (awayDir.sqrMagnitude < 0.01f)
                {
                    awayDir = Vector3.forward;
                }
                _retreatDestination = BotOwner.Position + awayDir * 80f;
                _destinationSet = true;
            }

            float distToDestination = Vector3.Distance(BotOwner.Position, _retreatDestination);
            float elapsed = Time.time - _retreatStartTime;

            if (distToDestination < 5f || elapsed > 45f)
            {
                TryDeactivate();
                return;
            }

            BotOwner.SetPose(1f);
            BotOwner.SetTargetMoveSpeed(1f);
            BotOwner.GoToPoint(_retreatDestination, mustHaveWay: false);
            BotOwner.Steering.LookToMovingDirection();
        }

        private void TryDeactivate()
        {
            try
            {
                BotOwner.LeaveData.RemoveFromMap();
            }
            catch (Exception ex)
            {
                Plugin._log.LogError($"[SupportRetreat] RemoveFromMap failed: {ex.Message}");
            }
        }
    }
}
