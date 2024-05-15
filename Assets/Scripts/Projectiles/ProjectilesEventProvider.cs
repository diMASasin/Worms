using System;
using System.Collections.Generic;
using EventProviders;
using Factories;
using UnityEngine;

namespace Projectiles
{
    [Serializable]
    public class ProjectilesEventProvider : IDisposable, IProjectileEvents
    {
        [SerializeField] private List<ProjectileFactory> _projectileFactories = new();

        public event Action<Projectile, Vector2> ProjectileLaunched;
        public event Action<Projectile> ProjectileExploded;
        
        public ProjectilesEventProvider()
        {
            foreach (var factory in _projectileFactories)
            {
                factory.ProjectileExploded += OnProjectileExploded;
                factory.ProjectileLaunched += OnProjectileLaunched;
            }
        }

        public void Dispose()
        {
            foreach (var factory in _projectileFactories)
            {
                factory.ProjectileExploded -= OnProjectileExploded;
                factory.ProjectileLaunched -= OnProjectileLaunched;
            }
        }

        private void OnProjectileLaunched(Projectile projectile, Vector2 velovity)
        {
            ProjectileLaunched?.Invoke(projectile, velovity);
        }

        private void OnProjectileExploded(Projectile projectile)
        {
            ProjectileExploded?.Invoke(projectile);
        }
    }
}