using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Configs;
using UnityEngine;

namespace Projectiles
{
    public interface IProjectile : IProjectileEvents
    {
        ProjectileConfig Config { get; }
        CircleCollider2D Collider { get; }
        Rigidbody2D Rigidbody { get; }
        void Init(ProjectileConfig config);
        void ResetProjectile();
        void InfluenceOnVelocity(Vector2 additionalVelocity);
        void ResetView();
        void Launch(Vector2 velocity);
        void Explode();
    }
}