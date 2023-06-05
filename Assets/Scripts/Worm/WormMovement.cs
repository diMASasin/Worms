using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WormMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _wormArmature;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private WormInput _wormInput;
    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _longJumpForce = new Vector2(2, 2);
    [SerializeField] private Vector2 _highJumpForce = new Vector2(2, 2);
    [SerializeField] private float _jumpCooldown = 0.5f;

    [SerializeField] private float _minGroundNormalY = .65f;
    [SerializeField] private float _gravityModifier = 1f;
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private LayerMask _layerMask;

    //private bool grounded;
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

    public event UnityAction<bool> IsWalkingChanged;

    private void OnEnable()
    {
        _wormInput.InputDisabled += OnInputDisabled;
    }

    private void OnDisable()
    {
        _wormInput.InputDisabled -= OnInputDisabled;
    }

    void Start()
    {
        _contactFilter.useTriggers = false;
        _contactFilter.SetLayerMask(_layerMask);
        _contactFilter.useLayerMask = true;

        _maxVelocityX = _speed;
    }

    private void FixedUpdate()
    {
        if (!_groundChecker.IsGrounded && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            _rigidbody.velocity += (Vector2)Physics.gravity * Time.deltaTime;
        else if (_groundChecker.IsGrounded && _rigidbody.bodyType == RigidbodyType2D.Kinematic)
            _rigidbody.velocity = Vector2.zero;
    }

    private void OnInputDisabled()
    {
        IsWalkingChanged?.Invoke(false);
    }

    public void TryMove(float horizontal)
    {
        if(horizontal != 0)
            _wormArmature.transform.right = new Vector3(-horizontal, 0);

        _velocity += _gravityModifier * Physics2D.gravity * Time.deltaTime;
        if(_inJump)
        {
            _jumpVelocityX += horizontal * _maxVelocityX * Time.deltaTime;
            _velocity.x = _jumpVelocityX;
        }
        else
        {
            _velocity.x = horizontal * _speed + _jumpVelocityX;
        }
        _velocity.x = Mathf.Clamp(_velocity.x, -_maxVelocityX, _maxVelocityX);

        //grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);

        IsWalkingChanged?.Invoke(horizontal != 0);
    }

    void Movement(Vector2 move, bool yMovement)
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
                    //grounded = true;
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
        _jumpVelocityX += _longJumpForce.x * -_wormArmature.transform.right.x;
        _velocity.y = _longJumpForce.y;
        _maxVelocityX = Mathf.Abs(_jumpVelocityX);
        StartCoroutine(StopJump());
    }

    public void HighJump()
    {
       if (!TryJump())
            return;
        _jumpVelocityX += _highJumpForce.x * _wormArmature.transform.right.x;
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
}
