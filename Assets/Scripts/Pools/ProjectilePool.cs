using System;
using Configs;
using EventProviders;
using Factories;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;

namespace Pools
{
    [CreateAssetMenu(fileName = "ProjectilePool", menuName = "ProjectilePool", order = 0)]
    public class ProjectilePool : ScriptableObject
    {
        [SerializeField] private ProjectileFactory _projectileFactory;
        [SerializeField] private int _amount;

        private ObjectPool<Projectile> _pool;

        public ProjectileFactory ProjectileFactory => _projectileFactory;
        
        public event Action<Projectile, ProjectileConfig> Got;
        public event Action Released;
        
        public static int Count { get; private set; }

        public static event Action<int> CountChanged;

        private void Awake()
        {
            _pool = new ObjectPool<Projectile>(OnCreated, OnGet, OnRelease, OnProjectileDestroy, true, _amount);
        }

        private void OnDestroy()
        {
            _pool.Dispose();
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
            Got?.Invoke(projectile, _projectileFactory.Config);
        }

        private void OnRelease(Projectile projectile)
        {
            Count--;
            CountChanged?.Invoke(Count);
            Released?.Invoke();
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
