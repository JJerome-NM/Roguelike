using DefaultNamespace;
using ExitGames.Client.Photon;
using FirstLevelScene.Game;
using Levels;
using Photon.Pun;

namespace FirstLevelScene.Player
{
    public class PlayerInventory : MonoBehaviourPunCallbacks
    {
        public static PlayerInventory Instance { get; private set; }
        
        private int _globalRunesCount = 0;
        public int collectedRunes { get; private set; } = 0;

        private void Awake()
        {
            Instance = this;
            LevelsEventManager.OnLevelUpdated.AddListener((_) =>
            {
                collectedRunes = 0;
            });
            
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoguelikeGame.GlobalRunesCount, out var runesCount))
            {
                _globalRunesCount = (int)runesCount;
            }
        }

        public void CollectRune()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoguelikeGame.CollectedRunes, out var runesCount))
            {
                collectedRunes = (int)runesCount;

                PhotonNetwork.CurrentRoom.SetCustomProperties(new()
                {
                    { RoguelikeGame.CollectedRunes, (int)runesCount + 1 }
                });
            }
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(RoguelikeGame.CollectedRunes, out var runesCount))
            {
                collectedRunes = (int)runesCount;

                if (PhotonNetwork.IsMasterClient && collectedRunes >= _globalRunesCount)
                {
                    GlobalEventManager.StopGame(GameEndState.AllRunesWasFound);
                }
            }
        }
    }
}