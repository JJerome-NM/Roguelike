using FirstLevelScene;
using FirstLevelScene.Game;
using Photon.Pun;
using UnityEngine;

namespace InteractObjects
{
    [RequireComponent(typeof(PhotonView))]
    public class RuneController : MonoBehaviour
    {
        [SerializeField] private GameObject glow;

        private bool _isCollected = false;
        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            
            GlobalEventManager.OnGameStarted.AddListener((state) =>
            {
                if (state != GameStartState.Resume)
                {
                    glow.SetActive(false);
                    _isCollected = false;
                }
            });
        }

        public void Interact()
        {
            glow.SetActive(true);
            _isCollected = true;
            
            _photonView.RPC(nameof(CollectRuneInOthers), RpcTarget.Others);
        }

        public bool IsCollected() => _isCollected;

        #region RPC

        [PunRPC]
        private void CollectRuneInOthers()
        {
            glow.SetActive(true);
            _isCollected = true;
        }

        #endregion
    }
}