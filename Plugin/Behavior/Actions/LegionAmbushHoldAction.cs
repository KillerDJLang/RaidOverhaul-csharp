using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Managers;
using UnityEngine;

namespace RaidOverhaul.Behavior.Actions
{
    internal class LegionAmbushHoldAction : CustomLogic
    {
        private LegionGroupManager _manager;
        private float _nextCheckTime;
        private float _ambushStartTime;
        private const float AmbushTimeout = 60f;
        private const float KillZoneRadius = 20f;
        private const float AbandonRadius = 55f;

        public LegionAmbushHoldAction(BotOwner botOwner)
            : base(botOwner) { }

        public override void Start()
        {
            base.Start();
            _manager = LegionGroupManager.Instance;
            _ambushStartTime = Time.time;
            BotOwner.PatrollingData.Pause();
            BotOwner.SetPose(0f);
            BotOwner.Mover.Stop();
        }

        public override void Stop()
        {
            base.Stop();
            BotOwner.SetPose(1f);
            BotOwner.PatrollingData.Unpause();
        }

        public override void Update(CustomLayer.ActionData data)
        {
            if (Time.time < _nextCheckTime)
            {
                return;
            }

            _nextCheckTime = Time.time + 3f;

            if (_manager == null)
            {
                return;
            }

            var target = _manager.NearbyUndetectedPMC;
            if (target == null || !target.HealthController.IsAlive)
            {
                _manager.ClearNearbyUndetectedPMC();
                return;
            }

            float dist = Vector3.Distance(BotOwner.Position, target.Transform.position);

            if (dist > AbandonRadius || Time.time - _ambushStartTime > AmbushTimeout)
            {
                _manager.ClearNearbyUndetectedPMC();
                return;
            }

            if (dist <= KillZoneRadius)
            {
                _manager.ClearNearbyUndetectedPMC();
            }
        }
    }
}
