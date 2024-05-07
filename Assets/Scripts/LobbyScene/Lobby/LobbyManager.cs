using DefaultNamespace;
using Global.Multiplayer;
using LobbyScene.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace LobbyScene.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public static LobbyManager Instance { get; private set; }
        
        [SerializeField] private UIMainPanel mainPanel;
        [SerializeField] private TextMeshProUGUI connectingText;

        private void Awake()
        {
            Instance = this;

            PhotonNetwork.AutomaticallySyncScene = true;
            mainPanel.AddNicknameChangesListener((text) => { PhotonNetwork.NickName = text;});
            mainPanel.gameObject.SetActive(false);
            connectingText.gameObject.SetActive(true);
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
            
            connectingText.gameObject.SetActive(false);
            mainPanel.gameObject.SetActive(true);
            
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log(PhotonNetwork.CountOfRooms);
            PhotonNetwork.LoadLevel(SceneNames.Level1);
            ServerNotificationEventManager.SendNotification("Joined the room");
        }

        #endregion

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom("Room - " + Random.Range(1000, 9999), new RoomOptions()
            {
                MaxPlayers = 4,
                CustomRoomProperties = new ()
                {
                    { RoguelikeGame.GameIsRunning, false }
                }
            });
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        
        public void ConnectToRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
}