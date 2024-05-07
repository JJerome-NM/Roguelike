using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(PhotonView))]
    public class EnemyController : MonoBehaviour
    {
        private static readonly string HealthText = "HP - ";
        
        [SerializeField] private float startHealth = 100;
        [SerializeField] private TextMeshPro health;

        private PhotonView _photonView;
        private float _health = 100;

        #region UNITY

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            
            _health = startHealth;
            
            UpdateHealsText();
        }

        #endregion

        public void Damage(float damage)
        {
            _photonView.RPC(nameof(ChangeEnemyHealsForOtherPlayers), RpcTarget.All, _health -= damage);
            
            if (_health < 0)
            {
                Kill();
            }
        }

        private void Kill()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        private void UpdateHealsText()
        {
            if (health != null)
            {
                health.SetText(HealthText + _health.ToString("F0"));
            }
        }
        
        #region RPC

        [PunRPC]
        private void ChangeEnemyHealsForOtherPlayers(float newHeals)
        {
            _health = newHeals;
            UpdateHealsText();
            
            
            if (_health <= 0)
            {
                Kill();
            }
        }

        #endregion
    }
}