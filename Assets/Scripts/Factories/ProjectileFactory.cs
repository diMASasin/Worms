using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using UI;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Factories
{
    [CreateAssetMenu(fileName = "ProjectileFactory", menuName = "Factories/Projectile")]
    public class ProjectileFactory : ScriptableObject, IProjectileEvents
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private ProjectileConfig _config;
        [SerializeField] private ExplosionPool _explosionPool;
        
        private FollowingTimerViewPool _followingTimerViewPool;
        private readonly List<Projectile> _projectiles = new();
        private Transform _projectileParent;
        private ProjectileConfigurator _configurator;

        public ProjectileConfig Config => _config;

        public event Action<Projectile, Vector2> ProjectileLaunched;
        public event Action<Projectile> ProjectileExploded; 

        public void Init(Transform projectileParent, FollowingTimerViewPool followingTimerViewPool)
        {
            _projectileParent = projectileParent;
            _followingTimerViewPool = followingTimerViewPool;
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
            Projectile projectile = Object.Instantiate(_projectilePrefab, _projectileParent);
            
            projectile.Init(_config);
            _projectiles.Add(projectile);
            
            projectile.Launched += OnLaunched;
            projectile.Exploded += OnExploded;
            
            _configurator = new ProjectileConfigurator(projectile, _config, _followingTimerViewPool);
            _configurator.Configure(_config);
            
            return projectile;
        }

        private void OnLaunched(Projectile projectile, Vector2 velocity)
        {
            ProjectileLaunched?.Invoke(projectile, velocity);
        }

        private void OnExploded(Projectile projectile)
        {
            Explosion explosion = _explosionPool.Get();
            explosion.Explode(_config.ExplosionConfig, projectile.Collider.radius, projectile.transform.position);
            
            ProjectileExploded?.Invoke(projectile);
        }
    }
}