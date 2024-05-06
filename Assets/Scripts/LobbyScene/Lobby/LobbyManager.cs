using System.Collections;
using DefaultNamespace;
using FirstLevelScene.Multiplayer;
using Global.Multiplayer;
using LobbyScene.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace LobbyScene.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private UIMainPanel mainPanel;

        private void Awake()
        {
            mainPanel.AddNicknameChangesListener((text) => { PhotonNetwork.NickName = text;});
        }

        private void Start()
        {
            PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectUsingSettings();
        }


        #region PHOTON

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions { MaxPlayers = 8 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnConnectedToMaster()
        {
            ServerNotificationEventManager.SendNotification("Connected to master");
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel(SceneNames.Level1);
            ServerNotificationEventManager.SendNotification("Joined the room");
        }

        public override void OnJoinedLobby()
        {
            mainPanel.gameObject.SetActive(false);
        }

        #endregion


        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions()
            {
                MaxPlayers = 4,
                CustomRoomProperties = new ()
                {
                    { RoguelikeGame.GameIsRunning, false }
                }
            });
        }

        public void ConnectToRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
}