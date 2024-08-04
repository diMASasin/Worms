using System;
using System.Collections.Generic;
using Configs;
using Projectiles;
using UnityEngine;
using static UnityEngine.Object;

namespace Factories
{
    public class ProjectileFactory : IDisposable
    {
        private readonly ProjectileConfig _config;
        private readonly List<Projectile> _projectiles = new();
        private readonly Transform _projectileParent;
        private AllProjectilesEvents _allProjectileEventsProvider;
        
        private Projectile ProjectilePrefab => _config.ProjectilePrefab;
        public ProjectileConfig Config => _config;

        public event Action<Projectile, Vector2> Launched;
        public event Action<Projectile> Exploded;

        public ProjectileFactory(ProjectileConfig config, Transform projectileParent)
        {
            _config = config;
            _projectileParent = projectileParent;
        }

        public void Dispose()
        {
            foreach (var projectile in _projectiles)
            {
                projectile.Launched -= OnLaunched;
                projectile.Exploded -= OnExploded;
            }
        }

        public Projectile Create()
        {
            Projectile projectile = Instantiate(ProjectilePrefab, _projectileParent);

            projectile.Init(_config);
            _projectiles.Add(projectile);

            projectile.Launched += OnLaunched;
            projectile.Exploded += OnExploded;

            return projectile;
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            Launched?.Invoke(projectile, velocity);
        }

        private void OnExploded(Projectile projectile)
        {
            Exploded?.Invoke(projectile);
        }
    }
}