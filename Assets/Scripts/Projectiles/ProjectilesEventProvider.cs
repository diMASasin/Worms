using System;
using System.Collections.Generic;
using Factories;
using UnityEngine;

namespace Projectiles
{
    public class AllProjectilesEvents : IDisposable, IProjectileEvents
    {
        private readonly IEnumerable<ProjectileFactory> _projectileFactories;

        public event Action<Projectile> Exploded;
        public event Action<Projectile, Vector2> Launched;
        
        public AllProjectilesEvents(IEnumerable<ProjectileFactory> projectileFactories)
        {
            _projectileFactories = projectileFactories;
            
            foreach (var factory in _projectileFactories)
            {
                factory.Exploded += OnExploded;
                factory.Launched += OnLaunched;
            }
        }

        public void Dispose()
        {
            foreach (var factory in _projectileFactories)
            {
                factory.Exploded -= OnExploded;
                factory.Launched -= OnLaunched;
            }
        }

        private void OnLaunched(Projectile projectile, Vector2 velovity)
        {
            Launched?.Invoke(projectile, velovity);
        }

        private void OnExploded(Projectile projectile)
        {
            Exploded?.Invoke(projectile);
        }
    }
}