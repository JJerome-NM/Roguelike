using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private float _health = 100;
        
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
            Debug.Log("Dead");            
        }
    }
}