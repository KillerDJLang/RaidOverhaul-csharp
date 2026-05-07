using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class EventsEffectsController : MonoBehaviour
    {
        public static EventsEffectsController Instance { get; private set; }

        private GameObject _canvas;

        private Image _redOverlay1;
        private Image _redOverlay2;
        private Image _redOverlay3;
        private Image _redOverlay4;
        private Image _blueOverlay1;
        private Image _blueOverlay2;
        private Image _blueOverlay3;
        private Image _blueOverlay4;
        private Image _topBlueOverlay;
        private Image _cornerGlowOverlay;

        private CancellationTokenSource _huntedCts;
        private CancellationTokenSource _negativeCts;
        private CancellationTokenSource _positiveCts;
        private CancellationTokenSource _cleanupCts;
        private CancellationTokenSource _legionCts;
        private CancellationTokenSource _cornerGlowCts;

        private void Start()
        {
            Instance = this;

            if (EffectsCanvasPrefab == null)
            {
                return;
            }

            _canvas = Instantiate(EffectsCanvasPrefab);
            DontDestroyOnLoad(_canvas);

            _redOverlay1 = FindImage("RedOverlay1");
            _redOverlay2 = FindImage("RedOverlay2");
            _redOverlay3 = FindImage("RedOverlay3");
            _redOverlay4 = FindImage("RedOverlay4");
            _blueOverlay1 = FindImage("BlueOverlay1");
            _blueOverlay2 = FindImage("BlueOverlay2");
            _blueOverlay3 = FindImage("BlueOverlay3");
            _blueOverlay4 = FindImage("BlueOverlay4");
            _topBlueOverlay = FindImage("TopBlueOverlay");
            _cornerGlowOverlay = FindImage("CornerGlowOverlay");
        }

        private Image FindImage(string childName)
        {
            var t = _canvas.transform.Find(childName);
            if (t == null)
            {
                _log.LogWarning($"[EventsEffectsController] Could not find child '{childName}' in EffectsCanvas.");
                return null;
            }
            return t.GetComponent<Image>();
        }

        public void ShowHuntedInvasionEffect()
        {
            _huntedCts?.Cancel();
            _huntedCts = new CancellationTokenSource();
            ShowImages(new[] { _redOverlay1, _redOverlay2, _redOverlay3, _redOverlay4 }, 2f, _huntedCts).Forget();
        }

        public void ShowNegativeEffect()
        {
            _negativeCts?.Cancel();
            _negativeCts = new CancellationTokenSource();
            ShowImages(new[] { _redOverlay2, _redOverlay4 }, 1.5f, _negativeCts).Forget();
        }

        public void ShowPositiveEffect()
        {
            _positiveCts?.Cancel();
            _positiveCts = new CancellationTokenSource();
            ShowImages(new[] { _blueOverlay2, _blueOverlay4 }, 1.5f, _positiveCts).Forget();
        }

        public void ShowCleanupEffect()
        {
            _cleanupCts?.Cancel();
            _cleanupCts = new CancellationTokenSource();
            ShowImages(new[] { _topBlueOverlay }, 1.5f, _cleanupCts).Forget();
        }

        public void ShowLegionQuestEffect()
        {
            _legionCts?.Cancel();
            _legionCts = new CancellationTokenSource();
            ShowImages(new[] { _blueOverlay1, _blueOverlay2, _blueOverlay3, _blueOverlay4 }, 2f, _legionCts).Forget();
        }

        public void ShowCornerGlowEffect()
        {
            _cornerGlowCts?.Cancel();
            _cornerGlowCts = new CancellationTokenSource();
            ShowImages(new[] { _cornerGlowOverlay }, 1.5f, _cornerGlowCts).Forget();
        }

        private async UniTaskVoid ShowImages(Image[] images, float duration, CancellationTokenSource cts)
        {
            foreach (var img in images)
            {
                if (img == null)
                {
                    continue;
                }

                var c = img.color;
                img.color = new Color(c.r, c.g, c.b, 1f);
            }

            try
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: cts.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                foreach (var img in images)
                {
                    if (img == null)
                    {
                        continue;
                    }

                    var c = img.color;
                    img.color = new Color(c.r, c.g, c.b, 0f);
                }
            }
        }
    }
}
