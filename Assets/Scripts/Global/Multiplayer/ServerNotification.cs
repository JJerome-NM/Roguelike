using System.Collections;
using TMPro;
using UIController;
using UnityEngine;

namespace Global.Multiplayer
{
    public class ServerNotification : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float defaultTimeout = 2f;
        
        private Coroutine _notificationCoroutine;

        private void Start()
        {
            NotificationEventManager.OnNotification.AddListener(ShowNotification);
            notificationPanel.SetActive(false);
        }

        private void ShowNotification(string message)
        {
            if (_notificationCoroutine != null)
            {
                StopCoroutine(_notificationCoroutine);
                _notificationCoroutine = null;
            }
            
            _notificationCoroutine = StartCoroutine(HideNotification(message, defaultTimeout));
        }

        private IEnumerator HideNotification(string message, float timeout)
        {
            notificationText.SetText(message);
            notificationPanel.SetActive(true);
            
            yield return new WaitForSeconds(timeout);

            notificationPanel.SetActive(false);
        }
    }
}