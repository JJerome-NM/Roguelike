using UnityEngine.Events;

namespace Player
{
    public static class PlayerEventManager
    {
        public static readonly UnityEvent<float> OnPlayerHealsUpdate = new();

        public static void OnPlayerHealsUpdated(float newHeals)
        {
            OnPlayerHealsUpdate.Invoke(newHeals);
        }
    }
}