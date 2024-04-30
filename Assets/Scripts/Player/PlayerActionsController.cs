using System;
using DefaultNamespace;
using Enemy;
using InteractObjects;
using Obstacle;
using UIController;
using UIController.Inventory;
using UnityEngine;

namespace Player
{
    public class PlayerActionsController : MonoBehaviour
    {
        [SerializeField] private float maxHitDistance = 2;
        [SerializeField] private float maxInteractionDistance = 2;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask interactable;
        [SerializeField] private PlayerInventory _inventory;
        [SerializeField] private float startHeals = 100;

        [Header("Other")] 
        [SerializeField] private NotificationController _notification;

        private PlayerMovement _playerMovement;
        private bool _performsAction = false;
        private float _heals = 100;

        private Vector3 _startPosition;
        private LayerMask _startLayer;
        private string _startLayerName;
        
        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _startPosition = transform.position;
            _startLayer = gameObject.layer;
            _startLayerName = LayerMask.LayerToName(_startLayer);
            _heals = startHeals;
            
            PlayerEventManager.OnPlayerHealsUpdated(_heals);
            
            GlobalEventManager.OnGameStarted.AddListener(OnGameStarted);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
        }

        private void OnGameStarted(GameStartState state)
        {
            if (state != GameStartState.Resume)
            {
                SetLayerToCharacter();
                gameObject.layer = _startLayer;
                transform.position = _startPosition;
                
                _heals = startHeals;
                PlayerEventManager.OnPlayerHealsUpdated(_heals);
            }
        }

        private void SetLayerToCharacter()
        {
            SpriteRenderer[] srs = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach ( SpriteRenderer sr in srs)
            {
                sr.sortingLayerName = _startLayerName;
            }
        }
        
        private void OnGameStopped(GameEndState state)
        {
        }

        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _heals = startHeals;
        }

        private void Update()
        {
            if (_performsAction) return;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                CanIDoSomething(Attack);
            }

            if (Input.GetKey(KeyCode.E))
            {
                CanIDoSomething(Interact);
            }
        }

        private void CanIDoSomething(Action action)
        {
            if (_performsAction
                || GlobalStateManager.IsGameStopped
                || UiInventoryController.IsInventoryOpen) return;

            action.Invoke();
        }

        private void Attack()
        {
            var cursorPosition = GetCursorWorldPosition();
            var distanceToCursor = Vector3.Distance(transform.position, GetCursorWorldPosition());

            if (distanceToCursor > maxHitDistance) return;

            _performsAction = true;
            var attackDirection = -(transform.position - cursorPosition).normalized;
            _playerMovement.SetCharacterDirection(attackDirection);

            _playerMovement.Attack(() =>
            {
                var targetCollider = Physics2D.OverlapCircle(cursorPosition, 0.01f, interactable);

                if (targetCollider != null)
                {
                    switch (targetCollider.tag)
                    {
                        case "Enemy":
                            AttackEnemy(targetCollider);
                            break;
                        case "ObstacleField":
                            DestroyTile(targetCollider, cursorPosition);
                            break;
                    }
                }

                _performsAction = false;
            });
        }

        private void AttackEnemy(Collider2D enemy)
        {
            if (enemy != null)
            {
                var enemyController = enemy.gameObject.GetComponent<EnemyController>();

                enemyController.Damage(100);
            }

            NotificationEventManager.ShowNotification("Hit");
        }

        private void DestroyTile(Collider2D collider, Vector3 cursorPosition)
        {
            var fieldController = collider.GetComponentInParent<ObstacleFieldController>();

            fieldController.DestroyTile(cursorPosition);

            GlobalEventManager.UpdateObstacleFields();
        }

        private void Interact()
        {
            var cursorPosition = GetCursorWorldPosition();
            var distanceToCursor = Vector3.Distance(transform.position, GetCursorWorldPosition());

            if (distanceToCursor > maxInteractionDistance) return;

            var collider = Physics2D.OverlapCircle(cursorPosition, 0.01f, interactable);

            if (collider == null) return;

            if (collider.gameObject.CompareTag("Rune"))
            {
                var interactObj = collider.gameObject.GetComponent<RuneController>();
                if (!interactObj.IsCollected())
                {
                    interactObj.Interact();
                    _inventory.CollectRune();

                    NotificationEventManager.ShowNotification("You find the RUNE");
                }
            }
        }

        private Vector3 GetCursorWorldPosition()
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(position.x, position.y, 0);
        }

        public void TakeDamage(GameObject attacker, float damage)
        {
            if (attacker.gameObject.GetComponent<EnemyAi>() != null)
            {
                _heals -= damage;
                PlayerEventManager.OnPlayerHealsUpdated(_heals);
                
                _playerMovement.GetHit();
                
                if (_heals <= 0)
                {
                    GlobalEventManager.StopGame(GameEndState.PlayerDied);
                }
            }
        }
    }
}