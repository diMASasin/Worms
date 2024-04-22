using Configs;
using System;
using Timers;
using UnityEngine;

public class SheepMovement : Movement, IDisposable
{
    private readonly float _jumpInterval;
    private readonly Timer _jumpTimer = new();

    private Vector3 _overlapPoint;
    private Vector3 _overlapBoxSize;

    public SheepMovement(Rigidbody2D rigidbody2D, Collider2D collider2D, Transform armature,
        GroundChecker groundChecker, MovementConfig config, float jumpInterval = 1) : 
        base(rigidbody2D, collider2D, armature, groundChecker, config)
    {
        _jumpInterval = jumpInterval;

        MoveDircetionChanged += OnMoveDircetionChanged;
    }

    public void Dispose()
    {
        MoveDircetionChanged -= OnMoveDircetionChanged;
    }

    public void Tick()
    {
        _jumpTimer.Tick();
    }

    public override void FixedTick()
    {
        base.FixedTick();

        var right = Armature.transform.right;

        _overlapPoint = Armature.transform.position +
                        new Vector3(-Collider.bounds.size.x / 1.9f * right.x, 0, 0) -
                        new Vector3(0.1f * right.x, 0, 0);
        _overlapBoxSize = new Vector2(0.01f, 0.12f);

        var overlap = Physics2D.OverlapBox(_overlapPoint, _overlapBoxSize, 0, LayerMask);

        if (overlap != null && GroundChecker.IsGrounded)
        {
            Horizontal = -Horizontal;
            ResetVelocity();
        }
    }

    private void OnMoveDircetionChanged()
    {
        _jumpTimer.Start(_jumpInterval, RepeatLongJump);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(_overlapPoint, _overlapBoxSize);
    }

    private void RepeatLongJump()
    {
        LongJump();
        _jumpTimer.Start(_jumpInterval, RepeatLongJump);
    }
}
