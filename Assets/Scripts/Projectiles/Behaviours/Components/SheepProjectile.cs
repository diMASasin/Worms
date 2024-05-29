using MovementComponents;
using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class SheepProjectile : Projectile
    {
        [SerializeField] private SheepMovement _sheepMovement;

        public override void Launch(Vector2 velocity)
        {
            base.Launch(velocity);
            _sheepMovement.Reset();
            _sheepMovement.TryMove(Mathf.Sign(velocity.x));
        }
    }
}
