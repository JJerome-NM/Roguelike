using System.Collections;
using DefaultNamespace;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAi : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private LayerMask playerLayer;

        [Header("Search")] 
        [SerializeField] private float searchDistance;
        [SerializeField] private float chaseDistance;
        [SerializeField] private float searchDelay;
        [SerializeField] private float chaseDelay;

        [Header("Interaction")] 
        [SerializeField] private float hitDistance;
        [SerializeField] private float demage;

        [Header("Animation")] 
        [SerializeField] private float updateAnimationDelay = 0.05f;
        
        private PlayerActionsController _playerAC;
        private NavMeshAgent _navAgent;

        private Coroutine _searchCoroutine;
        private Coroutine _chaseCoroutine;
        private Coroutine _updateAnimationCoroutine;
        private SkeletonAnimationController _animation;
        private Vector3 _startPosition;

        private void Awake()
        {
            _playerAC = player.GetComponent<PlayerActionsController>();
            _navAgent = GetComponent<NavMeshAgent>();
            _animation = GetComponent<SkeletonAnimationController>();
            _startPosition = transform.position;

            GlobalEventManager.OnGameStarted.AddListener(OnGameStart);
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
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
                Debug.Log("dfdfdf");
                _navAgent.SetDestination(_startPosition);
                // transform.position = _startPosition;
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
                Debug.Log("Search");
                
                if (_navAgent.velocity.magnitude != 0)
                {
                    StartUpdateAnimation();
                }
                
                var distanceTopPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceTopPlayer < searchDistance)
                {
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
                Debug.Log("Chase");
                var distanceTopPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceTopPlayer < chaseDistance)
                {
                    _navAgent.SetDestination(player.transform.position);
                    StartUpdateAnimation();

                    if (distanceTopPlayer < hitDistance)
                    {
                        _playerAC.TakeDamage(gameObject, demage);
                    }
                }
                else
                {
                    _navAgent.SetDestination(_startPosition);
                    StopChase();
                    StartSearch();
                }

                yield return new WaitForSeconds(chaseDelay);
            }
        }

        private void SetAnimationMoveDirection()
        {
            _animation.SetCharacterDirection(GetMoveDirection(transform.position, _navAgent.steeringTarget)); 
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