using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using FirstLevelScene;
using FirstLevelScene.Game;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerMovement : MonoBehaviour
    {
        private static readonly string[] LoopedAnimations = { "Run", "Walk", "Stunned" };
        private static readonly string[] SimpleAnimations = { "Attack", "Boost", "Death", "GetHit" };
        private static readonly Dictionary<string, float> AnimationsTime = new ()
        {
            { "Attack", 10f },
            { "Boost", 10f },
            { "Death", 10f },
            { "GetHit", 10f }
        };

        [Header("Settings")] 
        [SerializeField] private float walkSpeed = 1;
        [SerializeField] private float runSpeed = 4;
        
        private Animator _animator;
        private Rigidbody2D _rb;
        private Coroutine _animationCoroutine;
        private PhotonView _photonView;
        
        private float _verticalInput;
        private float _horizontalInput;
        private Vector2 _moveDirection;
        private string _currentAnimation = string.Empty;
        private bool _canMove = true;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();

            GetAnimationsTime();
            
            GlobalEventManager.OnGameStopped.AddListener(OnGameStopped);
        }

        private void OnGameStopped(GameEndState state)
        {
            _moveDirection = new Vector2(0, 0);
            _rb.velocity = _moveDirection;
            
            SetCharacterDirection(_rb.velocity);
            
            if (!string.IsNullOrWhiteSpace(_currentAnimation))
            {
                _animator.SetBool(_currentAnimation, false);
            }
            _currentAnimation = string.Empty;
        }

        private void GetAnimationsTime()
        {
            AnimationClip[] clips = _animator.runtimeAnimatorController.animationClips;
 
            foreach (AnimationClip clip in clips)
            {
                if (AnimationsTime.ContainsKey(clip.name))
                {
                    AnimationsTime[clip.name] = clip.length;
                }
            }
        }
        
        private void Update()
        {
            if (GlobalStateManager.IsGameStopped || !_photonView.IsMine) return;
            
            MovementInput();
            SelectLoopedAnimation();
        }
        
        private void MovementInput()
        {
            if (!_canMove || !_photonView.IsMine)
            {
                _rb.velocity = new Vector2(0, 0);
                return;
            }
            
            _verticalInput = Input.GetAxisRaw("Vertical");
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            
            _moveDirection = new Vector2(_horizontalInput, _verticalInput);
            
            _rb.velocity = _moveDirection.normalized * GetCurrentMaxSpeed();

            if (_horizontalInput != 0 || _verticalInput != 0)
            {
                SetCharacterDirection(_rb.velocity);
            }
        }

        private float GetCurrentMaxSpeed() => Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        public void SetCharacterDirection(Vector2 direction)
        {
            if (!_photonView.IsMine) return;
            _animator.SetFloat("Vertical", direction.y);
            _animator.SetFloat("Horizontal", direction.x);
        }
        
        public void Attack(Action afterAnimationAction) => DoSAnimation("Attack", afterAnimationAction);

        public void GetHit()
        {
            if (!_photonView.IsMine) return;
            if (_currentAnimation != String.Empty) return;
            
            _currentAnimation = "GetHit";
            _animator.Play(_currentAnimation);
            _animationCoroutine = StartCoroutine(ResetSimpleAnimation(_currentAnimation, () => {}));
        }
        
        private void DoSAnimation(string animationName, Action afterAnimationAction)
        {
            if (!_photonView.IsMine) return;
            _canMove = false;
            
            SelectSimpleAnimation(animationName, () =>
            {
                _canMove = true;
                afterAnimationAction();
            });
        }
        
        private void SelectSimpleAnimation(string anim, Action afterAnimationAction)
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
                _animationCoroutine = StartCoroutine(ResetSimpleAnimation(_currentAnimation, afterAnimationAction));
                _photonView.RPC(nameof(UpdateSimpleAnimationInOthers), RpcTarget.Others, anim);
            }
        }

        private IEnumerator ResetSimpleAnimation(string anim, Action afterAnimationAction)
        {
            yield return new WaitForSeconds(AnimationsTime[anim]);

            _currentAnimation = string.Empty;
            afterAnimationAction();
        }        
        
        private void SelectLoopedAnimation()
        {
            if (SimpleAnimations.Contains(_currentAnimation) || !_photonView.IsMine) return;
            
            float magnitude = _rb.velocity.magnitude;

            if (magnitude == 0)
            {
                if (!string.IsNullOrWhiteSpace(_currentAnimation))
                {
                    _animator.SetBool(_currentAnimation, false);
                }
                _currentAnimation = string.Empty;
                _photonView.RPC(nameof(UpdateLoopedAnimationForOther), RpcTarget.Others, _currentAnimation);
                return;
            }

            _currentAnimation = magnitude <= walkSpeed ? "Walk" : "Run";
            if (LoopedAnimations.Contains(_currentAnimation))
            {
                _animator.SetBool(_currentAnimation, true);
                _animator.Play(_currentAnimation);
                
                _photonView.RPC(nameof(UpdateLoopedAnimationForOther), RpcTarget.Others, _currentAnimation);
            }
        }

        #region RPC

        [PunRPC]
        public void UpdateSimpleAnimationInOthers(string animation)
        {
            if (SimpleAnimations.Contains(animation))
            {
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                    _animationCoroutine = null;
                }
                
                _currentAnimation = animation;
                _animator.Play(_currentAnimation);
                _animationCoroutine = StartCoroutine(ResetSimpleAnimation(_currentAnimation, () => {}));
            }
        }
        
        [PunRPC]
        private void UpdateLoopedAnimationForOther(string animation)
        {
            if (LoopedAnimations.Contains(animation))
            {
                _animator.SetBool(animation, true);
                _animator.Play(animation);
            }
        }

        #endregion
    }
}