using UnityEngine.Events;

namespace UIController
{
    public static class NotificationEventManager
    {
        public static readonly UnityEvent<string> OnNotification = new();

        public static void ShowNotification(string text)
        {
            OnNotification.Invoke(text);
        }
    }
}