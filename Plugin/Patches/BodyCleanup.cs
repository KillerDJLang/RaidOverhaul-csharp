using Cysharp.Threading.Tasks;
using EFT;
using RaidOverhaul.Configs;
using RaidOverhaul.Helpers;
using UnityEngine;

namespace RaidOverhaul.Patches
{
    internal class BodyCleanup : MonoBehaviour
    {
        private static bool _maidOnStandby;
        private static float _distanceSquaredThreshold;

        public void ManualUpdate()
        {
            if (!Utils.IsInRaid() || !DJConfig.EnableClean.Value)
            {
                return;
            }

            if (!_maidOnStandby)
            {
                _distanceSquaredThreshold = DJConfig.DistToClean.Value * DJConfig.DistToClean.Value;
                StartClean().Forget();
                _maidOnStandby = true;
            }
        }

        private static async UniTaskVoid StartClean()
        {
            await UniTask.WaitForSeconds(DJConfig.TimeToClean.Value * 60f);

            if (Utils.IsInRaid())
            {
                await UniTask.WaitForSeconds(10);
                CleanupDeadBots();
            }

            _maidOnStandby = false;
        }

        internal static void MaidServiceRun()
        {
            if (!Utils.IsInRaid())
            {
                return;
            }

            _distanceSquaredThreshold = DJConfig.DistToClean.Value * DJConfig.DistToClean.Value;
            CleanupDeadBots();
        }

        private static void CleanupDeadBots()
        {
            var playerPosition = Plugin.ROPlayer.Transform.position;
            var bots = FindObjectsOfType<BotOwner>();

            foreach (var bot in bots)
            {
                if (bot == null || bot.HealthController == null)
                {
                    continue;
                }

                if (!bot.HealthController.IsAlive)
                {
                    var botPosition = bot.Transform.position;
                    var distanceSquared = (playerPosition - botPosition).sqrMagnitude;

                    if (distanceSquared >= _distanceSquaredThreshold)
                    {
                        bot.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
