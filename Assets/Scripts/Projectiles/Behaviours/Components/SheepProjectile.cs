using System;
using System.Collections;
using Timers;
using UltimateCC;
using UnityEngine;

namespace Projectiles.Behaviours.Components
{
    public class SheepProjectile : MonoBehaviour, IMovementInput
    {
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private Projectile _projectile;
        [SerializeField] private PlayerMain _playerMain;
        [SerializeField] private float _jumpInterval = 2;
        [SerializeField] private LayerMask _wallDetectorLayerMask;

        private IMovementInput _movementInput;
        private Timer _jumpTimer;
        private Timer _changeDirectionCooldownTimer;
        private Vector3 _overlapPoint;
        private Vector3 _overlapBoxSize;
        private bool _shouldJump;
        private bool _canChangeDirection = true;
        private int _previousDirection;
        private int _currentDirection;

        private bool IsGrounded => _playerMain.PlayerData.Physics.IsGrounded;

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

        public void Init()
        {
        }

        private void OnProjectileReseted() => Reset();

        public void Reset()
        {
            _shouldJump = false;
            _canChangeDirection = true;

            _jumpTimer?.Stop();
            _changeDirectionCooldownTimer?.Stop();

            _inputHandler.Disable();
        }

        private void Update()
        {
            _currentDirection = _playerMain.PlayerData.Physics.FacingDirection;

            if (_currentDirection != _previousDirection)
                OnDirectionChanged();
                
            _previousDirection = _currentDirection;
            WalkPerformed?.Invoke(_playerMain.PlayerData.Physics.FacingDirection);
        }

        private void OnDirectionChanged()
        {
            // TryJumpAfterDelay();
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
            Debug.Log($"TryJumpAfterDelay");
            _jumpTimer.Start(_jumpInterval, () =>
            {
                if (IsGrounded == true)
                {
                    RepeatLongJump();
                }
                else
                {
                    Debug.Log($"Should jump");
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
            Reset();
            gameObject.SetActive(false);
        }

        private void RepeatLongJump()
        {
            Debug.Log($"Jump");
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