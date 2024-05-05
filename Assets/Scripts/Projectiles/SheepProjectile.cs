using Projectiles;
using Timers;
using UnityEngine;

public class SheepProjectile : ILaunchBehaviour
{
    private SheepMovement _sheepMovement;

    public SheepProjectile(SheepMovement sheepMovement)
    {
        _sheepMovement = sheepMovement;
    }

    public void OnLaunch(Vector2 velocity)
    {
        _sheepMovement.Reset();
        _sheepMovement.TryMove(Mathf.Sign(velocity.x));
    }
}
