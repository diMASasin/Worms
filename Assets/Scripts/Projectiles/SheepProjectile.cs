using Projectiles;
using Timers;
using UnityEngine;

public class SheepProjectile : IProjectileLauchModifier
{
    private SheepMovement _sheepMovement;

    public SheepProjectile(SheepMovement sheepMovement)
    {
        _sheepMovement = sheepMovement;
    }

    public void OnLaunch(Vector2 velocity)
    {
        _sheepMovement.Reset();
        _sheepMovement.TryMove(velocity.x / Mathf.Abs(velocity.x));
    }
}
