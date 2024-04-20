using System.Collections;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly string[] LoopedAnimations = { "Run", "Walk", "Stunned" };
        private static readonly string[] SimpleAnimations = { "Attack", "Boost", "Death", "GetHit" };
        
        [Header("Settings")] 
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private Transform orientation;
        
        
        private Animator _animator;
        private Rigidbody2D _rb;
        private Coroutine _animationCoroutine;

        private float _verticalInput;
        private float _horizontalInput;
        private Vector2 _moveDirection;
        private string _currentAnimation = string.Empty;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            MovementInput();
            
            if (Input.GetKey(KeyCode.Mouse0))
            {
                SelectSimpleAnimation("Attack");
            }

            SelectAnimation();
        }

        private void MovementInput()
        {
            _verticalInput = Input.GetAxisRaw("Vertical");
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            
            _moveDirection = new Vector2(_horizontalInput, _verticalInput);
            
            _rb.velocity = _moveDirection * moveSpeed;

            if (_horizontalInput != 0 || _verticalInput != 0)
            {
                _animator.SetFloat("Vertical", _rb.velocity.y);
                _animator.SetFloat("Horizontal", _rb.velocity.x);
            }
        }
        
        private void SelectSimpleAnimation(string anim)
        {
            if (SimpleAnimations.Contains(anim))
            {
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                    _animationCoroutine = null;
                }
                
                _currentAnimation = anim;
                _animator.Play(_currentAnimation);
                _animationCoroutine = StartCoroutine(ResetCurrentSimpleAnimation(_currentAnimation));
            }
        }

        private IEnumerator ResetCurrentSimpleAnimation(string anim)
        {
            while (_animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
            {
                yield return new WaitForSeconds(0.1f);
            }
            _currentAnimation = string.Empty;
        }        
        
        private void SelectAnimation()
        {
            if (SimpleAnimations.Contains(_currentAnimation)) return;
            
            float magnitude = _rb.velocity.magnitude;

            if (magnitude == 0)
            {
                if (!string.IsNullOrWhiteSpace(_currentAnimation))
                {
                    _animator.SetBool(_currentAnimation, false);
                }
                _currentAnimation = string.Empty;
                return;
            }

            _currentAnimation = magnitude < 1 ? "Walk" : "Run";
            _animator.SetBool(_currentAnimation, true);
            _animator.Play(_currentAnimation);
        }

    }
}