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
    internal class LegionGroupLayer : CustomLayer
    {
        private LegionGroupManager _manager;
        private Type _nextAction;
        private Type _lastAction;

        public LegionGroupLayer(BotOwner botOwner, int priority)
            : base(botOwner, priority) { }

        public override string GetName()
        {
            return "LegionGroup";
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

            if (_manager.HasHuntTarget)
            {
                float dist = Vector3.Distance(BotOwner.Position, _manager.HuntTargetPosition);
                if (dist <= 100f)
                {
                    return false;
                }
            }

            if (_manager.HasRecentlyLostTarget && !_manager.HasHuntTarget)
            {
                return false;
            }

            if (_manager.NearbyUndetectedPMC != null && BotOwner.Memory.GoalEnemy == null)
            {
                return false;
            }

            DetermineAction();
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

        private void DetermineAction()
        {
            var leader = _manager.Leader;

            if (leader == null)
            {
                _nextAction = typeof(LegionRegroupAction);
                return;
            }

            if (BotOwner != leader)
            {
                float dist = Vector3.Distance(BotOwner.Position, leader.Position);
                if (dist > 20f)
                {
                    _nextAction = typeof(LegionRegroupAction);
                    return;
                }
            }

            if (_manager.HasHuntTarget)
            {
                _nextAction = BotOwner == leader ? typeof(LegionLeaderFollowEscortsAction) : typeof(LegionEscortAdvanceAction);
                return;
            }

            _nextAction = typeof(LegionFormationHoldAction);
        }
    }
}
