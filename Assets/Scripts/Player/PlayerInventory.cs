using DefaultNamespace;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        public int collectedRunes { get; private set; } = 0;

        public void CollectRune()
        {
            ++collectedRunes;
            
            GlobalEventManager.StopGame(GameEndState.AllRunesWasFound);
        }
    }
}