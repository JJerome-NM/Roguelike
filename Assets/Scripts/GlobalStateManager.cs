using UnityEngine;

namespace DefaultNamespace
{
    public class GlobalStateManager : MonoBehaviour
    {
        public static bool IsGameStopped { get; private set; }
        
        private void Awake()
        {
            GlobalEventManager.OnGameStarted.AddListener((_) => IsGameStopped = false);
            GlobalEventManager.OnGameStopped.AddListener((_) => IsGameStopped = true);
        }
    }
}