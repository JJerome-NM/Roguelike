using ExitGames.Client.Photon;
using FirstLevelScene;
using Photon.Pun;

namespace DefaultNamespace
{
    public class GlobalStateManager : MonoBehaviourPunCallbacks
    {
        private static bool _isFirstStart = true;

        public static bool IsFirstStart
        {
            get
            {
                var oldVersion = _isFirstStart;
                _isFirstStart = false;
                return oldVersion;
            }

            private set => _isFirstStart = false;
        }

        public static bool IsGameRunning { get; private set; }
        public static bool IsGameStopped { get; private set; }

        #region UNITY

        private void Awake()
        {
            _isFirstStart = true;
            IsGameStopped = false;
            
            GlobalEventManager.OnGameStarted.AddListener((_) => IsGameStopped = false);
            GlobalEventManager.OnGameStopped.AddListener((_) => IsGameStopped = true);
        }

        #endregion

        #region PHOTON

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(RoguelikeGame.GameIsRunning, out var isGameRunning))
            {
                IsGameRunning = (bool)isGameRunning;
            }
        }

        #endregion
    }
}