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

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    private bool _canJump = true;

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
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(_layerMask);
        contactFilter.useLayerMask = true;
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
        _velocity.x = horizontal * _speed;

        grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);

        IsWalkingChanged?.Invoke(horizontal != 0);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = _rigidbody.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity = _velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
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
        StartCoroutine(JumpCooldown(_jumpCooldown));

        return true;
    }

    public void LongJump()
    {
        if (!TryJump())
            return;
        _velocity += new Vector2(_longJumpForce.x * -_wormArmature.transform.right.x, _longJumpForce.y);
    }

    public void HighJump()
    {
       if (!TryJump())
            return;
        _velocity += new Vector2(_highJumpForce.x * _wormArmature.transform.right.x, _highJumpForce.y);
    }

    private IEnumerator JumpCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        _canJump = true;
    }
}
