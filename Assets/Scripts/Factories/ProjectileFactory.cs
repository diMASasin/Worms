using System;
using System.Collections.Generic;
using Configs;
using Projectiles;
using Projectiles.Behaviours.Components;
using Projectiles.Behaviours.LaunchBehaviour;
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
            Debug.Log($"{GetType().Name}");

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

            if (projectile.TryGetComponent(out SheepProjectile sheepProjectile) == true)
                sheepProjectile.Init();

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