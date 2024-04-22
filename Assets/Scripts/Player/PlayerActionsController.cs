using Enemy;
using UnityEngine;

namespace Player
{
    public class PlayerActionsController : MonoBehaviour
    {
        [SerializeField] private float maxHitDistance = 2;
        [SerializeField] private LayerMask enemyLayer;
        
        private PlayerMovement _playerMovement;
        private bool _performsAction = false;
        
        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (_performsAction) return;
            
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Attack();
            }
        }

        private void Attack()
        {
            if (_performsAction) return;
            
            var cursorPosition = GetCursorWorldPosition();
            var distanceToCursor = Vector3.Distance(transform.position, GetCursorWorldPosition());
            
            if (distanceToCursor > maxHitDistance) return;

            _performsAction = true;
            var attackDirection = -(transform.position - cursorPosition).normalized;
            _playerMovement.SetCharacterDirection(attackDirection);
            _playerMovement.Attack(() =>
            {
                var hit = Physics2D.Raycast(transform.position, attackDirection, distanceToCursor, enemyLayer);

                if (hit.collider != null)
                {
                    var enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
                    
                    enemyController.Damage(100);
                }
                
                _performsAction = false;
            });
        }
        
        private Vector3 GetCursorWorldPosition()
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(position.x, position.y, 0);
        }
    }
}