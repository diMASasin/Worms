using System;
using Configs;
using Factories;
using Projectiles;
using UnityEngine.Pool;

namespace Pools
{
    public class ProjectilePool : IDisposable, IProjectilePool, IProjectilesCount
    {
        private readonly int _amount;
        private readonly ObjectPool<Projectile> _pool;
        private readonly ProjectileFactory _projectileFactory;

        public ProjectileConfig Config => _projectileFactory.Config;
        
        public static int Count { get; private set; }

        public static event Action<int> CountChanged;

        public ProjectilePool(ProjectileFactory factory, int amount)
        {
            _pool = new ObjectPool<Projectile>(OnCreated, OnGet, OnRelease, OnProjectileDestroy, true, _amount);
            
            _projectileFactory = factory;
            _amount = amount;
        }

        public void Dispose()
        {
            _pool?.Dispose();
        }

        public Projectile Get() => _pool.Get();

        private Projectile OnCreated()
        {
            Projectile projectile = _projectileFactory.Create();
            projectile.Exploded += OnExploded;
            
            return projectile;
        }

        private void OnGet(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.ResetProjectile();
            Count++;
            CountChanged?.Invoke(Count);
        }

        private void OnRelease(Projectile projectile)
        {
            Count--;
            CountChanged?.Invoke(Count);
        }

        private void OnExploded(Projectile projectile)
        {
            _pool.Release(projectile);
        }

        private void OnProjectileDestroy(Projectile projectile)
        {
            projectile.Exploded -= OnExploded;
        }
    }
}
