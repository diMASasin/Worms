using System;
using UnityEngine;

namespace Projectiles
{
    public interface IProjectileEvents
    {
        event Action<Projectile> Exploded;
        event Action<Projectile, Vector2> Launched;
    }
}