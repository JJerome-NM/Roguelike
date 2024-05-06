using UnityEngine.Events;

namespace Levels
{
    public static class LevelsEventManager
    {
        public static readonly UnityEvent<float> OnLevelMultiplayerUpdated = new ();
        public static readonly UnityEvent<int> OnLevelUpdated = new ();

        public static void UpdateLevelMultiplayer(float newMultiplayer)
        {
            OnLevelMultiplayerUpdated.Invoke(newMultiplayer);
        }

        public static void UpdateLevel(int newLevel)
        {
            OnLevelUpdated.Invoke(newLevel);
        }
    }
}