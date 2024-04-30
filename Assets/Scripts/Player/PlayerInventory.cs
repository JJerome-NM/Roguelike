using DefaultNamespace;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int globalRunesCount = 0;
        
        public int collectedRunes { get; private set; } = 0;

        public void CollectRune()
        {
            ++collectedRunes;

            if (collectedRunes >= globalRunesCount)
            {
                GlobalEventManager.StopGame(GameEndState.AllRunesWasFound);
            }
        }
    }
}