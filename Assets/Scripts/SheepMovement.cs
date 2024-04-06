using UnityEngine;

public class SheepMovement : Movement
{
    [SerializeField] private float _jumpInterval = 1;
    
    private Timer _jumpTimer = new();

    private Vector3 _overlapPoint;
    private Vector3 _overlapBoxSize;


    private void Awake()
    {
        _jumpTimer.Start(_jumpInterval);
    }

    private void Update()
    {
        _jumpTimer.Tick();
    }

    private void OnEnable()
    {
        _jumpTimer.Elapsed += JumpAndDelayedJump;
    }

    private void OnDisable()
    {
        _jumpTimer.Elapsed -= JumpAndDelayedJump;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        _overlapPoint = transform.position +
            new Vector3(-_collider.bounds.size.x / 1.9f * _armature.transform.right.x, 0, 0) -
            new Vector3(0.1f * _armature.transform.right.x, 0, 0);
        _overlapBoxSize = new Vector2(0.01f, 0.12f);

        var overlap = Physics2D.OverlapBox(_overlapPoint, _overlapBoxSize, 0, _layerMask);

        if (overlap != null && _groundChecker.IsGrounded)
        {
            Horizontal = -Horizontal;
            ResetVelocity();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(_overlapPoint, _overlapBoxSize);
    }

    private void JumpAndDelayedJump()
    {
        LongJump();
        _jumpTimer.Start(_jumpInterval);
    }
}
