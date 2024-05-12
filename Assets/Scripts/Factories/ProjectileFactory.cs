using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
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
        [SerializeField] private FollowingObject _followingTimerView;
        
        private ObjectPool<FollowingObject> _followingTimerViewPool;
        private readonly List<Projectile> _projectiles = new();
        
        public ProjectileConfig Config => _config;

        public event Action<Projectile, Vector2> ProjectileLaunched;
        public event Action<Projectile> ProjectileExploded; 

        private void Awake()
        {
            _followingTimerViewPool = new ObjectPool<FollowingObject>(() => Instantiate(_followingTimerView));
        }

        private void OnDestroy()
        {
            foreach (var projectile in _projectiles)
                projectile.Exploded -= OnExploded;
        }

        public Projectile Create()
        {
            Projectile projectile = Object.Instantiate(_projectilePrefab);
            Explosion explosion = _explosionPool.Get();
            
            projectile.Init(_config);
            _projectiles.Add(projectile);
            
            projectile.Launched += OnLaunched;
            projectile.Exploded += OnExploded;
            
            new ProjectileConfigurator(_projectilePrefab, _config, _followingTimerViewPool).Configure(_config);
            
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