using System.Collections.Generic;
using DefaultNamespace;
using ExitGames.Client.Photon;
using Global.Multiplayer;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstLevelScene.Multiplayer
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameObject playerPrefab;
        
        private PlayerList _playerList;
        private Dictionary<int, GameObject> _playerListEntrys = new();
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _playerList = PlayerList.Instance;
            _playerListEntrys = _playerList._playerListEntrys;
            
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                _playerList.AddPlayer(p);
            }
        }

        #region PHOTON

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(RoguelikeGame.GameIsRunning, out var isGameRunning))
            {
                if ((bool)isGameRunning)
                {
                    Vector3 pos = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
                    PhotonNetwork.Instantiate(playerPrefab.name, pos, Quaternion.identity);
                    
                    GlobalEventManager.StartGame(GameStartState.Start);
                }
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(SceneNames.Lobby);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            ServerNotificationEventManager.SendNotification(newPlayer.NickName + " enter the room");
            
            _playerList.AddPlayer(newPlayer);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            ServerNotificationEventManager.SendNotification(otherPlayer.NickName + " leave the room");
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (_playerListEntrys.TryGetValue(targetPlayer.ActorNumber, out GameObject entry))
            {
                if (changedProps.TryGetValue(RoguelikeGame.PlayerIsReady, out var IsPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)IsPlayerReady);
                }
            }
            
            _playerList.UpdateStartButton();
        }
        
        #endregion
        
        public void Leave()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}