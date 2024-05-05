using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.Events;

public class Movement
{
    private float _speed => _config.Speed;
    private Vector2 _longJumpForce => _config.LongJumpForce;
    private Vector2 _highJumpForce => _config.HighJumpForce;
    private float _jumpCooldown => _config.JumpCooldown;
    private float _minGroundNormalY => _config.MinGroundNormalY;
    private float _gravityModifier => _config.GravityModifier;
    public LayerMask LayerMask => _config.LayerMask;

    protected Transform Armature;
    protected GroundChecker GroundChecker;

    private Vector2 _velocity;
    private Vector2 _groundNormal;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);

    private const float _minMoveDistance = 0.001f;
    private const float _shellRadius = 0.01f;

    public Rigidbody2D Rigidbody { get; private set; }
    public Collider2D Collider { get; private set; }

    private bool _canJump = true;
    private float _jumpVelocityX;
    private float _maxVelocityX;
    private bool _inJump = false;
    private Vector2 _moveAlongGround;
    private Vector2 _move;
    private MovementConfig _config;

    protected float Horizontal;

    public event UnityAction<bool> IsWalkingChanged;
    public event Action MoveDircetionChanged;
    
    public Movement(Rigidbody2D rigidbody2D, Collider2D collider2D, Transform armature, 
        GroundChecker groundChecker, MovementConfig config)
    {
        Rigidbody = rigidbody2D;
        Collider = collider2D;
        Armature = armature;
        GroundChecker = groundChecker;
        _config = config;

        _maxVelocityX = _config.Speed;

        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(LayerMask);
        _contactFilter.useLayerMask = true;
    }

    public void Reset()
    {
        ResetVelocity();
        Horizontal = 0;
        IsWalkingChanged?.Invoke(false);
    }

    public void ResetVelocity()
    {
        _velocity = Vector2.zero;
    }

    public virtual void FixedTick()
    {
        if (Horizontal != 0)
            Armature.transform.right = new Vector3(-Horizontal, 0);

        _velocity += _gravityModifier * Physics2D.gravity * Time.deltaTime;
        if (_inJump)
        {
            _jumpVelocityX += Horizontal * _maxVelocityX * Time.deltaTime;
            _velocity.x = _jumpVelocityX;
        }
        else
        {
            _velocity.x = Horizontal * _speed + _jumpVelocityX;
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

    void Move(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > _minMoveDistance)
        {
            int count = Rigidbody.Cast(move, _contactFilter, _hitBuffer, distance + _shellRadius);

            _hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }

            for (int i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                if (currentNormal.y > _minGroundNormalY)
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

                float modifiedDistance = _hitBufferList[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        Rigidbody.position = Rigidbody.position + move.normalized * distance;
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
        CoroutinePerformer.StartRoutine(JumpCooldown(_jumpCooldown));

        return true;
    }

    public void LongJump()
    {
        if (!TryJump())
            return;
        _jumpVelocityX += _longJumpForce.x * -Armature.transform.right.x;
        _velocity.y = _longJumpForce.y;
        _maxVelocityX = Mathf.Abs(_jumpVelocityX);
        CoroutinePerformer.StartRoutine(StopJump());
    }

    public void HighJump()
    {
        if (!TryJump())
            return;
        _jumpVelocityX += _highJumpForce.x * Armature.transform.right.x;
        _velocity.y = _highJumpForce.y;
        _maxVelocityX = Mathf.Abs(_jumpVelocityX);
        CoroutinePerformer.StartRoutine(StopJump());
    }

    private IEnumerator JumpCooldown(float duration)
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
        _maxVelocityX = _speed;
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
        CoroutinePerformer.StartRoutine(StopJump());
    }
}
