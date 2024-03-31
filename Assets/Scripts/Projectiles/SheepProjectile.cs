using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepProjectile : Projectile
{
    [SerializeField] private SheepMovement _sheepMovement;
    [SerializeField] private Timer _jumpTimer;
    [SerializeField] private Timer _explodeTimer;
    [SerializeField] private float _explodeDelay = 10;
    [SerializeField] private float _jumpFreaquency = 1;

    public override void OnShot()
    {
        _sheepMovement.Reset();
        _sheepMovement.TryMove(Velocity.x / Mathf.Abs(Velocity.x));
        ResetVelocity();
        _explodeTimer.StartTimer(_explodeDelay, Explode);
        _jumpTimer.StartTimer(_jumpFreaquency, JumpAndDelayedJump);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Explode();
    }

    private void JumpAndDelayedJump()
    {
        _sheepMovement.LongJump();
        _jumpTimer.StartTimer(_jumpFreaquency, JumpAndDelayedJump);
    }
}
