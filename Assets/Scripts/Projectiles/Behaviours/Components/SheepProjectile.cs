using System;
using System.Collections;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System;
using _2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Input_System.InputManager;
using Infrastructure.Interfaces;
using Timers;
using UnityEngine;

namespace Projectiles.Behaviours.Components
{
    public class SheepProjectile : MonoBehaviour, IMovementInput, ICoroutinePerformer
    {
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private Projectile _projectile;
        [SerializeField] private PlayerMain _playerMain;
        [SerializeField] private float _jumpInterval = 2;
        [SerializeField] private LayerMask _wallDetectorLayerMask;

        private IMovementInput _movementInput;
        private ITimer _jumpTimer;
        private ITimer _changeDirectionCooldownTimer;
        private Vector3 _overlapPoint;
        private Vector3 _overlapBoxSize;
        private bool _shouldJump;
        private bool _canChangeDirection = true;
        private int _previousDirection;
        private int _currentDirection;

        private bool IsGrounded => _playerMain.PlayerData.Physics.IsGrounded;

        private void Awake()
        {
            _jumpTimer = new UniTaskTimer();
            _changeDirectionCooldownTimer = new UniTaskTimer();
        }

        private void OnEnable()
        {
            _projectile.Launched += OnLaunched;
            _projectile.Exploded += OnExploded;
        }

        private void OnDisable()
        {
            _projectile.Launched -= OnLaunched;
            _projectile.Exploded -= OnExploded;
        }

        public void ResetSheep()
        {
            _shouldJump = false;
            _canChangeDirection = true;

            _jumpTimer?.Stop();
            _changeDirectionCooldownTimer?.Stop();

            _inputHandler.Disable();
        }

        private void Update()
        {
            if(_inputHandler.PlayerData.Physics.IsGrounded == true)
                WalkPerformed?.Invoke(_playerMain.PlayerData.Physics.FacingDirection);
        }

        public void FixedUpdate()
        {
            Vector3 right = -transform.right;

            _overlapPoint = transform.position +
                            new Vector3(-_projectile.Collider.bounds.size.x / 3f * right.x, 0, 0) -
                            new Vector3(0.1f * right.x, 0, 0);
            _overlapBoxSize = new Vector2(0.5f, 0.1f);

            Collider2D overlap = Physics2D.OverlapBox(_overlapPoint, _overlapBoxSize, 0, (int)_wallDetectorLayerMask);

            if (overlap != null && IsGrounded == true && _canChangeDirection == true)
            {
                WalkPerformed?.Invoke(-_playerMain.PlayerData.Physics.FacingDirection);
                _canChangeDirection = false;
                _changeDirectionCooldownTimer.Start(0.5f, () => _canChangeDirection = true);
            }
        }

        private void TryJumpAfterDelay()
        { 
            _jumpTimer.Start(_jumpInterval, () =>
            {
                if (IsGrounded == true)
                {
                    RepeatLongJump();
                }
                else
                {
                    _shouldJump = true;
                    
                    if(gameObject.activeInHierarchy == true)
                        StartCoroutine(JumpWhenGrounded());
                }
            });
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawCube(_overlapPoint, _overlapBoxSize);
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            _inputHandler.Enable(this);
            TryJumpAfterDelay();
        }

        private void OnExploded(Projectile projectile)
        {
            ResetSheep();
            gameObject.SetActive(false);
        }

        private void RepeatLongJump()
        {
            LongJumpStarted?.Invoke(true);
            _jumpTimer.Start(_jumpInterval, TryJumpAfterDelay);
        }

        private IEnumerator JumpWhenGrounded()
        {
            while (IsGrounded == false || _shouldJump == false)
                yield return null;
            
            RepeatLongJump();
        }

        public void Enable()
        {
        }

        public void Disable()
        {
        }

        public event Action<bool> LongJumpStarted;

        public event Action<bool> HighJumpStarted;

        public event Action<float> WalkPerformed;

        public event Action<bool> DashPerformed;

        public event Action<bool> CrouchPerformed;

        public event Action<bool> WallGrabPerformed;
        public event Action<float> WallClimbPerformed;
    }
}