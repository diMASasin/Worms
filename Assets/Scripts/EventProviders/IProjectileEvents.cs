using System;
using Projectiles;
using UnityEngine;

namespace EventProviders
{
    public interface IProjectileEvents
    {
        event Action<Projectile, Vector2> ProjectileLaunched;
        event Action<Projectile> ProjectileExploded;
    }
}