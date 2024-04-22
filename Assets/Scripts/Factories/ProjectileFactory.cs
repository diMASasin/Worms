using System.Collections.Generic;
using Configs;
using Pools;
using Projectiles;
using UnityEngine;

namespace Factories
{
    public class ProjectileFactory
    {
        private readonly Wind _wind;
        private readonly Projectile _prefab;
        private readonly Transform _projectilesParent;
        private readonly ExplosionPool _explosionPool;

        public ProjectileFactory(Wind wind, Projectile prefab, Transform projectilesParent, ExplosionPool explosionPool)
        {
            _wind = wind;
            _prefab = prefab;
            _projectilesParent = projectilesParent;
            _explosionPool = explosionPool;
        }

        public Projectile GetProjectile(ProjectileConfig config)
        {
            IProjectileExplodeModifier explodeModifier = null;
            List<IProjectileLauchModifier> lauchModifiers = new();

            Projectile projectile = Object.Instantiate(_prefab, _projectilesParent);

            if (config.RandomizedTorqueForce.StartValue != 0 || config.RandomizedTorqueForce.EndValue != 0)
                lauchModifiers.Add(new ProjectileRotator(config, projectile.Rigidbody2D));
            
            if(config.HasFragments)
                explodeModifier = new FragmentsExplodeModifier(config.FragmentsPool, config.FragmentsAmount, projectile.transform);

            if (config.CanWalk == true)
            {
                var groundChecker = new GroundChecker(projectile.transform, projectile.Collider2D, config.MovementConfig.GroundCheckerConfig);
                var sheepMovement = new SheepMovement(projectile.Rigidbody2D, projectile.Collider2D,
                    projectile.transform, groundChecker, config.MovementConfig);

                SheepProjectile sheepProjectile = new SheepProjectile(sheepMovement);

                lauchModifiers.Add(sheepProjectile);
            }
            projectile.Init(_wind, config, _explosionPool, explodeModifier, lauchModifiers.ToArray());

            return projectile;
        }
    }
}