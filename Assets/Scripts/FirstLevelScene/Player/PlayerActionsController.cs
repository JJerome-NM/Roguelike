using System;
using DefaultNamespace;
using Enemy;
using FirstLevelScene;
using FirstLevelScene.Game;
using FirstLevelScene.Obstacle;
using FirstLevelScene.Player;
using InteractObjects;
using Obstacle;
using Photon.Pun;
using TMPro;
using UIController;
using UIController.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(PhotonView), typeof(PlayerMovement))]
    public class PlayerActionsController : MonoBehaviour
    {
        private static readonly string HealthText = "HP - ";

        [SerializeField] private float damage = 25;
        [SerializeField] private float maxHitDistance = 2;
        [SerializeField] private float maxInteractionDistance = 2;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask interactable;
        [SerializeField] private PlayerInventory _inventory;
        [SerializeField] private float startHeals = 100;
        [SerializeField] private TextMeshPro nickname;
        [SerializeField] private TextMeshPro health;

        [Header("Other")] 
        [SerializeField] private NotificationController _notification;

        private PlayerMovement _playerMovement;
        private bool _performsAction = false;
        private float _health = 100;
        private PhotonView _photonView;
        
        private Vector3 _startPosition;
        private LayerMask _startLayer;
        private string _startLayerName;

        private int _ownerId = -1;
        
        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _playerMovement = GetComponent<PlayerMovement>();
            _startPosition = transform.position;
            _startLayer = gameObject.layer;
            _startLayerName = LayerMask.LayerToName(_startLayer);
            _health = startHeals;
            _ownerId = _photonView.Owner.ActorNumber;

            if (nickname != null)
            {
                nickname.SetText(_photonView.IsMine ? "You" : _photonView.Owner.NickName);
            }

            UpdateHealthText();
            
            PlayerEventManager.OnPlayerHealsUpdated(_health);
            
            GlobalEventManager.OnGameStarted.AddListener(OnGameStarted);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
        }

        private void UpdateHealthText()
        {
            if (health != null)
            {
                health.SetText(HealthText + _health.ToString("F0"));
            }
        }
        
        private void OnGameStarted(GameStartState state)
        {
            if (state != GameStartState.Resume)
            {
                SetLayerToCharacter();
                gameObject.layer = _startLayer;
                transform.position = _startPosition;
                
                _health = startHeals;
                PlayerEventManager.OnPlayerHealsUpdated(_health);
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
            _health = startHeals;
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
                || UiInventoryController.IsInventoryOpen
                || !_photonView.IsMine) return;

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

                enemyController.Damage(damage);
            }
            NotificationEventManager.ShowNotification("Hit");
        }

        private void DestroyTile(Collider2D c, Vector3 cursorPosition)
        {
            var fieldController = c.GetComponentInParent<ObstacleFieldController>();

            fieldController.DestroyTile(cursorPosition);

            GlobalEventManager.UpdateObstacleFields();
        }

        private void Interact()
        {
            var cursorPosition = GetCursorWorldPosition();
            var distanceToCursor = Vector3.Distance(transform.position, cursorPosition);

            if (distanceToCursor > maxInteractionDistance) return;

            var c = Physics2D.OverlapCircle(cursorPosition, 0.01f, interactable);

            if (c == null) return;

            if (c.gameObject.CompareTag("Rune"))
            {
                var interactObj = c.gameObject.GetComponent<RuneController>();
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
            if (attacker.gameObject.GetComponent<EnemyAi>() != null && _photonView.IsMine)
            {
                _health -= damage;
                PlayerEventManager.OnPlayerHealsUpdated(_health);
                UpdateHealthText();
                
                _playerMovement.GetHit();

                _photonView.RPC(nameof(ChangeOtherPlayerHeals), RpcTarget.Others, _photonView.Owner.ActorNumber, _health);
                
                if (_health <= 0)
                {
                    GlobalEventManager.StopGame(GameEndState.PlayerDied);
                }
            }
        }

        #region RPC

        [PunRPC]
        private void ChangeOtherPlayerHeals(int playerId, float newHeals)
        {
            if (playerId == _ownerId)
            {
                _health = newHeals;
                UpdateHealthText();

                if (PhotonNetwork.IsMasterClient)
                {
                    if (_health <= 0)
                    {
                        GlobalEventManager.StopGame(GameEndState.PlayerDied);
                    }
                }
            }
        }

        #endregion
    }
}