using System;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Behavior.Actions;
using RaidOverhaul.Fika;
using RaidOverhaul.Managers;
using RaidOverhaul.Models;

namespace RaidOverhaul.Behavior.Layers
{
    internal class LegionSweepLayer : CustomLayer
    {
        private LegionGroupManager _manager;
        private Type _nextAction;
        private Type _lastAction;

        public LegionSweepLayer(BotOwner botOwner, int priority)
            : base(botOwner, priority) { }

        public override string GetName()
        {
            return "LegionSweep";
        }

        public override bool IsActive()
        {
            if (!FikaBridge.AmHost())
            {
                return false;
            }

            if (!WildSpawnTypeExtensions.IsLegion(BotOwner.Profile.Info.Settings.Role))
            {
                return false;
            }

            _manager = LegionGroupManager.Instance;
            if (_manager == null || !_manager.IsReady)
            {
                return false;
            }

            if (!_manager.HasRecentlyLostTarget || _manager.HasHuntTarget)
            {
                return false;
            }

            if (BotOwner.Memory.GoalEnemy != null)
            {
                return false;
            }

            _nextAction = typeof(LegionSweepAdvanceAction);
            return true;
        }

        public override Action GetNextAction()
        {
            _lastAction = _nextAction;
            return new Action(_nextAction, _nextAction.Name);
        }

        public override bool IsCurrentActionEnding()
        {
            return _nextAction != _lastAction;
        }
    }
}
