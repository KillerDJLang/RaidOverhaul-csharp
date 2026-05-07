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
    internal class LegionFallbackLayer : CustomLayer
    {
        private LegionGroupManager _manager;
        private Type _nextAction;
        private Type _lastAction;
        private bool _retreatDone;
        private float _retreatEndTime;

        public LegionFallbackLayer(BotOwner botOwner, int priority)
            : base(botOwner, priority) { }

        public override string GetName()
        {
            return "LegionFallback";
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

            if (!_manager.IsFallingBack)
            {
                _retreatDone = false;
                _retreatEndTime = 0f;
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
            if (_retreatDone)
            {
                _nextAction = typeof(LegionFallbackRegroupAction);
                return;
            }

            if (_retreatEndTime == 0f)
            {
                _retreatEndTime = Time.time + 20f;
            }

            if (Time.time >= _retreatEndTime)
            {
                _retreatDone = true;
                _nextAction = typeof(LegionFallbackRegroupAction);
                return;
            }

            _nextAction = typeof(LegionFallbackMoveAction);
        }
    }
}
