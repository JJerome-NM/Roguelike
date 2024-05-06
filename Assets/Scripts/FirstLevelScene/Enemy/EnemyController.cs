using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float startHealth = 100;
        
        private float _health = 100;

        private void Awake()
        {
            _health = startHealth;
        }

        public void Damage(float damage)
        {
            _health -= damage;

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
    }
}