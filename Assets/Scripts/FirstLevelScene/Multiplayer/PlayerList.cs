using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace FirstLevelScene.Multiplayer
{
    public class PlayerList : MonoBehaviour
    {
        public static PlayerList Instance { get; private set; }
        
        [SerializeField] private GameObject playerListEntry;
        [SerializeField] private Button startButton;
        
        public Dictionary<int, GameObject> _playerListEntrys { get; private set; } = new();

        #region UNITY

        private void Awake()
        {
            Instance = this;
        }

        #endregion
        
        public void AddPlayer(Photon.Realtime.Player player)
        {
            AddPlayerInView(player, out var entry);
            _playerListEntrys.Add(player.ActorNumber, entry);
            startButton.gameObject.SetActive(CheckPlayersIsReady());
        }

        public void RemovePlayer(Photon.Realtime.Player player)
        {
            foreach (Transform child in gameObject.transform)
            {
                Destroy(child.gameObject);
            }
            
            _playerListEntrys.Clear();

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                if (!p.Equals(player))
                {
                    AddPlayer(p);
                }
            }
        }

        private void AddPlayerInView(Photon.Realtime.Player player, out GameObject entry)
        {
            entry = Instantiate(playerListEntry);
            entry.transform.SetParent(gameObject.transform);
            entry.transform.localScale = Vector3.one;
            RectTransform entryTransform = entry.GetComponent<RectTransform>();
            entryTransform.offsetMax = new Vector2(0, -50 * _playerListEntrys.Count);
            entryTransform.offsetMin = new Vector2(0, -50 * (_playerListEntrys.Count + 1));

            entry.GetComponent<PlayerListEntry>().Initialize(player);
        }

        public void UpdateStartButton()
        {
            startButton.gameObject.SetActive(CheckPlayersIsReady());
        }
        
        private bool CheckPlayersIsReady()
        {
            if (!PhotonNetwork.IsMasterClient) return false;

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                if (p.CustomProperties.TryGetValue(RoguelikeGame.PlayerIsReady, out var isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}