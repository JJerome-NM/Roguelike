using System.Collections;
using System.Linq;
using DefaultNamespace;
using FirstLevelScene;
using Levels;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAi : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayer;
        
        [Header("Search")] 
        [SerializeField] private float searchDistance;
        [SerializeField] private float chaseDistance;
        [SerializeField] private float searchDelay;
        [SerializeField] private float chaseDelay;
        
        [Header("Interaction")] 
        [SerializeField] private float attackDistance = 1.5f;
        [SerializeField] private float hitDistance = 2f;
        [SerializeField] private float damage = 25;

        [Header("Animation")] 
        [SerializeField] private float updateAnimationDelay = 0.05f;

        private NavMeshAgent _navAgent;

        private Coroutine _searchCoroutine;
        private Coroutine _chaseCoroutine;
        private Coroutine _updateAnimationCoroutine;
        private SkeletonAnimationController _animation;
        private Vector3 _startPosition;
        private GameObject _player;
        private PlayerActionsController _playerAC;

        private void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();
            _animation = GetComponent<SkeletonAnimationController>();
            _startPosition = transform.position;

            GlobalEventManager.OnGameStarted.AddListener(OnGameStart);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
            LevelsEventManager.OnLevelMultiplayerUpdated.AddListener((multiplayer) => { damage += multiplayer; });
        }

        private void OnGameStart(GameStartState state)
        {
            StartSearch();
            StartUpdateAnimation();
        }

        private void OnGameStopped(GameEndState state)
        {
            StopChase();
            StopSearch();

            if (state != GameEndState.Pause)
            {
                _navAgent.SetDestination(_startPosition);
                transform.position = _startPosition;
            }
            else
            {
                _navAgent.SetDestination(transform.position);
            }
        }

        private IEnumerator UpdateAnimations()
        {
            while (true)
            {
                if (_navAgent.velocity.magnitude == 0)
                {
                    StopUpdateAnimation();
                }

                SetAnimationMoveDirection();
                yield return new WaitForSeconds(updateAnimationDelay);
            }
        }

        private IEnumerator SearchPlayer()
        {
            while (true)
            {
                if (_navAgent.velocity.magnitude != 0)
                {
                    StartUpdateAnimation();
                }

                var target = Physics2D.OverlapCircleAll(transform.position, searchDistance)
                    .ToList()
                    .Find(t => t.CompareTag("Player"));

                if (target != null)
                {
                    _player = target.gameObject;
                    _playerAC = _player.GetComponent<PlayerActionsController>();
                    
                    StopSearch();
                    StartChase();
                }
                yield return new WaitForSeconds(searchDelay);
            }
        }

        private IEnumerator ChasePlayer()
        {
            while (true)
            {
                if (_animation.canMove)
                {
                    var distanceTopPlayer = Vector3.Distance(transform.position, _player.transform.position);

                    if (distanceTopPlayer < chaseDistance)
                    {
                        if (distanceTopPlayer < attackDistance)
                        {
                            AttackPlayer();
                        }
                        else
                        {
                            _navAgent.SetDestination(_player.transform.position);
                            StartUpdateAnimation();
                        }
                    }
                    else
                    {
                        _navAgent.SetDestination(_startPosition);
                        StopChase();
                        StartSearch();
                    }
                }

                yield return new WaitForSeconds(chaseDelay);
            }
        }

        private void AttackPlayer()
        {
            StopUpdateAnimation();
            SetPlayerDirection();

            _navAgent.SetDestination(transform.position);
            _animation.Attack(() =>
            {
                var distanceTopPlayer = Vector3.Distance(transform.position, _player.transform.position);

                if (distanceTopPlayer < hitDistance)
                {
                    SetPlayerDirection();
                    _playerAC.TakeDamage(gameObject, damage);
                }
            });
        }

        private void SetAnimationMoveDirection()
        {
            _animation.SetCharacterDirection(GetMoveDirection(transform.position, _navAgent.steeringTarget));
        }

        private void SetPlayerDirection()
        {
            _animation.SetCharacterDirection(GetMoveDirection(transform.position, _player.transform.position));
        }

        private void StartUpdateAnimation() => _updateAnimationCoroutine ??= StartCoroutine(UpdateAnimations());

        private void StopUpdateAnimation() => StopMyCoroutine(ref _updateAnimationCoroutine);

        private void StartChase() => _chaseCoroutine ??= StartCoroutine(ChasePlayer());

        private void StopChase() => StopMyCoroutine(ref _chaseCoroutine);

        private void StartSearch() => _searchCoroutine ??= StartCoroutine(SearchPlayer());

        private void StopSearch() => StopMyCoroutine(ref _searchCoroutine);

        private void StopMyCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        private Vector2 GetMoveDirection(Vector3 firstPosition, Vector3 secondPosition)
        {
            return -(firstPosition - secondPosition).normalized;
        }
    }
}