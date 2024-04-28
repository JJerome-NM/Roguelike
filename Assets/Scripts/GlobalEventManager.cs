using UnityEngine.Events;

namespace DefaultNamespace
{
    public static class GlobalEventManager
    {
        public static readonly UnityEvent<GameStartState> OnGameStarted = new();
        public static readonly UnityEvent<GameEndState> OnGameStopped = new();
        public static readonly UnityEvent OnObstacleFieldsUpdate = new();
        
        public static void StartGame(GameStartState state)
        {
            OnGameStarted.Invoke(state);
        }

        public static void StopGame(GameEndState state)
        {
            OnGameStopped.Invoke(state);
        }

        public static void UpdateObstacleFields()
        {
            OnObstacleFieldsUpdate.Invoke();
        }
    }
}