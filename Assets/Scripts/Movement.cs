using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _longJumpForce = new(2, 2);
    [SerializeField] private Vector2 _highJumpForce = new(2, 2);
    [SerializeField] private float _jumpCooldown = 0.5f;
    [SerializeField] private float _minGroundNormalY = .65f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private Vector2 _velocity;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField] protected Transform _armature;
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected GroundChecker _groundChecker;

    private Vector2 _groundNormal;
    private ContactFilter2D _contactFilter;
    private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    private List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);

    private const float _minMoveDistance = 0.001f;
    private const float _shellRadius = 0.01f;

    private bool _canJump = true;
    private float _jumpVelocityX;
    private float _maxVelocityX;
    private bool _inJump = false;
    private Vector2 _moveAlongGround;
    private Vector2 _move;

    protected float Horizontal;

    public event UnityAction<bool> IsWalkingChanged;

    void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(_layerMask);
        _contactFilter.useLayerMask = true;

        _maxVelocityX = _speed;
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

    protected virtual void FixedUpdate()
    {
        if (Horizontal != 0)
            _armature.transform.right = new Vector3(-Horizontal, 0);

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
    }

    void Move(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > _minMoveDistance)
        {
            int count = _rigidbody.Cast(move, _contactFilter, _hitBuffer, distance + _shellRadius);

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
        _rigidbody.position = _rigidbody.position + move.normalized * distance;
    }

    public void TurnRight()
    {
        _armature.transform.right = new Vector3(-1, 0);
    }

    public void TurnLeft()
    {
        _armature.transform.right = new Vector3(1, 0);
    }

    private bool TryJump()
    {
        if (!_groundChecker.IsGrounded || !_canJump)
            return false;

        _canJump = false;
        _inJump = true;
        StartCoroutine(JumpCooldown(_jumpCooldown));

        return true;
    }

    public void LongJump()
    {
        if (!TryJump())
            return;
        _jumpVelocityX += _longJumpForce.x * -_armature.transform.right.x;
        _velocity.y = _longJumpForce.y;
        _maxVelocityX = Mathf.Abs(_jumpVelocityX);
        StartCoroutine(StopJump());
    }

    public void HighJump()
    {
        if (!TryJump())
            return;
        _jumpVelocityX += _highJumpForce.x * _armature.transform.right.x;
        _velocity.y = _highJumpForce.y;
        _maxVelocityX = Mathf.Abs(_jumpVelocityX);
        StartCoroutine(StopJump());
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        _canJump = true;
    }

    private IEnumerator StopJump()
    {
        while (_groundChecker.IsGrounded == true)
            yield return null;

        while (_groundChecker.IsGrounded != true)
            yield return null;

        _jumpVelocityX = 0;
        _maxVelocityX = _speed;
        _inJump = false;
    }

    public void AddExplosionForce(float explosionForce, Vector2 explosionPosition, float upwardsModifier = 0.0F)
    {
        var explosionDir = (Vector2)transform.position - explosionPosition;
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
        StartCoroutine(StopJump());
    }
}
