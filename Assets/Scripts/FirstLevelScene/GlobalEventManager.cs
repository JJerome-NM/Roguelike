using DefaultNamespace;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.Events;

namespace FirstLevelScene
{
    public class GlobalEventManager : MonoBehaviourPunCallbacks
    {
        public static readonly UnityEvent<GameStartState> OnGameStarted = new();
        public static readonly UnityEvent<GameEndState> OnGameStopped = new();
        public static readonly UnityEvent OnObstacleFieldsUpdate = new();
        
        private static void StartLocalGame(GameStartState state)
        {
            OnGameStarted.Invoke(state);
        }

        private static void StopLocalGame(GameEndState state)
        {
            OnGameStopped.Invoke(state);
        }

        public static void UpdateObstacleFields()
        {
            OnObstacleFieldsUpdate.Invoke();
        }

        #region MULTIPLAYER

        public static void StartGame(GameStartState state)
        {
            ChangeGameState(RoguelikeGame.GameStartState, state);
        }

        public static void StopGame(GameEndState state)
        {
            ChangeGameState(RoguelikeGame.GameEndState, state);
        }

        private static void ChangeGameState(string stateName, object value)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new()
                {
                    { stateName, value }
                });
            }
        }
        
        #endregion

        #region PHOTON

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(RoguelikeGame.GameStartState, out var gameStartState))
            {
                StartLocalGame((GameStartState) gameStartState);
            } 
            else if (propertiesThatChanged.TryGetValue(RoguelikeGame.GameEndState, out var gameEndState))
            {
                StopLocalGame((GameEndState) gameEndState);
            }
        }

        #endregion
    }
}