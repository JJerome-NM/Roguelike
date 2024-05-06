using DefaultNamespace;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstLevelScene.Multiplayer
{
    public class PlayerListEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private Button readyButton;

        private int _ownerId;
        private bool _isPlayerReady;

        #region UNITY

        private void Start()
        {
            UpdateReady();
            
            if (PhotonNetwork.LocalPlayer.ActorNumber != _ownerId)
            {
                readyButton.gameObject.SetActive(false);
                readyText.gameObject.SetActive(true);
            }
            else
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new()
                {
                    { RoguelikeGame.PlayerIsReady, _isPlayerReady }
                });

                readyButton.onClick.AddListener(() =>
                {
                    _isPlayerReady = !_isPlayerReady;

                    PhotonNetwork.LocalPlayer.SetCustomProperties(new()
                    {
                        { RoguelikeGame.PlayerIsReady, _isPlayerReady }
                    });

                    UpdateReady();
                    
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PlayerList.Instance.LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }

        #endregion

        private void UpdateReady()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == _ownerId)
            {
                var readyButtonText = readyButton.GetComponentInChildren<TextMeshProUGUI>();
                readyButtonText.SetText(_isPlayerReady ? "Ready" : "NotReady");
                readyButtonText.color = _isPlayerReady ? new Color(0.03f, 0.64f, 0.09f) : new Color(1f, 0.22f, 0.24f);
            }
            else
            {
                readyText.SetText(_isPlayerReady ? "Ready" : "NotReady");
                readyText.color = _isPlayerReady ? new Color(0.03f, 0.64f, 0.09f) : new Color(1f, 0.22f, 0.24f);
            }
        }
        
        public void Initialize(Photon.Realtime.Player newPlayer)
        {
            _ownerId = newPlayer.ActorNumber;
            playerName.SetText(newPlayer.NickName);

            if (PhotonNetwork.LocalPlayer.ActorNumber == _ownerId)
            {
                readyButton.gameObject.SetActive(true);
                readyText.gameObject.SetActive(false);
            }
            else
            {
                readyButton.gameObject.SetActive(false);
                readyText.gameObject.SetActive(true);
            }

            if (newPlayer.CustomProperties.TryGetValue(RoguelikeGame.PlayerIsReady, out var isReady))
            {
                _isPlayerReady = (bool)isReady;
            }

            Debug.Log(newPlayer.NickName + " - " + _isPlayerReady);
            
            UpdateReady();
        }

        public void SetPlayerReady(bool ready)
        {
            _isPlayerReady = ready;
            readyText.SetText(_isPlayerReady ? "Ready" : "NotReady");
            readyText.color = _isPlayerReady ? new Color(0.03f, 0.64f, 0.09f) : new Color(1f, 0.22f, 0.24f);
        }
    }
}