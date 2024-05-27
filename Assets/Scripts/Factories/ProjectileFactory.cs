using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Factories
{
    public class ProjectileFactory : IProjectileEvents
    {
        private readonly ProjectileConfig _config;
        private readonly List<Projectile> _projectiles = new();
        private readonly Transform _projectileParent;
        private readonly ProjectileConfigurator _configurator;
        private AllProjectilesEvents _allProjectileEventsProvider;
        
        private Projectile ProjectilePrefab => _config.ProjectilePrefab;
        public ProjectileConfig Config => _config;

        public event Action<Projectile, Vector2> Launched;
        public event Action<Projectile> Exploded;

        public ProjectileFactory(ProjectileConfig config, Transform projectileParent, ProjectileConfigurator configurator)
        {
            _config = config;
            _projectileParent = projectileParent;
            _configurator = configurator;
        }

        private void OnDestroy()
        {
            foreach (var projectile in _projectiles)
                projectile.Exploded -= OnExploded;
        }

        public void FixedTick()
        {
            _configurator?.FixedTick();
        }

        public void OnDrawGizmos() => _configurator?.OnDrawGizmos();

        public Projectile Create()
        {
            Projectile projectile = Instantiate(ProjectilePrefab, _projectileParent);

            projectile.Init(_config);
            _projectiles.Add(projectile);

            projectile.Launched += OnLaunched;
            projectile.Exploded += OnExploded;

            _configurator.Configure(projectile, _config);

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