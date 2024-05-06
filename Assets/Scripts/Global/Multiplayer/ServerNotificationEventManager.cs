using UnityEngine.Events;

namespace Global.Multiplayer
{
    public static class ServerNotificationEventManager
    {
        public static readonly UnityEvent<string> OnServerNotification = new();

        public static void SendNotification(string message)
        {
            OnServerNotification.Invoke(message);
        }
    }
}