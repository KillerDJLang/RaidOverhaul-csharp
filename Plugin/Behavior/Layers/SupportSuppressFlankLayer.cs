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
    internal class SupportSuppressFlankLayer : CustomLayer
    {
        private SupportBotManager _manager;
        private Type _nextAction;
        private Type _lastAction;

        public SupportSuppressFlankLayer(BotOwner botOwner, int priority)
            : base(botOwner, priority) { }

        public override string GetName()
        {
            return "SupportSuppressFlank";
        }

        public override bool IsActive()
        {
            if (!FikaBridge.AmHost())
            {
                return false;
            }

            if (!WildSpawnTypeExtensions.IsWolf(BotOwner.Profile.Info.Settings.Role))
            {
                return false;
            }

            _manager = SupportBotManager.Instance;
            if (_manager == null || !_manager.IsActive || _manager.IsDespawning)
            {
                return false;
            }

            if (!_manager.HasThreatTarget)
            {
                return false;
            }

            float dist = Vector3.Distance(BotOwner.Position, _manager.ThreatTargetPosition);
            if (dist > 80f)
            {
                return false;
            }

            _nextAction = _manager.Assaulters.Contains(BotOwner) ? typeof(SupportFlankAction) : typeof(SupportSuppressAction);
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
