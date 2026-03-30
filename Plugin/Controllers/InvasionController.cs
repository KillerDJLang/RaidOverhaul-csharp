using Comfort.Common;
using Cysharp.Threading.Tasks;
using RaidOverhaul.Fika;
using RaidOverhaul.Helpers;
using RaidOverhaul.Models;
using UnityEngine;

namespace RaidOverhaul.Controllers
{
    internal class InvasionController : MonoBehaviour
    {
        private bool _isReady;
        internal bool _invasionHasRun;
        private bool _invasionEventIsRunning;
        private double _invasionChance;

        public void ManualUpdate()
        {
            _isReady = Utils.IsInRaid();

            if (!ConfigController.ServerConfig.EnableCustomBoss || !FikaBridge.AmHost())
            {
                return;
            }

            if (!_isReady)
            {
                ResetInvasionState();
                return;
            }

            if (_invasionEventIsRunning || _invasionHasRun)
            {
                return;
            }

            ConfigController.LegionConfig = Utils.Get<LegionProgressionConfig>("/RaidOverhaul/GetLegionConfig");
            SetInvasionChance();
            InvasionEvents(_invasionChance).Forget();
            _invasionEventIsRunning = true;
        }

        private void ResetInvasionState()
        {
            _invasionHasRun = false;
        }

        private void SetInvasionChance()
        {
            try
            {
                if (ConfigController.ServerConfig.UseLegionGlobalSpawnChance)
                {
                    _invasionChance = ConfigController.ServerConfig.GlobalSpawnChance;
                }
                else
                {
                    _invasionChance = ConfigController.LegionConfig.LegionChance;
                }
            }
            catch (System.Exception ex)
            {
                Plugin._log.LogError($"[Raid Overhaul] Error setting invasion chance: {ex}");
                _invasionChance = 15;
            }
        }

        private async UniTaskVoid InvasionEvents(double chance)
        {
            await UniTask.WaitForSeconds(60);

            if (Random.Range(0, 100) < _invasionChance)
            {
                StartInvasion("legionInvasion");
            }
            _invasionHasRun = true;
            _invasionEventIsRunning = false;
        }

        internal void StartInvasion(string invasionEvent)
        {
            if (!Utils.IsInRaid())
            {
                return;
            }

            Singleton<BotEventHandler>.Instance.AnyEvent(invasionEvent);
            Plugin._log.LogInfo($"[Raid Overhaul] Starting invasion event {invasionEvent}");
        }
    }
}
