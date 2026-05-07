using System;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using RaidOverhaul.Behavior.Actions;
using RaidOverhaul.Fika;
using RaidOverhaul.Managers;
using RaidOverhaul.Models;
using UnityEngine;

namespace RaidOverhaul.Behavior.Layers
{
    internal class LegionBreachLayer : CustomLayer
    {
        private LegionGroupManager _manager;
        private Type _nextAction;
        private Type _lastAction;

        public LegionBreachLayer(BotOwner botOwner, int priority)
            : base(botOwner, priority) { }

        public override string GetName()
        {
            return "LegionBreach";
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

            if (BotOwner == _manager.Leader && _manager.GetLivingEscorts().Count > 0)
            {
                return false;
            }

            if (!_manager.HasHuntTarget)
            {
                return false;
            }

            float dist = Vector3.Distance(BotOwner.Position, _manager.HuntTargetPosition);
            if (dist > 20f && !_manager.IsBreaching)
            {
                return false;
            }

            bool isAssaulter = BotOwner == _manager.Leader || _manager.Assaulters.Contains(BotOwner);
            _nextAction = isAssaulter ? typeof(LegionBreachAssaultAction) : typeof(LegionBreachCoverAction);
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
