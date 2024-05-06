using UnityEngine;
using UnityEngine.Events;

namespace UIController
{
    public class NotificationEventManager
    {
        public static readonly UnityEvent<string> OnNotification = new();

        public static void ShowNotification(string text)
        {
            OnNotification.Invoke(text);
        }
    }
}