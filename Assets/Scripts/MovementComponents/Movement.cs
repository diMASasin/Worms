using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using Infrastructure;
using UnityEngine;
using UnityEngine.Events;
using WormComponents;

namespace MovementComponents
{
    public class Movement : MonoBehaviour, IMovement
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private MovementConfig _config;
        [field: SerializeField] protected Collider2D Collider { get; private set; }
        [field: SerializeField] protected GroundChecker GroundChecker { get; private set; }
        [field: SerializeField] protected Transform Armature { get; private set; }
        
        private float Speed => _config.Speed;
        private Vector2 LongJumpForce => _config.LongJumpForce;
        private Vector2 HighJumpForce => _config.HighJumpForce;
        private float JumpCooldown => _config.JumpCooldown;
        private float MinGroundNormalY => _config.MinGroundNormalY;
        private float GravityModifier => _config.GravityModifier;
        protected LayerMask LayerMask => _config.ContactFilter.layerMask;
        
        private float _maxVelocityX;
        private Vector2 _velocity;
        private Vector2 _groundNormal;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
        private readonly List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);

        private const float MinMoveDistance = 0.001f;
        private const float ShellRadius = 0.01f;

        private bool _canJump = true;
        private float _jumpVelocityX;
        private bool _inJump = false;
        private Vector2 _moveAlongGround;
        private Vector2 _move;

        protected float Horizontal;

        public event UnityAction<bool> IsWalkingChanged;
        public event Action MoveDircetionChanged;

        private void Start() => _maxVelocityX = _config.Speed;

        public virtual void Reset()
        {
            ResetVelocity();
            Horizontal = 0;
            IsWalkingChanged?.Invoke(false);
        }

        public void ResetVelocity()
        {
            _velocity = Vector2.zero;
        }

        public virtual void FixedUpdate()
        {
            if (Horizontal != 0)
                Armature.transform.right = new Vector3(Horizontal, 0);

            _velocity += Physics2D.gravity * (GravityModifier * Time.deltaTime);
            if (_inJump)
            {
                _jumpVelocityX += Horizontal * _maxVelocityX * Time.deltaTime;
                _velocity.x = _jumpVelocityX;
            }
            else
            {
                _velocity.x = Horizontal * Speed + _jumpVelocityX;
            }
            _velocity.x = Mathf.Clamp(_velocity.x, -_maxVelocityX, _maxVelocityX);

            Vector2 deltaPosition = _velocity * Time.deltaTime;
            _moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);
            _move = _moveAlongGround * deltaPosition.x;

            Move(_move, false);

            _move = Vector2.up * deltaPosition.y;

            Move(_move, true);

            IsWalkingChanged?.Invoke(Horizontal != 0);
        }

        public void TryMove(float horizontal)
        {
            Horizontal = horizontal;
            MoveDircetionChanged?.Invoke();
        }

        private void Move(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;

            if (distance > MinMoveDistance)
            {
                int count = _rigidbody.Cast(move, _config.ContactFilter, _hitBuffer, distance + ShellRadius);

                _hitBufferList.Clear();

                for (int i = 0; i < count; i++)
                {
                    _hitBufferList.Add(_hitBuffer[i]);
                }

                for (int i = 0; i < _hitBufferList.Count; i++)
                {
                    Vector2 currentNormal = _hitBufferList[i].normal;
                    if (currentNormal.y > MinGroundNormalY)
                    {
                        if (yMovement)
                        {
                            _groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    float projection = Vector2.Dot(_velocity, currentNormal);
                    if (projection < 0)
                    {
                        _velocity = _velocity - projection * currentNormal;
                    }

                    float modifiedDistance = _hitBufferList[i].distance - ShellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            _rigidbody.position += move.normalized * distance;
        }

        public void TurnRight()
        {
            Armature.transform.right = new Vector3(-1, 0);
        }

        public void TurnLeft()
        {
            Armature.transform.right = new Vector3(1, 0);
        }

        private bool TryJump()
        {
            if (!GroundChecker.IsGrounded || !_canJump)
                return false;

            _canJump = false;
            _inJump = true;
            CoroutinePerformer.StartCoroutine(ReloadJump(JumpCooldown));

            return true;
        }

        public void LongJump()
        {
            if (!TryJump())
                return;
            _jumpVelocityX += LongJumpForce.x * Armature.transform.right.x;
            _velocity.y = LongJumpForce.y;
            _maxVelocityX = Mathf.Abs(_jumpVelocityX);
            CoroutinePerformer.StartCoroutine(StopJump());
        }

        public void HighJump()
        {
            if (!TryJump())
                return;
            _jumpVelocityX += HighJumpForce.x * -Armature.transform.right.x;
            _velocity.y = HighJumpForce.y;
            _maxVelocityX = Mathf.Abs(_jumpVelocityX);
            CoroutinePerformer.StartCoroutine(StopJump());
        }

        private IEnumerator ReloadJump(float duration)
        {
            yield return new WaitForSeconds(duration);
            _canJump = true;
        }

        private IEnumerator StopJump()
        {
            while (GroundChecker.IsGrounded == true)
                yield return null;

            while (GroundChecker.IsGrounded != true)
                yield return null;

            _jumpVelocityX = 0;
            _maxVelocityX = Speed;
            _inJump = false;
        }

        public void AddExplosionForce(float explosionForce, Vector2 explosionPosition, float upwardsModifier = 0.0F)
        {
            var explosionDir = (Vector2)Armature.transform.position - explosionPosition;
            var explosionDistance = explosionDir.magnitude;

            // Normalize without computing magnitude again
            if (upwardsModifier == 0)
            {
                explosionDir /= explosionDistance;
            }
            else
            {
                // From Rigidbody.AddExplosionForce doc:
                // If you pass a non-zero value for the upwardsModifier parameter, the direction
                // will be modified by subtracting that value from the Y component of the centre point.
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }

            _velocity += Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir;
            _jumpVelocityX = _velocity.x;
            CoroutinePerformer.StartCoroutine(StopJump());
        }
    }
}
