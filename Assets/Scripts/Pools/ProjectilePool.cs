using System;
using Configs;
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
        private FollowingObject _followingTimerView;

        private ProjectileConfig Config => _projectileFactory.Config;

        public event Action<Projectile, ProjectileConfig> Got;
        public event Action Released;
        
        public static int Count { get; private set; }

        public static event Action<int> CountChanged;
        
        public void Init(FollowingObject followingTimerView)
        {
            _followingTimerView = followingTimerView;
            
            _pool = new ObjectPool<Projectile>(OnCreated, OnGet, OnRelease, OnDestroy, true, _amount);
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        public Projectile Get() => _pool.Get();

        private void OnGet(Projectile projectile)
        {
            projectile.ResetProjectile();
            Count++;
            CountChanged?.Invoke(Count);
            Got?.Invoke(projectile, Config);
        }

        private void OnRelease(Projectile projectile)
        {
            Count--;
            CountChanged?.Invoke(Count);
            Released?.Invoke();
        }

        private Projectile OnCreated()
        {
            Projectile projectile = _projectileFactory.Create();
            projectile.Exploded += OnExploded;
            return projectile;
        }

        private void OnExploded(Projectile projectile)
        {
            _pool.Release(projectile);
        }

        private void OnDestroy(Projectile projectile)
        {
            projectile.Exploded -= OnExploded;
        }
    }
}
