using System;
using Configs;
using Projectiles;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Pools
{
    public class ExplosionPool : IDisposable
    {
        private readonly ExplosionConfig _config;
        private int _amount;

        private readonly Transform _objectsParent;
        private readonly IShovel _shovelWrapper;

        private readonly ObjectPool<Explosion> _pool;
        private readonly IProjectileEvents _projectileEvents;

        public static int Count { get; private set; }

        public ExplosionPool(ExplosionConfig config, Transform objectsParent, IShovel shovelWrapper, IProjectileEvents projectileEvents, int capacity = 5)
        {
            _config = config;
            _projectileEvents = projectileEvents;
            _objectsParent = objectsParent;
            _shovelWrapper = shovelWrapper;
            
            _pool = new ObjectPool<Explosion>(CreateObject, OnGet, OnRelease, OnExplosionDestroy, defaultCapacity: _amount);
            
            _projectileEvents.Exploded += OnExploded;
        }

        private void OnExploded(Projectile projectile)
        {
            
            Explosion explosion = Get();
            ExplosionConfig explosionConfig = projectile.Config.ExplosionConfig;
            explosion.Explode(explosionConfig, projectile.transform.position);
        }

        public void Dispose()
        {
            _projectileEvents.Exploded -= OnExploded;
            _pool?.Dispose();
        }

        public Explosion Get() => _pool.Get();

        private Explosion CreateObject()
        {
            var explosion = Instantiate(_config.Prefab, _objectsParent);
            explosion.Init(_shovelWrapper);
            explosion.AnimationStopped += OnAnimationStopped;

            explosion.gameObject.SetActive(false);
            return explosion;
        }

        private void OnAnimationStopped(Explosion explosion)
        {
            _pool.Release(explosion);
        }

        private void OnExplosionDestroy(Explosion explosion)
        {
            explosion.AnimationStopped -= OnAnimationStopped;
        }

        private void OnGet(Explosion explosion)
        {
            explosion.gameObject.SetActive(true);
            Count++;
        }

        private void OnRelease(Explosion explosion)
        {
            explosion.gameObject.SetActive(false);
            Count--;
        }
    }
}