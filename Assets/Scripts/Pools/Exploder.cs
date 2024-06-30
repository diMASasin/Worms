using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Projectiles;

namespace Pools
{
    public class Exploder : IDisposable
    {
        private ExplosionPool _explosionPool;
        private readonly IProjectileEvents _projectileEvents;
        private readonly Dictionary<ExplosionConfig, ExplosionPool> _poolsDictionary;
        
        public Exploder(List<ExplosionPool> explosionPools, IProjectileEvents projectileEvents)
        {
            _projectileEvents = projectileEvents;

            _poolsDictionary = explosionPools.ToDictionary(pool => pool.Config);
            
            _projectileEvents.Exploded += OnExploded;
        }

        public void Dispose()
        {
            _projectileEvents.Exploded -= OnExploded;
        }

        private void OnExploded(Projectile projectile)
        {
            ExplosionConfig explosionConfig = projectile.Config.ExplosionConfig;
            
            if(_poolsDictionary.ContainsKey(explosionConfig) == false)
                return;
            
            Explosion explosion = _poolsDictionary[explosionConfig].Get();
            explosion.Explode(explosionConfig, projectile.transform.position, projectile.MaxDamage, projectile.Collider);
        }
    }
}