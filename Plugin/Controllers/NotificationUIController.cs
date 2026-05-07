using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using static RaidOverhaul.Plugin;

namespace RaidOverhaul.Controllers
{
    public class NotificationUIController : MonoBehaviour
    {
        private GameObject _panelPrefab;
        private GameObject _panelInstance;
        private TMP_Text _messageText;
        private CancellationTokenSource _hideCts;

        public static NotificationUIController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _panelPrefab = NotificationCanvasPrefab;
        }

        public void Show(string message, float duration, Color color)
        {
            if (_panelInstance == null)
            {
                _panelInstance = Instantiate(_panelPrefab);
                DontDestroyOnLoad(_panelInstance);
                _messageText = _panelInstance.GetComponentInChildren<TMP_Text>();
            }

            _messageText.text = message;
            _messageText.color = color;
            _panelInstance.SetActive(true);

            _hideCts?.Cancel();
            _hideCts = new CancellationTokenSource();
            HideAfter(duration, _hideCts.Token).Forget();
        }

        private async UniTaskVoid HideAfter(float seconds, CancellationToken ct)
        {
            await UniTask.WaitForSeconds(seconds, cancellationToken: ct);
            if (_panelInstance != null)
            {
                _panelInstance.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _hideCts?.Cancel();
            if (_panelInstance != null)
            {
                Destroy(_panelInstance);
            }
        }
    }
}
